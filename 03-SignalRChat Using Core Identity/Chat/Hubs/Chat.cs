using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Chat.Hubs
{
    public class Chat : Hub
    {
        public async Task SendMessage(string message)
        {
            // Context.User.Identity.Name gives the user name of the Identity based on the current user session.
            await Clients.All.SendAsync("ReceiveMessage", Context.User.Identity.Name ?? "anonymous", message);
        }
    }
}
