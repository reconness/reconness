using Microsoft.Extensions.Configuration;
using NLog;
using RabbitMQ.Client;
using ReconNess.Core.Models;
using ReconNess.Core.Providers;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Providers
{
    public class AgentRunnerQueueProvider : IQueueProvider<AgentRunnerQueue>
    {
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly IModel channel;
        private readonly int reconnessAgentCount;

        public AgentRunnerQueueProvider(IConfiguration configuration)
        {
            var rabbitmqConnectionString = configuration.GetConnectionString("DefaultRabbitmqConnection");

            var rabbitMQUserName = Environment.GetEnvironmentVariable("RabbitMQUser") ??
                                 Environment.GetEnvironmentVariable("RabbitMQUser", EnvironmentVariableTarget.User);
            var rabbitMQPassword = Environment.GetEnvironmentVariable("RabbitMQPassword") ??
                             Environment.GetEnvironmentVariable("RabbitMQPassword", EnvironmentVariableTarget.User);

            var reconnessAgentCountFromEnv = Environment.GetEnvironmentVariable("ReconnessAgentCount") ??
                             Environment.GetEnvironmentVariable("ReconnessAgentCount", EnvironmentVariableTarget.User);

            rabbitmqConnectionString = rabbitmqConnectionString.Replace("{{username}}", rabbitMQUserName)
                                                               .Replace("{{password}}", rabbitMQPassword);

            if (!int.TryParse(reconnessAgentCountFromEnv, out int result))
            {
                result = 1;
            }

            reconnessAgentCount = result;

            var factory = new ConnectionFactory() { Uri = new Uri(rabbitmqConnectionString) };

            var conn = factory.CreateConnection();

            this.channel = conn.CreateModel();

            this.channel.ExchangeDeclare("reconness", ExchangeType.Direct);
        }

        public async Task EnqueueAsync(AgentRunnerQueue agentRunnerQueue, CancellationToken cancellationToken)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(agentRunnerQueue));
            
            string agentRoutingKey = await this.GetAgentRoutingKeyAsync();
            _logger.Info($"Send to the routingKey {agentRoutingKey}");

            lock (this.channel)
            {
                this.channel.BasicPublish("reconness", agentRoutingKey,
                                basicProperties: this.channel.CreateBasicProperties(),
                                body: body);
            }
        }

        private Task<string> GetAgentRoutingKeyAsync()
        {

            var r = new Random(DateTime.Now.Millisecond);

            return Task.FromResult($"reconness-{r.Next(1, reconnessAgentCount + 1)}");
        }
    }
}
