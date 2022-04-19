using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReconNess.Core.Services;
using ReconNess.Entities;
using ReconNess.Managers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.UnitTests
{
    [TestClass]
    public class AgentServerManagerTest
    {
        [TestMethod]
        public async Task TestGetAvailableServerSimpleAsync()
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
        public async Task TestGetAvailableServerSimpleOneServerAsync()
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
        public async Task TestGetAvailableServerComplexTwoServerAsync()
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
            var channel1 = await agentServerManager.GetAvailableServerAsync("my-channel-1");
            var channel2 = await agentServerManager.GetAvailableServerAsync("my-channel-2");
            var channel3 = await agentServerManager.GetAvailableServerAsync("my-channel-1");

            // Assert
            Assert.IsTrue(1 == channel1);
            Assert.IsTrue(2 == channel2);
            Assert.IsTrue(1 == channel3);
        }

        [TestMethod]
        public async Task TestGetAvailableServerComplexAsync()
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
    }
}
