using Birdie.Service.Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Birdie.API.Middlewares
{
    public class SocketMiddleware
    {
        private readonly RequestDelegate requestDelegate;
        private readonly ISocketManager socketManager;
        private readonly ILogger<SocketMiddleware> logger;

        public SocketMiddleware(RequestDelegate requestDelegate, ISocketManager socketManager, ILogger<SocketMiddleware> logger)
        {
            this.requestDelegate = requestDelegate;
            this.socketManager = socketManager;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var webSocket = await context.WebSockets.AcceptWebSocketAsync();

                    try
                    {
                        await socketManager.HandleClient(GetNewSocketClient(webSocket));
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.Message);
                        logger.LogError(ex.StackTrace);
                    }
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            else
            {
                await requestDelegate(context);
            }
        }

        public SocketClient GetNewSocketClient(WebSocket webSocket)
        {
            string socketId = Guid.NewGuid().ToString();

            return new SocketClient
            {
                Id = socketId,
                Socket = webSocket
            };
        }
    }
}
