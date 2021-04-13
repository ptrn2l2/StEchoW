using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace StEchoW.Web.Hubs
{
    // Same as SignalR Getting Started: https://docs.microsoft.com/en-us/aspnet/core/tutorials/signalr?view=aspnetcore-5.0&tabs=visual-studio
    public class ChatHub : Hub
    {
        public const string JsReceiveMessageFuncName = "ReceiveMessage";

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync(JsReceiveMessageFuncName, user, message);
        }
    }
}