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
        [TestMethod]
        public void TestGetCategoriesAsyncNotNewCategoryMethod()
        {
            // Arrange
            var addWasCalled = false;

            var categoryIdOnDb = Guid.NewGuid();
            var categoriesOnDb = new List<Category>
            { 
                new Category 
                {
                    Id = categoryIdOnDb,
                    Name = "Brute Force" 
                }                
            };

            var myCategoriesOnDb = new List<AgentCategory>
            {
                new AgentCategory
                {
                    Category = new Category
                    {
                        Id = categoryIdOnDb,
                        Name = "Brute Force"
                    }
                }
            };

            var myNewCategories = new List<string> { "Brute Force" };

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
                    addWasCalled = true;
                });

            unitOfWorkMock.Setup(m => m.Repository<Category>(It.IsAny<CancellationToken>()))
                .Returns(repositoryMock.Object);

            // Act
            var categoryService = new CategoryService(unitOfWorkMock.Object);
            var categorties = categoryService.GetCategoriesAsync(myCategoriesOnDb, myNewCategories).Result;

            // Assert
            Assert.IsTrue(categorties.Count == 1);
            Assert.IsTrue(addWasCalled == false);
        }

        [TestMethod]
        public void TestGetCategoriesAsyncRemoveCategoryMethod()
        {
            // Arrange
            var addWasCalled = false;

            var categoryIdOnDb = Guid.NewGuid();
            var categoriesOnDb = new List<Category>
            {
                new Category
                {
                    Id = categoryIdOnDb,
                    Name = "Brute Force"
                }
            };

            var myCategoriesOnDb = new List<AgentCategory>
            {
                new AgentCategory
                {
                    Category = new Category
                    {
                        Id = categoryIdOnDb,
                        Name = "Brute Force"
                    }
                }
            };

            var myNewCategories = new List<string> ();

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
                    addWasCalled = true;
                });

            unitOfWorkMock.Setup(m => m.Repository<Category>(It.IsAny<CancellationToken>()))
                .Returns(repositoryMock.Object);

            // Act
            var categoryService = new CategoryService(unitOfWorkMock.Object);
            var categorties = categoryService.GetCategoriesAsync(myCategoriesOnDb, myNewCategories).Result;

            // Assert
            Assert.IsTrue(categorties.Count == 0);
            Assert.IsTrue(addWasCalled == false);
        }

        [TestMethod]
        public void TestGetCategoriesAsyncAddNewCategoryMethod()
        {
            // Arrange
            var addWasCalled = false;

            var categoryIdOnDb = Guid.NewGuid();
            var categoriesOnDb = new List<Category>
            {
                new Category
                {
                    Id = categoryIdOnDb,
                    Name = "Brute Force"
                }
            };

            var myCategoriesOnDb = new List<AgentCategory>
            {
                new AgentCategory
                {
                    Category = new Category
                    {
                        Id = categoryIdOnDb,
                        Name = "Brute Force"
                    }
                }
            };

            var myNewCategories = new List<string> { "Brute Force", "New Category" };

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
                    addWasCalled = true;
                });

            unitOfWorkMock.Setup(m => m.Repository<Category>(It.IsAny<CancellationToken>()))
                .Returns(repositoryMock.Object);

            // Act
            var categoryService = new CategoryService(unitOfWorkMock.Object);
            var categorties = categoryService.GetCategoriesAsync(myCategoriesOnDb, myNewCategories).Result;

            // Assert
            Assert.IsTrue(categorties.Count == 2);
            Assert.IsTrue(addWasCalled == true);
        }

        [TestMethod]
        public void TestGetCategoriesAsyncAddAllNewCategoryMethod()
        {
            // Arrange
            var addWasCalled = false;

            var categoryIdOnDb = Guid.NewGuid();
            var categoriesOnDb = new List<Category>
            {
                new Category
                {
                    Id = categoryIdOnDb,
                    Name = "Brute Force"
                }
            };

            var myCategoriesOnDb = new List<AgentCategory>
            {
                new AgentCategory
                {
                    Category = new Category
                    {
                        Id = categoryIdOnDb,
                        Name = "Brute Force"
                    }
                }
            };

            var myNewCategories = new List<string> { "New Category" };

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
                    addWasCalled = true;
                });

            unitOfWorkMock.Setup(m => m.Repository<Category>(It.IsAny<CancellationToken>()))
                .Returns(repositoryMock.Object);

            // Act
            var categoryService = new CategoryService(unitOfWorkMock.Object);
            var categorties = categoryService.GetCategoriesAsync(myCategoriesOnDb, myNewCategories).Result;

            // Assert
            Assert.IsTrue(categorties.Count == 1);
            Assert.IsTrue(addWasCalled == true);
        }

        [TestMethod]
        public void TestGetCategoriesAsyncAddAllNewDuplicateCategoryMethod()
        {
            // Arrange
            var addWasCalled = false;

            var categoryIdOnDb = Guid.NewGuid();
            var categoriesOnDb = new List<Category>
            {
                new Category
                {
                    Id = categoryIdOnDb,
                    Name = "Brute Force"
                }
            };

            var myCategoriesOnDb = new List<AgentCategory>
            {
                new AgentCategory
                {
                    Category = new Category
                    {
                        Id = categoryIdOnDb,
                        Name = "Brute Force"
                    }
                }
            };

            var myNewCategories = new List<string> { "New Category", "New Category" };

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
                    addWasCalled = true;
                });

            unitOfWorkMock.Setup(m => m.Repository<Category>(It.IsAny<CancellationToken>()))
                .Returns(repositoryMock.Object);

            // Act
            var categoryService = new CategoryService(unitOfWorkMock.Object);
            var categorties = categoryService.GetCategoriesAsync(myCategoriesOnDb, myNewCategories).Result;

            // Assert
            Assert.IsTrue(categorties.Count == 1);
            Assert.IsTrue(addWasCalled == true);
        }
    }
}
