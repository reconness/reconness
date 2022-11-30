using NLog;
using ReconNess.Application.Providers;
using ReconNess.Domain.Entities;
using RestSharp;
using RestSharp.Serializers;

namespace ReconNess.Infrastructure.Providers;

/// <summary>
/// Implement the interface <see cref="INotificationProvider"/>
/// </summary>
public class NotificationProvider : INotificationProvider
{
    protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    /// <inheritdoc/>
    public async Task SendNotificationAsync(Notification notification, string payload, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = new RestClient(notification.Url);
            var request = new RestRequest();

            payload = notification.Payload.Replace("{{notification}}", payload);
            request.AddStringBody(payload, ContentType.Json);

            var method = "POST".Equals(notification.Method) ? Method.Post : Method.Get;
            var response = await client.ExecuteAsync(request, method, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, ex.Message);
        }
    }
}
