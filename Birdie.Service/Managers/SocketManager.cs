using Birdie.Service.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Birdie.Service.Managers
{
    public interface ISocketManager
    {
        Task HandleClient(SocketClient socketClient);
    }

    public class SocketManager : ISocketManager
    {
        private readonly IRabbitMQService rabbitMQService;
        private readonly ILogger<SocketManager> logger;
        private ConcurrentDictionary<string, SocketClient> sockets = new ConcurrentDictionary<string, SocketClient>();

        public SocketManager(IRabbitMQService rabbitMQService, ILogger<SocketManager> logger)
        {
            this.rabbitMQService = rabbitMQService;
            this.logger = logger;
        }

        public async Task HandleClient(SocketClient socketClient)
        {
            sockets.TryAdd(socketClient.Id, socketClient);
            logger.LogInformation($"Websocket client added.");

            WebSocketReceiveResult result = null;

            do
            {
                var buffer = new byte[1024 * 2];

                result = await socketClient.Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text && !result.CloseStatus.HasValue)
                {
                    var msgString = Encoding.UTF8.GetString(buffer);

                    logger.LogInformation($"Websocket client ReceiveAsync message {msgString}.");

                    SocketMessageModel message = JsonConvert.DeserializeObject<SocketMessageModel>(msgString);

                    message.ClientId = socketClient.Id;

                    MessageRoute(message);
                }
            } while (!result.CloseStatus.HasValue);

            sockets.Remove(socketClient.Id, out SocketClient removedClient);
        }

        private void MessageRoute(SocketMessageModel message)
        {
            SocketClient client = sockets[message.ClientId];

            switch (message.Type)
            {
                case SocketMessageType.Join:
                    client.RoomIdentifier = message.Content;
                    message.Content = $"{message.UserName} joined";
                    BroadcastToRoom(client.RoomIdentifier, message);
                    break;
                case SocketMessageType.Content:
                    if (string.IsNullOrEmpty(client.RoomIdentifier)) break;
                    rabbitMQService.PushToChatQueue(new ChatQueueModel
                    {
                        UserId = message.UserId,
                        Content = message.Content,
                        RoomIdentifier = client.RoomIdentifier,
                    });
                    BroadcastToRoom(client.RoomIdentifier, message);
                    break;
                case SocketMessageType.Bot:
                    rabbitMQService.PushToStockQueue(new StockQueueModel
                    {
                        CorrelationId = client.RoomIdentifier,
                        Search = message.Content,
                        UserId = message.UserId,
                    });
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void BroadcastToRoom(string roomIdentifier, SocketMessageModel message)
        {
            List<SocketClient> clientsByRoom = GetClientsByRoom(roomIdentifier);
            clientsByRoom.ForEach(client => client.SendMessageAsync(message));
        }

        private List<SocketClient> GetClientsByRoom(string roomIdentifier)
        {
            return sockets
                .Where(x => x.Value.RoomIdentifier == roomIdentifier)
                .Select(x => x.Value).ToList();
        }
    }
}
