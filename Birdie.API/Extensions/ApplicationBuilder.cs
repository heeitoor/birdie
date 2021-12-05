using Birdie.API.Middlewares;
using Microsoft.AspNetCore.Builder;
using System;

namespace Birdie.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseBirdieSocket(this IApplicationBuilder app)
        {
            WebSocketOptions webSocketOptions = new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromSeconds(60)
            };

            webSocketOptions.AllowedOrigins.Add("http://localhost:5000");

            app
                .UseWebSockets(webSocketOptions)
                .UseMiddleware<SocketMiddleware>();

            return app;
        }
    }
}