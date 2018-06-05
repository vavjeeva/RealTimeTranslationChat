using Microsoft.AspNetCore.SignalR;
using RealTimeTranslationChat.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealTimeTranslationChat.Hubs
{
    public class ChatHub : Hub
    {
        static List<User> ConnectedUsers = new List<User>();

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public async Task Connect(string name, string language)
        {
            var id = Context.ConnectionId;
            if (ConnectedUsers.Count(x => x.ConnectionId == id) == 0)
            {
                ConnectedUsers.Add(new User() { Name = name, ConnectionId = id });

                await Groups.AddToGroupAsync(id, language);

                await Clients.Caller.SendAsync("onConnected", ConnectedUsers, name, id);

                await Clients.AllExcept(id).SendAsync("onNewUserConnected", name);
            }

        }

        public async Task Disconnect()
        {
            var item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                ConnectedUsers.Remove(item);
                await Clients.AllExcept(item.ConnectionId).SendAsync("onDisconnected", item.Name);
            }
        }
    }
}
