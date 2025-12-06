using OrderService.Application.Common.Interfaces;
using OrderService.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Infrastructure.ExternalServices
{
    public class NotificationClient : INotificationClient
    {
        private readonly HttpClient _httpClient;
        public NotificationClient(HttpClient httpClient) => _httpClient = httpClient;

        public async Task SendNotificationAsync(Order order)
        {
            // In real life: await _httpClient.PostAsJsonAsync("/notify", order);
            await Task.CompletedTask;
        }
    }
}
