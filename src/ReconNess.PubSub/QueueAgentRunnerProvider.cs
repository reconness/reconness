using NLog;
using RabbitMQ.Client;
using ReconNess.Core.Models;
using ReconNess.Core.Providers;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ReconNess.PubSub
{
    public class QueueAgentRunnerProvider : IAgentRunnerProvider
    {
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        ConnectionFactory _factory;
        IConnection _conn;
        IModel _channel;

        public QueueAgentRunnerProvider()
        {
            _factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672 };
            _factory.UserName = "guest";
            _factory.Password = "guest";
            _conn = _factory.CreateConnection();
            _channel = _conn.CreateModel();
            _channel.QueueDeclare(queue: "hello",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
        }

        public Task<int> RunningCountAsync => Task.FromResult(0);

        public Task<IList<string>> RunningChannelsAsync => Task.FromResult((IList<string>)new List<string>());

        public Task InitializesAsync(string channel)
        {
            return Task.CompletedTask;
        }

        public Task<bool> IsStoppedAsync(string channel)
        {
            return Task.FromResult(true);
        }

        public Task RunAsync(AgentRunnerProviderArgs providerArgs)
        {
            _logger.Info($"Sending command to rabbitmq: {providerArgs.Command}");

            var body = Encoding.UTF8.GetBytes("server command: " + providerArgs.Command);
            _channel.BasicPublish(exchange: "",
                                routingKey: "hello",
                                basicProperties: null,
                                body: body);

            return Task.CompletedTask;
        }

        public Task StopAsync(string channel)
        {
            return Task.CompletedTask;
        }
    }
}
