using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Managers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.UnitTests
{
    [TestClass]
    public class AgentServerManagerTest
    {
        [TestMethod]
        public async Task TestGetAvailableServerRounRobinSimpleAsync()
        {
            // Arrange
            var agentsSettingServiceMock = new Mock<IAgentsSettingService>();

            var agentsSettings = new List<AgentsSetting>
            {
                new AgentsSetting
                {
                    Strategy = Entities.Enum.AgentRunnerStrategy.ROUND_ROBIN,
                    AgentServerCount = 4
                }
            };

            agentsSettingServiceMock.Setup(c => c.GetAllAsync(It.IsAny<CancellationToken>()))
                .Returns<CancellationToken>(c =>
                {
                    return Task.FromResult(agentsSettings);
                });

            var agentServerManager = new AgentServerManager(agentsSettingServiceMock.Object);

            // Act
            var channel1 = await agentServerManager.GetAvailableServerAsync("my-channel-1");
            var channel2 = await agentServerManager.GetAvailableServerAsync("my-channel-2");
            var channel3 = await agentServerManager.GetAvailableServerAsync("my-channel-1");

            // Assert
            Assert.IsTrue(1 == channel1);
            Assert.IsTrue(2 == channel2);
            Assert.IsTrue(1 == channel3);
        }

        [TestMethod]
        public async Task TestGetAvailableServerGreedySimpleAsync()
        {
            // Arrange
            var agentsSettingServiceMock = new Mock<IAgentsSettingService>();

            var agentsSettings = new List<AgentsSetting>
            {
                new AgentsSetting
                {
                    Strategy = Entities.Enum.AgentRunnerStrategy.GREEDY,
                    AgentServerCount = 4
                }
            };

            agentsSettingServiceMock.Setup(c => c.GetAllAsync(It.IsAny<CancellationToken>()))
                .Returns<CancellationToken>(c =>
                {
                    return Task.FromResult(agentsSettings);
                });

            var agentServerManager = new AgentServerManager(agentsSettingServiceMock.Object);

            // Act
            var channel1 = await agentServerManager.GetAvailableServerAsync("my-channel-1");
            var channel2 = await agentServerManager.GetAvailableServerAsync("my-channel-2");
            var channel3 = await agentServerManager.GetAvailableServerAsync("my-channel-1");

            // Assert
            Assert.IsTrue(1 == channel1);
            Assert.IsTrue(2 == channel2);
            Assert.IsTrue(3 == channel3);
        }

        [TestMethod]
        public async Task TestGetAvailableServerSimpleRounRobinOneServerAsync()
        {
            // Arrange
            var agentsSettingServiceMock = new Mock<IAgentsSettingService>();

            var agentsSettings = new List<AgentsSetting>
            {
                new AgentsSetting
                {
                    Strategy = Entities.Enum.AgentRunnerStrategy.ROUND_ROBIN,
                    AgentServerCount = 1
                }
            };

            agentsSettingServiceMock.Setup(c => c.GetAllAsync(It.IsAny<CancellationToken>()))
                .Returns<CancellationToken>(c =>
                {
                    return Task.FromResult(agentsSettings);
                });

            var agentServerManager = new AgentServerManager(agentsSettingServiceMock.Object);

            // Act
            var channel1 = await agentServerManager.GetAvailableServerAsync("my-channel-1");
            var channel2 = await agentServerManager.GetAvailableServerAsync("my-channel-2");
            var channel3 = await agentServerManager.GetAvailableServerAsync("my-channel-1");

            // Assert
            Assert.IsTrue(1 == channel1);
            Assert.IsTrue(1 == channel2);
            Assert.IsTrue(1 == channel3);
        }

        [TestMethod]
        public async Task TestGetAvailableServerSimpleGreedyOneServerAsync()
        {
            // Arrange
            var agentsSettingServiceMock = new Mock<IAgentsSettingService>();

            var agentsSettings = new List<AgentsSetting>
            {
                new AgentsSetting
                {
                    Strategy = Entities.Enum.AgentRunnerStrategy.GREEDY,
                    AgentServerCount = 1
                }
            };

            agentsSettingServiceMock.Setup(c => c.GetAllAsync(It.IsAny<CancellationToken>()))
                .Returns<CancellationToken>(c =>
                {
                    return Task.FromResult(agentsSettings);
                });

            var agentServerManager = new AgentServerManager(agentsSettingServiceMock.Object);

            // Act
            var channel1 = await agentServerManager.GetAvailableServerAsync("my-channel-1");
            var channel2 = await agentServerManager.GetAvailableServerAsync("my-channel-2");
            var channel3 = await agentServerManager.GetAvailableServerAsync("my-channel-1");

            // Assert
            Assert.IsTrue(1 == channel1);
            Assert.IsTrue(1 == channel2);
            Assert.IsTrue(1 == channel3);
        }

        [TestMethod]
        public async Task TestGetAvailableServerComplexRounRobinTwoServerAsync()
        {
            // Arrange
            var agentsSettingServiceMock = new Mock<IAgentsSettingService>();

            var agentsSettings = new List<AgentsSetting>
            {
                new AgentsSetting
                {
                    Strategy = Entities.Enum.AgentRunnerStrategy.ROUND_ROBIN,
                    AgentServerCount = 2
                }
            };

            agentsSettingServiceMock.Setup(c => c.GetAllAsync(It.IsAny<CancellationToken>()))
                .Returns<CancellationToken>(c =>
                {
                    return Task.FromResult(agentsSettings);
                });

            var agentServerManager = new AgentServerManager(agentsSettingServiceMock.Object);

            // Act
            // Assert
            foreach (var i in Enumerable.Range(0, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-1");
                Assert.IsTrue(1 == channel);
            }

            foreach (var i in Enumerable.Range(0, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-2");
                Assert.IsTrue(2 == channel);
            }

            foreach (var i in Enumerable.Range(0, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-1");
                Assert.IsTrue(1 == channel);
            }

            foreach (var i in Enumerable.Range(0, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-3");
                Assert.IsTrue(2 == channel);
            }

            foreach (var i in Enumerable.Range(0, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-4");
                Assert.IsTrue(1 == channel);
            }
        }

        [TestMethod]
        public async Task TestGetAvailableServerComplexGreedyTwoServerAsync()
        {
            // Arrange
            var agentsSettingServiceMock = new Mock<IAgentsSettingService>();

            var agentsSettings = new List<AgentsSetting>
            {
                new AgentsSetting
                {
                    Strategy = Entities.Enum.AgentRunnerStrategy.GREEDY,
                    AgentServerCount = 2
                }
            };

            agentsSettingServiceMock.Setup(c => c.GetAllAsync(It.IsAny<CancellationToken>()))
                .Returns<CancellationToken>(c =>
                {
                    return Task.FromResult(agentsSettings);
                });

            var agentServerManager = new AgentServerManager(agentsSettingServiceMock.Object);

            // Act
            // Assert
            foreach (var i in Enumerable.Range(1, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-1");
                if (i % 2 == 1)
                {
                    Assert.IsTrue(1 == channel);
                }
                else
                {
                    Assert.IsTrue(2 == channel);
                }                
            }

            foreach (var i in Enumerable.Range(1, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-2");
                if (i % 2 == 1)
                {
                    Assert.IsTrue(1 == channel);
                }
                else
                {
                    Assert.IsTrue(2 == channel);
                }
            }

            foreach (var i in Enumerable.Range(1, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-1");
                if (i % 2 == 1)
                {
                    Assert.IsTrue(1 == channel);
                }
                else
                {
                    Assert.IsTrue(2 == channel);
                }
            }

            foreach (var i in Enumerable.Range(1, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-3");
                if (i % 2 == 1)
                {
                    Assert.IsTrue(1 == channel);
                }
                else
                {
                    Assert.IsTrue(2 == channel);
                }
            }

            foreach (var i in Enumerable.Range(1, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-4");
                if (i % 2 == 1)
                {
                    Assert.IsTrue(1 == channel);
                }
                else
                {
                    Assert.IsTrue(2 == channel);
                }
            }
        }

        [TestMethod]
        public async Task TestGetAvailableServerRounRobinComplexAsync()
        {
            // Arrange
            var agentsSettingServiceMock = new Mock<IAgentsSettingService>();

            var agentsSettings = new List<AgentsSetting>
            {
                new AgentsSetting
                {
                    Strategy = Entities.Enum.AgentRunnerStrategy.ROUND_ROBIN,
                    AgentServerCount = 4
                }
            };

            agentsSettingServiceMock.Setup(c => c.GetAllAsync(It.IsAny<CancellationToken>()))
                .Returns<CancellationToken>(c =>
                {
                    return Task.FromResult(agentsSettings);
                });

            var agentServerManager = new AgentServerManager(agentsSettingServiceMock.Object);

            // Assert
            foreach (var i in Enumerable.Range(0, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-1");
                Assert.IsTrue(1 == channel);
            }

            foreach (var i in Enumerable.Range(0, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-2");
                Assert.IsTrue(2 == channel);
            }

            foreach (var i in Enumerable.Range(0, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-1");
                Assert.IsTrue(1 == channel);
            }

            foreach (var i in Enumerable.Range(0, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-3");
                Assert.IsTrue(3 == channel);
            }

            foreach (var i in Enumerable.Range(0, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-4");
                Assert.IsTrue(4 == channel);
            }

            foreach (var i in Enumerable.Range(0, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-5");
                Assert.IsTrue(2 == channel);
            }
        }

        [TestMethod]
        public async Task TestGetAvailableServerRounRobinParalleltComplexAsync()
        {
            // Arrange
            var agentsSettingServiceMock = new Mock<IAgentsSettingService>();

            var agentsSettings = new List<AgentsSetting>
            {
                new AgentsSetting
                {
                    Strategy = Entities.Enum.AgentRunnerStrategy.ROUND_ROBIN,
                    AgentServerCount = 4
                }
            };

            agentsSettingServiceMock.Setup(c => c.GetAllAsync(It.IsAny<CancellationToken>()))
                .Returns<CancellationToken>(c =>
                {
                    return Task.FromResult(agentsSettings);
                });

            var agentServerManager = new AgentServerManager(agentsSettingServiceMock.Object);

            // Assert
            await Parallel.ForEachAsync<int>(Enumerable.Range(0, 100), async (i, c) =>
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-1");
                Assert.IsTrue(1 == channel);

                var channel2 = await agentServerManager.GetAvailableServerAsync("my-channel-2");
                Assert.IsTrue(2 == channel2);

                var channel3 = await agentServerManager.GetAvailableServerAsync("my-channel-1");
                Assert.IsTrue(1 == channel3);

                var channel4 = await agentServerManager.GetAvailableServerAsync("my-channel-3");
                Assert.IsTrue(3 == channel4);

                var channel5 = await agentServerManager.GetAvailableServerAsync("my-channel-4");
                Assert.IsTrue(4 == channel5);
            });
        }

        [TestMethod]
        public async Task TestGetAvailableServerGreedyComplexAsync()
        {
            // Arrange
            var agentsSettingServiceMock = new Mock<IAgentsSettingService>();

            var agentsSettings = new List<AgentsSetting>
            {
                new AgentsSetting
                {
                    Strategy = Entities.Enum.AgentRunnerStrategy.GREEDY,
                    AgentServerCount = 4
                }
            };

            agentsSettingServiceMock.Setup(c => c.GetAllAsync(It.IsAny<CancellationToken>()))
                .Returns<CancellationToken>(c =>
                {
                    return Task.FromResult(agentsSettings);
                });

            var agentServerManager = new AgentServerManager(agentsSettingServiceMock.Object);

            // Assert
            foreach (var i in Enumerable.Range(1, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-1");
                if (i % 4 == 1)
                {
                    Assert.IsTrue(1 == channel);
                }
                else if (i % 4 == 2)
                {
                    Assert.IsTrue(2 == channel);
                }
                else if (i % 4 == 3)
                {
                    Assert.IsTrue(3 == channel);
                }
                else
                {
                    Assert.IsTrue(4 == channel);
                }
            }

            foreach (var i in Enumerable.Range(1, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-2");
                if (i % 4 == 1)
                {
                    Assert.IsTrue(1 == channel);
                }
                else if (i % 4 == 2)
                {
                    Assert.IsTrue(2 == channel);
                }
                else if (i % 4 == 3)
                {
                    Assert.IsTrue(3 == channel);
                }
                else
                {
                    Assert.IsTrue(4 == channel);
                }
            }

            foreach (var i in Enumerable.Range(1, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-1");
                if (i % 4 == 1)
                {
                    Assert.IsTrue(1 == channel);
                }
                else if (i % 4 == 2)
                {
                    Assert.IsTrue(2 == channel);
                }
                else if (i % 4 == 3)
                {
                    Assert.IsTrue(3 == channel);
                }
                else
                {
                    Assert.IsTrue(4 == channel);
                }
            }

            foreach (var i in Enumerable.Range(1, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-3");
                if (i % 4 == 1)
                {
                    Assert.IsTrue(1 == channel);
                }
                else if (i % 4 == 2)
                {
                    Assert.IsTrue(2 == channel);
                }
                else if (i % 4 == 3)
                {
                    Assert.IsTrue(3 == channel);
                }
                else
                {
                    Assert.IsTrue(4 == channel);
                }
            }

            foreach (var i in Enumerable.Range(1, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-4");
                if (i % 4 == 1)
                {
                    Assert.IsTrue(1 == channel);
                }
                else if (i % 4 == 2)
                {
                    Assert.IsTrue(2 == channel);
                }
                else if (i % 4 == 3)
                {
                    Assert.IsTrue(3 == channel);
                }
                else
                {
                    Assert.IsTrue(4 == channel);
                }
            }

            foreach (var i in Enumerable.Range(1, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-5");
                if (i % 4 == 1)
                {
                    Assert.IsTrue(1 == channel);
                }
                else if (i % 4 == 2)
                {
                    Assert.IsTrue(2 == channel);
                }
                else if (i % 4 == 3)
                {
                    Assert.IsTrue(3 == channel);
                }
                else
                {
                    Assert.IsTrue(4 == channel);
                }
            }
        }

        [TestMethod]
        public async Task TestGetAvailableServerComplexRounRobinRefreshTwoServerAsync()
        {
            // Arrange
            var agentsSettingServiceMock = new Mock<IAgentsSettingService>();

            var agentsSettings = new List<AgentsSetting>
            {
                new AgentsSetting
                {
                    Strategy = Entities.Enum.AgentRunnerStrategy.ROUND_ROBIN,
                    AgentServerCount = 2
                }
            };

            agentsSettingServiceMock.Setup(c => c.GetAllAsync(It.IsAny<CancellationToken>()))
                .Returns<CancellationToken>(c =>
                {
                    return Task.FromResult(agentsSettings);
                });

            var agentServerManager = new AgentServerManager(agentsSettingServiceMock.Object);

            // Act
            // Assert
            foreach (var i in Enumerable.Range(0, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-1", 1);
                Assert.IsTrue(1 == channel);
            }

            foreach (var i in Enumerable.Range(0, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-2", 1);
                Assert.IsTrue(2 == channel);
            }

            foreach (var i in Enumerable.Range(0, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-1", 1);
                Assert.IsTrue(1 == channel);
            }

            await Task.Delay(1000 * 60);

            foreach (var i in Enumerable.Range(0, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-3", 1);
                Assert.IsTrue(1 == channel);
            }

            foreach (var i in Enumerable.Range(0, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-4", 1);
                Assert.IsTrue(2 == channel);
            }
        }

        [TestMethod]
        public async Task TestGetAvailableServerComplexGreedyRefreshTwoServerAsync()
        {
            // Arrange
            var agentsSettingServiceMock = new Mock<IAgentsSettingService>();

            var agentsSettings = new List<AgentsSetting>
            {
                new AgentsSetting
                {
                    Strategy = Entities.Enum.AgentRunnerStrategy.GREEDY,
                    AgentServerCount = 2
                }
            };

            agentsSettingServiceMock.Setup(c => c.GetAllAsync(It.IsAny<CancellationToken>()))
                .Returns<CancellationToken>(c =>
                {
                    return Task.FromResult(agentsSettings);
                });

            var agentServerManager = new AgentServerManager(agentsSettingServiceMock.Object);

            // Act
            // Assert
            foreach (var i in Enumerable.Range(1, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-1", 1);
                if (i % 2 == 1)
                {
                    Assert.IsTrue(1 == channel);
                }
                else
                {
                    Assert.IsTrue(2 == channel);
                }
            }

            foreach (var i in Enumerable.Range(1, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-2", 1);
                if (i % 2 == 1)
                {
                    Assert.IsTrue(1 == channel);
                }
                else
                {
                    Assert.IsTrue(2 == channel);
                }
            }

            foreach (var i in Enumerable.Range(1, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-1", 1);
                if (i % 2 == 1)
                {
                    Assert.IsTrue(1 == channel);
                }
                else
                {
                    Assert.IsTrue(2 == channel);
                }
            }

            await Task.Delay(1000 * 60);

            foreach (var i in Enumerable.Range(1, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-3", 1);
                if (i % 2 == 1)
                {
                    Assert.IsTrue(1 == channel);
                }
                else
                {
                    Assert.IsTrue(2 == channel);
                }
            }

            foreach (var i in Enumerable.Range(1, 100))
            {
                var channel = await agentServerManager.GetAvailableServerAsync("my-channel-4", 1);
                if (i % 2 == 1)
                {
                    Assert.IsTrue(1 == channel);
                }
                else
                {
                    Assert.IsTrue(2 == channel);
                }
            }
        }
    }
}
