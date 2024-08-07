using System.Net;
using System.Net.Sockets;
using GameFrameX.SuperSocket.Primitives;
using GameFrameX.SuperSocket.Server.Abstractions;
using GameFrameX.SuperSocket.Server.Abstractions.Connections;
using GameFrameX.SuperSocket.Server.Abstractions.Session;
using GameFrameX.SuperSocket.Server.Host;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GameFrameX.SuperSocket.Server
{
    public static class SuperSocketExtensions
    {
        public static async ValueTask<bool> ActiveConnect(this IServer server, EndPoint remoteEndpoint)
        {
            var socket = new Socket(remoteEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                await socket.ConnectAsync(remoteEndpoint);
                await (server as IConnectionRegister).RegisterConnection(socket);
                return true;
            }
            catch (Exception e)
            {
                var loggerFactory = server.ServiceProvider.GetService<ILoggerFactory>();

                if (loggerFactory != null)
                    loggerFactory.CreateLogger(nameof(ActiveConnect)).LogError(e, $"Failed to connect to {remoteEndpoint}");

                return false;
            }
        }

        public static ILogger GetDefaultLogger(this IAppSession session)
        {
            return (session.Server as ILoggerAccessor)?.Logger;
        }

        public static ServerOptions AddListener(this ServerOptions serverOptions, ListenOptions listener)
        {
            var listeners = serverOptions.Listeners;

            if (listeners == null)
                listeners = serverOptions.Listeners = new List<ListenOptions>();

            listeners.Add(listener);
            return serverOptions;
        }

        public static void AdaptMultipleServerHost(this IHost host)
        {
            var services = host.Services;
            services.GetService<MultipleServerHostBuilder>()?.AdaptMultipleServerHost(services);
        }
    }
}