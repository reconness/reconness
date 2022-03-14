using Microsoft.Extensions.Configuration;
using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ReconNess.Core.Models;
using ReconNess.Core.Providers;
using ReconNess.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.PubSub
{
    public class QueueAgentRunnerProvider : IAgentRunnerProvider
    {
        private readonly IScriptEngineService scriptEngineService;

        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly ConnectionFactory _factory;
        private readonly IConnection _conn;
        private readonly IModel _channel;

        public QueueAgentRunnerProvider(IConfiguration configuration, IScriptEngineService scriptEngineService)
        {
            var rabbitmqConnectionString = configuration.GetConnectionString("DefaultRabbitmqConnection");

            var rabbitMQUserName = Environment.GetEnvironmentVariable("RabbitMQUser") ??
                                 Environment.GetEnvironmentVariable("RabbitMQUser", EnvironmentVariableTarget.User);
            var rabbitMQPassword = Environment.GetEnvironmentVariable("RabbitMQPassword") ??
                             Environment.GetEnvironmentVariable("RabbitMQPassword", EnvironmentVariableTarget.User);

            rabbitmqConnectionString = rabbitmqConnectionString.Replace("{{username}}", rabbitMQUserName)
                                                               .Replace("{{password}}", rabbitMQPassword);

            _factory = new ConnectionFactory() { Uri = new Uri(rabbitmqConnectionString) };
            _conn = _factory.CreateConnection();
            _channel = _conn.CreateModel();
            _channel.QueueDeclare(queue: "hello",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

            this.scriptEngineService = scriptEngineService;
        }

        public Task<int> RunningCountAsync => Task.FromResult(0);

        public Task<IList<string>> RunningChannelsAsync => Task.FromResult((IList<string>)new List<string>());

        public Task InitializesAsync(string channel)
        {
            return Task.CompletedTask;
        }

        public Task<bool> IsStoppedAsync(string channel)
        {
            return Task.FromResult(false);
        }

        public Task RunAsync(AgentRunnerProviderArgs providerArgs)
        {
            _logger.Info($"Sending command to rabbitmq: {providerArgs.Command}");

            var body = Encoding.UTF8.GetBytes("server command: " + providerArgs.Command);
            _channel.BasicPublish(exchange: "",
                                routingKey: "hello",
                                basicProperties: null,
                                body: body);

            var lineCount = 1;

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body;
                var terminalLineOutput = Encoding.UTF8.GetString(body.ToArray());

                var script = providerArgs.AgentRunner.Agent.Script;
                var scriptOutput = await scriptEngineService.TerminalOutputParseAsync(script, terminalLineOutput, lineCount++);

                await providerArgs.ParserOutputHandlerAsync(new AgentRunnerProviderResult
                {
                    AgentRunner = providerArgs.AgentRunner,
                    AgentRunnerType = providerArgs.AgentRunnerType,
                    Channel = providerArgs.Channel,
                    ScriptOutput = scriptOutput,
                    LineCount = lineCount,
                    TerminalLineOutput = terminalLineOutput,
                    CancellationToken = CancellationToken.None
                });

            };

            _channel.BasicConsume(queue: "hello",
                                    autoAck: true,
                                    consumer: consumer);

            return Task.CompletedTask;
        }

        public Task StopAsync(string channel)
        {
            return Task.CompletedTask;
        }
    }
}
