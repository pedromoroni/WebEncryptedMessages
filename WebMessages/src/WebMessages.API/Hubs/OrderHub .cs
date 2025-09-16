using Microsoft.AspNetCore.SignalR;
using WebMessages.Models.Entities;

namespace WebMessages.API.Hubs
{
    public sealed class OrderHub : Hub
    {
        public async Task SubscribeToOrder(string orderId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, orderId);
        }

        public async Task UnsubscribeFromOrder(string orderId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, orderId);
        }
    }
}
