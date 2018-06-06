using Microsoft.AspNetCore.SignalR;
using RealTimeTranslationChat.Helper;
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
        private CognitiveServiceClient _client;
        public ChatHub(CognitiveServiceClient client)
        {
            _client = client;
        }

        static List<User> ConnectedUsers = new List<User>();

        public User CurrentUser
        {
            get
            {
                return ConnectedUsers.FirstOrDefault(i => i.ConnectionId == Context.ConnectionId);
            }
        }

        public string LanguageFormatted
        {
            get
            {
                string result = "";
                var items = ConnectedUsers.Select(i => i.LanguagePreference).Distinct();
                foreach (var language in items)
                {
                    result += $"to={language}&";
                }

                if (result.Length > 1)
                    result = result.Substring(0, result.Length - 1);
                return result;
            }
        }

        public async Task SendMessage(string user, string message)
        {
            var results = _client.Translate(message, LanguageFormatted);
            var translationResult = results.Result.FirstOrDefault();
            if (translationResult != null)
            {
                foreach (var translation in translationResult.translations)
                {
                    await Clients.GroupExcept(translation.to, Context.ConnectionId).SendAsync("ReceiveMessage", user, translation.text);
                    //await Clients.AllExcept(Context.ConnectionId).SendAsync("ReceiveMessage", user, message);
                }
            }
        }

        public async Task Connect(string name, string language)
        {
            var id = Context.ConnectionId;
            if (ConnectedUsers.Count(x => x.ConnectionId == id) == 0)
            {
                ConnectedUsers.Add(new User() { Name = name, ConnectionId = id, LanguagePreference = language });

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
