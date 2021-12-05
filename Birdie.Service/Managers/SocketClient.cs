using Birdie.Service.Models;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Birdie.Service.Managers
{
    public class SocketClient
    {
        public string Id { get; set; }

        public WebSocket Socket { get; set; }

        public string RoomIdentifier { get; set; }

        public Task SendMessageAsync(SocketMessageModel message)
        {
            var messageContent = Util.GetJsonBytes((SimpleSocketMessageModel)message);

            return Socket.SendAsync(new ArraySegment<byte>(messageContent, 0, messageContent.Length), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
