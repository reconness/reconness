using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReconNess.Core;
using ReconNess.Entities;
using ReconNess.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.UnitTests
{
    [TestClass]
    public class LabelServiceTests
    {
        private IUnitOfWork unitOfWork;
        private Guid labelIdOnDb;
        private bool addWasCalled = false;

        [TestInitialize]
        public void TestInitialize()
        {
            this.labelIdOnDb = Guid.NewGuid();
            var LabelsOnDb = new List<Label>
            {
                new Label
                {
                    Id = labelIdOnDb,
                    Name = "Brute Force"
                }
            };

            var repositoryMock = new Mock<IRepository<Label>>();
            repositoryMock.Setup(c => c.GetByCriteriaAsync(It.IsAny<Expression<Func<Label, bool>>>(), It.IsAny<CancellationToken>()))
                .Returns<Expression<Func<Label, bool>>, CancellationToken>((f, c) =>
                {
                    return Task.FromResult(LabelsOnDb.AsQueryable().FirstOrDefault(f));
                });

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(c => c.CommitAsync(It.IsAny<CancellationToken>()))
                .Callback(() =>
                {
                    this.addWasCalled = true;
                });

            unitOfWorkMock.Setup(m => m.Repository<Label>(It.IsAny<CancellationToken>()))
                .Returns(repositoryMock.Object);

            this.unitOfWork = unitOfWorkMock.Object;
        }

        [TestMethod]
        public void GetLabelsAsync_NotNewLabel()
        {
            var myNewLabels = new List<string> { "Brute Force" };

            // Arrange       
            var myLabelsOnDb = new List<SubdomainLabel>
            {
                new SubdomainLabel
                {
                    Label = new Label
                    {
                        Id = this.labelIdOnDb,
                        Name = "Brute Force"
                    }
                }
            };

            var labelService = new LabelService(this.unitOfWork);

            // Act
            var labels = labelService.GetLabelsAsync(myLabelsOnDb, myNewLabels).Result;

            // Assert
            Assert.IsTrue(labels.Count == 1);
            Assert.IsTrue(this.addWasCalled == false);
        }

        [TestMethod]
        public void GetLabelsAsync_RemoveLabel()
        {
            // Arrange
            var myLabelsOnDb = new List<SubdomainLabel>
            {
                new SubdomainLabel
                {
                    Label = new Label
                    {
                        Id = this.labelIdOnDb,
                        Name = "Brute Force"
                    }
                }
            };

            var myNewLabels = new List<string>();
            var labelService = new LabelService(this.unitOfWork);

            // Act
            var labels = labelService.GetLabelsAsync(myLabelsOnDb, myNewLabels).Result;

            // Assert
            Assert.IsTrue(labels.Count == 0);
            Assert.IsTrue(this.addWasCalled == false);
        }

        [TestMethod]
        public void GetLabelsAsync_AddNewLabel()
        {
            // Arrange
            var myLabelsOnDb = new List<SubdomainLabel>
            {
                new SubdomainLabel
                {
                    Label = new Label
                    {
                        Id = this.labelIdOnDb,
                        Name = "Brute Force"
                    }
                }
            };

            var myNewLabels = new List<string> { "Brute Force", "New Label" };
            var labelService = new LabelService(this.unitOfWork);

            // Act
            var labels = labelService.GetLabelsAsync(myLabelsOnDb, myNewLabels).Result;

            // Assert
            Assert.IsTrue(labels.Count == 2);
            Assert.IsTrue(this.addWasCalled == true);
        }

        [TestMethod]
        public void GetLabelsAsync_AddAllNewLabel()
        {
            // Arrange
            var myLabelsOnDb = new List<SubdomainLabel>
            {
                new SubdomainLabel
                {
                    Label = new Label
                    {
                        Id = this.labelIdOnDb,
                        Name = "Brute Force"
                    }
                }
            };

            var myNewLabels = new List<string> { "New Label" };
            var labelService = new LabelService(this.unitOfWork);

            // Act
            var labels = labelService.GetLabelsAsync(myLabelsOnDb, myNewLabels).Result;

            // Assert
            Assert.IsTrue(labels.Count == 1);
            Assert.IsTrue(this.addWasCalled == true);
        }

        [TestMethod]
        public void GetLabelsAsyncAdd_AllNewDuplicateLabel()
        {
            // Arrange
            var myLabelsOnDb = new List<SubdomainLabel>
            {
                new SubdomainLabel
                {
                    Label = new Label
                    {
                        Id = this.labelIdOnDb,
                        Name = "Brute Force"
                    }
                }
            };

            var myNewLabels = new List<string> { "New Label", "New Label" };
            var labelService = new LabelService(this.unitOfWork);

            // Act
            var labels = labelService.GetLabelsAsync(myLabelsOnDb, myNewLabels).Result;

            // Assert
            Assert.IsTrue(labels.Count == 1);
            Assert.IsTrue(this.addWasCalled == true);
        }
    }
}
