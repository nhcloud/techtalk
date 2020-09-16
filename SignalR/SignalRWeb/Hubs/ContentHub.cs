using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SignalRWeb.Models;

namespace SignalRWeb.Hubs
{
    public class ContentHub : Hub
    {
        public async Task GetItems()
        {
            await Clients.All.SendAsync("items", await Helper.GetDataAsync());
        }
        public override Task OnConnectedAsync()
        {
            return Clients.Caller.SendAsync("items",  Helper.GetDataAsync().Result);
        }
    }
}