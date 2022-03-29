﻿using Newtonsoft.Json;
using NLog;
using ReconNess.Core;
using ReconNess.Core.Models;
using ReconNess.Core.Services;
using ReconNess.Entities;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Services
{
    /// <summary>
    /// This class implement <see cref="INotificationService"/>
    /// </summary>
    public class NotificationService : Service<Notification>, IService<Notification>, INotificationService
    {
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="INotificationService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        public NotificationService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        /// <inheritdoc/>
        public async Task SaveNotificationAsync(Notification newNotification, CancellationToken cancellationToken)
        {
            var notification = await GetByCriteriaAsync(n => !n.Deleted, cancellationToken);
            if (notification == null)
            {
                await AddAsync(newNotification, cancellationToken);
                return;
            }

            notification.Url = newNotification.Url;
            notification.Method = newNotification.Method;
            notification.Payload = newNotification.Payload;
            notification.RootDomainPayload = newNotification.RootDomainPayload;
            notification.SubdomainPayload = newNotification.SubdomainPayload;
            notification.IpAddressPayload = newNotification.IpAddressPayload;
            notification.IsAlivePayload = newNotification.IsAlivePayload;
            notification.HasHttpOpenPayload = newNotification.HasHttpOpenPayload;
            notification.ServicePayload = newNotification.ServicePayload;
            notification.DirectoryPayload = newNotification.DirectoryPayload;
            notification.TakeoverPayload = newNotification.TakeoverPayload;
            notification.ScreenshotPayload = newNotification.ScreenshotPayload;
            notification.NotePayload = newNotification.NotePayload;
            notification.TechnologyPayload = newNotification.TechnologyPayload;

            await UpdateAsync(notification, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task SendAsync(string agentPayload, CancellationToken cancellationToken)
        {
            var notification = await GetByCriteriaAsync(n => !n.Deleted, cancellationToken);
            if (notification == null)
            {
                return;
            }

            try
            {
                var client = new RestClient(notification.Url);
                var request = new RestRequest();

                var payload = notification.Payload.Replace("{{notification}}", agentPayload);
                request.AddJsonBody(JsonConvert.DeserializeObject(payload));

                var method = "POST".Equals(notification.Method) ? Method.Post : Method.Get;
                var response = await client.ExecuteAsync(request, method, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
            }
        }

        /// <inheritdoc/>
        public async Task SendAsync(NotificationType notificationType, (string, string)[] replaces, CancellationToken cancellationToken = default)
        {
            var payload = await GetPayloadAsync(notificationType, cancellationToken);
            if (string.IsNullOrEmpty(payload))
            {
                return;
            }

            foreach (var replace in replaces)
            {
                cancellationToken.ThrowIfCancellationRequested();
                payload = payload.Replace(replace.Item1, replace.Item2);
            }

            await SendAsync(payload, cancellationToken);
        }

        /// <summary>
        /// Obtain the payload
        /// </summary>
        /// <param name="notificationType">Notification type</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The payload</returns>
        private async Task<string> GetPayloadAsync(NotificationType notificationType, CancellationToken cancellationToken)
        {
            var notification = await GetByCriteriaAsync(n => !n.Deleted, cancellationToken);

            return notificationType switch
            {
                NotificationType.SUBDOMAIN => notification.SubdomainPayload,
                NotificationType.IP => notification.IpAddressPayload,
                NotificationType.IS_ALIVE => notification.IsAlivePayload,
                NotificationType.HAS_HTTP_OPEN => notification.HasHttpOpenPayload,
                NotificationType.SERVICE => notification.ServicePayload,
                NotificationType.TAKEOVER => notification.TakeoverPayload,
                NotificationType.DIRECTORY => notification.DirectoryPayload,
                NotificationType.SCREENSHOT => notification.ScreenshotPayload,
                NotificationType.TECHNOLOGY => notification.TechnologyPayload,
                _ => string.Empty,
            };
        }
    }
}
