using Moq;
using ReconNess.Application.Managers;
using ReconNess.Domain.Entities;
using ReconNess.Domain.Enum;
using ReconNess.Infrastructure.Managers;

namespace ReconNess.Application.Services.UnitTests;

public class AgentServerManagerTest
{
    [Fact]
    public async Task TestGetAvailableServerRounRobinSimpleAsync()
    {
        // Arrange
        var agentServerSettingMock = new Mock<IAgentsSettingServerManager>();

        var agentsSettings = new AgentsSetting
        {
            Strategy = AgentRunnerStrategy.ROUND_ROBIN,
            AgentServerCount = 4
        };
        

        agentServerSettingMock.Setup(c => c.GetAgentSettingAsync(It.IsAny<CancellationToken>()))
           .Returns<CancellationToken>(c =>
           {
               return Task.FromResult(agentsSettings);
           });

        var agentServerManager = new AgentServerManager(agentServerSettingMock.Object);

        // Act
        var channel1 = await agentServerManager.GetAvailableServerAsync("my-channel-1");
        var channel2 = await agentServerManager.GetAvailableServerAsync("my-channel-2");
        var channel3 = await agentServerManager.GetAvailableServerAsync("my-channel-1");

        // Assert
        Assert.True(1 == channel1);
        Assert.True(2 == channel2);
        Assert.True(1 == channel3);
    }
    
    [Fact]
    public async Task TestGetAvailableServerGreedySimpleAsync()
    {
        // Arrange
        var agentServerSettingMock = new Mock<IAgentsSettingServerManager>();

        var agentsSettings = new AgentsSetting
        {
            Strategy = AgentRunnerStrategy.GREEDY,
            AgentServerCount = 4
        };


        agentServerSettingMock.Setup(c => c.GetAgentSettingAsync(It.IsAny<CancellationToken>()))
           .Returns<CancellationToken>(c =>
           {
               return Task.FromResult(agentsSettings);
           });

        var agentServerManager = new AgentServerManager(agentServerSettingMock.Object);

        // Act
        var channel1 = await agentServerManager.GetAvailableServerAsync("my-channel-1");
        var channel2 = await agentServerManager.GetAvailableServerAsync("my-channel-2");
        var channel3 = await agentServerManager.GetAvailableServerAsync("my-channel-1");

        // Assert
        Assert.True(1 == channel1);
        Assert.True(2 == channel2);
        Assert.True(3 == channel3);
    }

    [Fact]
    public async Task TestGetAvailableServerSimpleRounRobinOneServerAsync()
    {
        // Arrange
        var agentServerSettingMock = new Mock<IAgentsSettingServerManager>();

        var agentsSettings = new AgentsSetting
        {
            Strategy = AgentRunnerStrategy.ROUND_ROBIN,
            AgentServerCount = 1
        };

        agentServerSettingMock.Setup(c => c.GetAgentSettingAsync(It.IsAny<CancellationToken>()))
            .Returns<CancellationToken>(c =>
            {
                return Task.FromResult(agentsSettings);
            });

        var agentServerManager = new AgentServerManager(agentServerSettingMock.Object);

        // Act
        var channel1 = await agentServerManager.GetAvailableServerAsync("my-channel-1");
        var channel2 = await agentServerManager.GetAvailableServerAsync("my-channel-2");
        var channel3 = await agentServerManager.GetAvailableServerAsync("my-channel-1");

        // Assert
        Assert.True(1 == channel1);
        Assert.True(1 == channel2);
        Assert.True(1 == channel3);
    }

    [Fact]
    public async Task TestGetAvailableServerSimpleGreedyOneServerAsync()
    {
        // Arrange
        var agentServerSettingMock = new Mock<IAgentsSettingServerManager>();

        var agentsSettings = new AgentsSetting
        {
            Strategy = AgentRunnerStrategy.GREEDY,
            AgentServerCount = 1
        };

        agentServerSettingMock.Setup(c => c.GetAgentSettingAsync(It.IsAny<CancellationToken>()))
           .Returns<CancellationToken>(c =>
           {
               return Task.FromResult(agentsSettings);
           });

        var agentServerManager = new AgentServerManager(agentServerSettingMock.Object);

        // Act
        var channel1 = await agentServerManager.GetAvailableServerAsync("my-channel-1");
        var channel2 = await agentServerManager.GetAvailableServerAsync("my-channel-2");
        var channel3 = await agentServerManager.GetAvailableServerAsync("my-channel-1");

        // Assert
        Assert.True(1 == channel1);
        Assert.True(1 == channel2);
        Assert.True(1 == channel3);
    }

    [Fact]
    public async Task TestGetAvailableServerComplexRounRobinTwoServerAsync()
    {
        // Arrange
        var agentServerSettingMock = new Mock<IAgentsSettingServerManager>();

        var agentsSettings = new AgentsSetting
        {
            Strategy = AgentRunnerStrategy.ROUND_ROBIN,
            AgentServerCount = 2
        };

        agentServerSettingMock.Setup(c => c.GetAgentSettingAsync(It.IsAny<CancellationToken>()))
           .Returns<CancellationToken>(c =>
           {
               return Task.FromResult(agentsSettings);
           });

        var agentServerManager = new AgentServerManager(agentServerSettingMock.Object);

        // Act
        // Assert
        foreach (var i in Enumerable.Range(0, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-1");
            Assert.True(1 == channel);
        }

        foreach (var i in Enumerable.Range(0, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-2");
            Assert.True(2 == channel);
        }

        foreach (var i in Enumerable.Range(0, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-1");
            Assert.True(1 == channel);
        }

        foreach (var i in Enumerable.Range(0, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-3");
            Assert.True(2 == channel);
        }

        foreach (var i in Enumerable.Range(0, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-4");
            Assert.True(1 == channel);
        }
    }

    [Fact]
    public async Task TestGetAvailableServerComplexGreedyTwoServerAsync()
    {
        // Arrange
        var agentServerSettingMock = new Mock<IAgentsSettingServerManager>();

        var agentsSettings = new AgentsSetting
        {
            Strategy = AgentRunnerStrategy.GREEDY,
            AgentServerCount = 2
        };

        agentServerSettingMock.Setup(c => c.GetAgentSettingAsync(It.IsAny<CancellationToken>()))
           .Returns<CancellationToken>(c =>
           {
               return Task.FromResult(agentsSettings);
           });

        var agentServerManager = new AgentServerManager(agentServerSettingMock.Object);

        // Act
        // Assert
        foreach (var i in Enumerable.Range(1, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-1");
            if (i % 2 == 1)
            {
                Assert.True(1 == channel);
            }
            else
            {
                Assert.True(2 == channel);
            }                
        }

        foreach (var i in Enumerable.Range(1, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-2");
            if (i % 2 == 1)
            {
                Assert.True(1 == channel);
            }
            else
            {
                Assert.True(2 == channel);
            }
        }

        foreach (var i in Enumerable.Range(1, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-1");
            if (i % 2 == 1)
            {
                Assert.True(1 == channel);
            }
            else
            {
                Assert.True(2 == channel);
            }
        }

        foreach (var i in Enumerable.Range(1, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-3");
            if (i % 2 == 1)
            {
                Assert.True(1 == channel);
            }
            else
            {
                Assert.True(2 == channel);
            }
        }

        foreach (var i in Enumerable.Range(1, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-4");
            if (i % 2 == 1)
            {
                Assert.True(1 == channel);
            }
            else
            {
                Assert.True(2 == channel);
            }
        }
    }

    [Fact]
    public async Task TestGetAvailableServerRounRobinComplexAsync()
    {
        // Arrange
        var agentServerSettingMock = new Mock<IAgentsSettingServerManager>();

        var agentsSettings = new AgentsSetting
        {
            Strategy = AgentRunnerStrategy.ROUND_ROBIN,
            AgentServerCount = 4
        };

        agentServerSettingMock.Setup(c => c.GetAgentSettingAsync(It.IsAny<CancellationToken>()))
            .Returns<CancellationToken>(c =>
            {
                return Task.FromResult(agentsSettings);
            });

        var agentServerManager = new AgentServerManager(agentServerSettingMock.Object);

        // Assert
        foreach (var i in Enumerable.Range(0, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-1");
            Assert.True(1 == channel);
        }

        foreach (var i in Enumerable.Range(0, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-2");
            Assert.True(2 == channel);
        }

        foreach (var i in Enumerable.Range(0, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-1");
            Assert.True(1 == channel);
        }

        foreach (var i in Enumerable.Range(0, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-3");
            Assert.True(3 == channel);
        }

        foreach (var i in Enumerable.Range(0, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-4");
            Assert.True(4 == channel);
        }

        foreach (var i in Enumerable.Range(0, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-5");
            Assert.True(2 == channel);
        }
    }

    [Fact]
    public async Task TestGetAvailableServerRounRobinParalleltComplexAsync()
    {
        // Arrange
        var agentServerSettingMock = new Mock<IAgentsSettingServerManager>();

        var agentsSettings = new AgentsSetting
        {
            Strategy = AgentRunnerStrategy.ROUND_ROBIN,
            AgentServerCount = 4
        };

        agentServerSettingMock.Setup(c => c.GetAgentSettingAsync(It.IsAny<CancellationToken>()))
           .Returns<CancellationToken>(c =>
           {
               return Task.FromResult(agentsSettings);
           });

        var agentServerManager = new AgentServerManager(agentServerSettingMock.Object);

        // Assert
        await Parallel.ForEachAsync<int>(Enumerable.Range(0, 100), async (i, c) =>
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-1");
            Assert.True(1 == channel);

            var channel2 = await agentServerManager.GetAvailableServerAsync("my-channel-2");
            Assert.True(2 == channel2);

            var channel3 = await agentServerManager.GetAvailableServerAsync("my-channel-1");
            Assert.True(1 == channel3);

            var channel4 = await agentServerManager.GetAvailableServerAsync("my-channel-3");
            Assert.True(3 == channel4);

            var channel5 = await agentServerManager.GetAvailableServerAsync("my-channel-4");
            Assert.True(4 == channel5);
        });
    }

    [Fact]
    public async Task TestGetAvailableServerGreedyComplexAsync()
    {
        // Arrange
        var agentServerSettingMock = new Mock<IAgentsSettingServerManager>();

        var agentsSettings = new AgentsSetting
        {
            Strategy = AgentRunnerStrategy.GREEDY,
            AgentServerCount = 4
        };

        agentServerSettingMock.Setup(c => c.GetAgentSettingAsync(It.IsAny<CancellationToken>()))
           .Returns<CancellationToken>(c =>
           {
               return Task.FromResult(agentsSettings);
           });

        var agentServerManager = new AgentServerManager(agentServerSettingMock.Object);

        // Assert
        foreach (var i in Enumerable.Range(1, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-1");
            if (i % 4 == 1)
            {
                Assert.True(1 == channel);
            }
            else if (i % 4 == 2)
            {
                Assert.True(2 == channel);
            }
            else if (i % 4 == 3)
            {
                Assert.True(3 == channel);
            }
            else
            {
                Assert.True(4 == channel);
            }
        }

        foreach (var i in Enumerable.Range(1, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-2");
            if (i % 4 == 1)
            {
                Assert.True(1 == channel);
            }
            else if (i % 4 == 2)
            {
                Assert.True(2 == channel);
            }
            else if (i % 4 == 3)
            {
                Assert.True(3 == channel);
            }
            else
            {
                Assert.True(4 == channel);
            }
        }

        foreach (var i in Enumerable.Range(1, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-1");
            if (i % 4 == 1)
            {
                Assert.True(1 == channel);
            }
            else if (i % 4 == 2)
            {
                Assert.True(2 == channel);
            }
            else if (i % 4 == 3)
            {
                Assert.True(3 == channel);
            }
            else
            {
                Assert.True(4 == channel);
            }
        }

        foreach (var i in Enumerable.Range(1, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-3");
            if (i % 4 == 1)
            {
                Assert.True(1 == channel);
            }
            else if (i % 4 == 2)
            {
                Assert.True(2 == channel);
            }
            else if (i % 4 == 3)
            {
                Assert.True(3 == channel);
            }
            else
            {
                Assert.True(4 == channel);
            }
        }

        foreach (var i in Enumerable.Range(1, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-4");
            if (i % 4 == 1)
            {
                Assert.True(1 == channel);
            }
            else if (i % 4 == 2)
            {
                Assert.True(2 == channel);
            }
            else if (i % 4 == 3)
            {
                Assert.True(3 == channel);
            }
            else
            {
                Assert.True(4 == channel);
            }
        }

        foreach (var i in Enumerable.Range(1, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-5");
            if (i % 4 == 1)
            {
                Assert.True(1 == channel);
            }
            else if (i % 4 == 2)
            {
                Assert.True(2 == channel);
            }
            else if (i % 4 == 3)
            {
                Assert.True(3 == channel);
            }
            else
            {
                Assert.True(4 == channel);
            }
        }
    }

    [Fact]
    public async Task TestGetAvailableServerComplexRounRobinRefreshTwoServerAsync()
    {
        // Arrange
        var agentServerSettingMock = new Mock<IAgentsSettingServerManager>();

        var agentsSettings = new AgentsSetting
        {
            Strategy = AgentRunnerStrategy.ROUND_ROBIN,
            AgentServerCount = 2
        };

        agentServerSettingMock.Setup(c => c.GetAgentSettingAsync(It.IsAny<CancellationToken>()))
           .Returns<CancellationToken>(c =>
           {
               return Task.FromResult(agentsSettings);
           });

        var agentServerManager = new AgentServerManager(agentServerSettingMock.Object);

        // Act
        // Assert
        foreach (var i in Enumerable.Range(0, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-1", 1);
            Assert.True(1 == channel);
        }

        foreach (var i in Enumerable.Range(0, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-2", 1);
            Assert.True(2 == channel);
        }

        foreach (var i in Enumerable.Range(0, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-1", 1);
            Assert.True(1 == channel);
        }

        await Task.Delay(1000 * 60);

        foreach (var i in Enumerable.Range(0, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-3", 1);
            Assert.True(1 == channel);
        }

        foreach (var i in Enumerable.Range(0, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-4", 1);
            Assert.True(2 == channel);
        }
    }

    [Fact]
    public async Task TestGetAvailableServerComplexGreedyRefreshTwoServerAsync()
    {
        // Arrange
        var agentServerSettingMock = new Mock<IAgentsSettingServerManager>();

        var agentsSettings = new AgentsSetting
        {
            Strategy = AgentRunnerStrategy.GREEDY,
            AgentServerCount = 2
        };

        agentServerSettingMock.Setup(c => c.GetAgentSettingAsync(It.IsAny<CancellationToken>()))
           .Returns<CancellationToken>(c =>
           {
               return Task.FromResult(agentsSettings);
           });

        var agentServerManager = new AgentServerManager(agentServerSettingMock.Object);

        // Act
        // Assert
        foreach (var i in Enumerable.Range(1, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-1", 1);
            if (i % 2 == 1)
            {
                Assert.True(1 == channel);
            }
            else
            {
                Assert.True(2 == channel);
            }
        }

        foreach (var i in Enumerable.Range(1, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-2", 1);
            if (i % 2 == 1)
            {
                Assert.True(1 == channel);
            }
            else
            {
                Assert.True(2 == channel);
            }
        }

        foreach (var i in Enumerable.Range(1, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-1", 1);
            if (i % 2 == 1)
            {
                Assert.True(1 == channel);
            }
            else
            {
                Assert.True(2 == channel);
            }
        }

        await Task.Delay(1000 * 60);

        foreach (var i in Enumerable.Range(1, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-3", 1);
            if (i % 2 == 1)
            {
                Assert.True(1 == channel);
            }
            else
            {
                Assert.True(2 == channel);
            }
        }

        foreach (var i in Enumerable.Range(1, 100))
        {
            var channel = await agentServerManager.GetAvailableServerAsync("my-channel-4", 1);
            if (i % 2 == 1)
            {
                Assert.True(1 == channel);
            }
            else
            {
                Assert.True(2 == channel);
            }
        }
    }
}
