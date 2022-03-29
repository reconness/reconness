using Microsoft.Extensions.Configuration;
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

            this.channel.ExchangeDeclare("reconness", ExchangeType.Direct, true);
            this.channel.QueueDeclare("reconness-queue", true);

            this.channel.QueueBind("reconness-queue", "reconness", "reconness");
        }

        public Task EnqueueAsync(AgentRunnerQueue agentRunnerQueue, CancellationToken cancellationToken)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(agentRunnerQueue));
            this.channel.BasicPublish("reconness", "reconness",
                                basicProperties: this.channel.CreateBasicProperties(),
                                body: body);

            return Task.CompletedTask;
        }
    }
}
