using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReconNess.Core;
using ReconNess.Entities;
using ReconNess.Services;

namespace ReconNess.UnitTests
{
    [TestClass]
    public class CategoryServiceTests
    {
        private IUnitOfWork unitOfWork;
        private Guid categoryIdOnDb;
        private bool addWasCalled = false;

        [TestInitialize]
        public void TestInitialize()
        {
            this.categoryIdOnDb = Guid.NewGuid();
            var categoriesOnDb = new List<Category>
            {
                new Category
                {
                    Id = categoryIdOnDb,
                    Name = "Brute Force"
                }
            };

            var repositoryMock = new Mock<IRepository<Category>>();
            repositoryMock.Setup(c => c.GetByCriteriaAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()))
                .Returns<Expression<Func<Category, bool>>, CancellationToken>((f, c) =>
                {
                    return Task.FromResult(categoriesOnDb.AsQueryable().FirstOrDefault(f));
                });

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(c => c.CommitAsync(It.IsAny<CancellationToken>()))
                .Callback(() =>
                {
                    this.addWasCalled = true;
                });

            unitOfWorkMock.Setup(m => m.Repository<Category>(It.IsAny<CancellationToken>()))
                .Returns(repositoryMock.Object);

            this.unitOfWork = unitOfWorkMock.Object;
        }

        [TestMethod]
        public void GetCategoriesAsync_NotNewCategory()
        {
            var myNewCategories = new List<string> { "Brute Force" };

            // Arrange       
            var myCategoriesOnDb = new List<AgentCategory>
            {
                new AgentCategory
                {
                    Category = new Category
                    {
                        Id = this.categoryIdOnDb,
                        Name = "Brute Force"
                    }
                }
            };   

            var categoryService = new CategoryService(this.unitOfWork);

            // Act
            var categories = categoryService.GetCategoriesAsync(myCategoriesOnDb, myNewCategories).Result;

            // Assert
            Assert.IsTrue(categories.Count == 1);
            Assert.IsTrue(this.addWasCalled == false);
        }

        [TestMethod]
        public void GetCategoriesAsync_RemoveCategory()
        {
            // Arrange
            var myCategoriesOnDb = new List<AgentCategory>
            {
                new AgentCategory
                {
                    Category = new Category
                    {
                        Id = this.categoryIdOnDb,
                        Name = "Brute Force"
                    }
                }
            };

            var myNewCategories = new List<string>();
            var categoryService = new CategoryService(this.unitOfWork);

            // Act
            var categories = categoryService.GetCategoriesAsync(myCategoriesOnDb, myNewCategories).Result;

            // Assert
            Assert.IsTrue(categories.Count == 0);
            Assert.IsTrue(this.addWasCalled == false);
        }

        [TestMethod]
        public void GetCategoriesAsync_AddNewCategory()
        {
            // Arrange
            var myCategoriesOnDb = new List<AgentCategory>
            {
                new AgentCategory
                {
                    Category = new Category
                    {
                        Id = this.categoryIdOnDb,
                        Name = "Brute Force"
                    }
                }
            };

            var myNewCategories = new List<string> { "Brute Force", "New Category" };
            var categoryService = new CategoryService(this.unitOfWork);

            // Act
            var categories = categoryService.GetCategoriesAsync(myCategoriesOnDb, myNewCategories).Result;

            // Assert
            Assert.IsTrue(categories.Count == 2);
            Assert.IsTrue(this.addWasCalled == true);
        }

        [TestMethod]
        public void GetCategoriesAsync_AddAllNewCategory()
        {
            // Arrange
            var myCategoriesOnDb = new List<AgentCategory>
            {
                new AgentCategory
                {
                    Category = new Category
                    {
                        Id = this.categoryIdOnDb,
                        Name = "Brute Force"
                    }
                }
            };

            var myNewCategories = new List<string> { "New Category" };
            var categoryService = new CategoryService(this.unitOfWork);

            // Act
            var categories = categoryService.GetCategoriesAsync(myCategoriesOnDb, myNewCategories).Result;

            // Assert
            Assert.IsTrue(categories.Count == 1);
            Assert.IsTrue(this.addWasCalled == true);
        }

        [TestMethod]
        public void GetCategoriesAsyncAdd_AllNewDuplicateCategory()
        {
            // Arrange
            var myCategoriesOnDb = new List<AgentCategory>
            {
                new AgentCategory
                {
                    Category = new Category
                    {
                        Id = this.categoryIdOnDb,
                        Name = "Brute Force"
                    }
                }
            };

            var myNewCategories = new List<string> { "New Category", "New Category" };
            var categoryService = new CategoryService(this.unitOfWork);

            // Act
            var categories = categoryService.GetCategoriesAsync(myCategoriesOnDb, myNewCategories).Result;

            // Assert
            Assert.IsTrue(categories.Count == 1);
            Assert.IsTrue(this.addWasCalled == true);
        }
    }
}
