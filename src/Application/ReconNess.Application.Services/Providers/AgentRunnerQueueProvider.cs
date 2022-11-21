using Microsoft.Extensions.Configuration;
using NLog;
using RabbitMQ.Client;
using ReconNess.Core.Models;
using ReconNess.Core.Providers;
using System;
using System.Text;
using System.Text.Json;

namespace ReconNess.Providers
{
    public class AgentRunnerQueueProvider : IQueueProvider<AgentRunnerQueue>
    {
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        
        private readonly IModel channel;

        public AgentRunnerQueueProvider(IConfiguration configuration)
        {
            var rabbitmqConnectionString = configuration.GetConnectionString("DefaultRabbitmqConnection");

            var rabbitMQUserName = Environment.GetEnvironmentVariable("RabbitMQUser") ??
                                 Environment.GetEnvironmentVariable("RabbitMQUser", EnvironmentVariableTarget.User);
            var rabbitMQPassword = Environment.GetEnvironmentVariable("RabbitMQPassword") ??
                             Environment.GetEnvironmentVariable("RabbitMQPassword", EnvironmentVariableTarget.User);

            rabbitmqConnectionString = rabbitmqConnectionString.Replace("{{username}}", rabbitMQUserName)
                                                               .Replace("{{password}}", rabbitMQPassword);
            

            var factory = new ConnectionFactory() { Uri = new Uri(rabbitmqConnectionString) };

            var conn = factory.CreateConnection();

            this.channel = conn.CreateModel();

            this.channel.ExchangeDeclare("reconness", ExchangeType.Direct);
        }

        public void Enqueue(AgentRunnerQueue agentRunnerQueue)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(agentRunnerQueue));
            var routingKey = $"reconness-{agentRunnerQueue.ServerNumber}";

            _logger.Info($"Send to the routingKey {routingKey}");

            lock (this.channel)
            {
                this.channel.BasicPublish("reconness", routingKey,
                                basicProperties: this.channel.CreateBasicProperties(),
                                body: body);
            }
        }
    }
}
