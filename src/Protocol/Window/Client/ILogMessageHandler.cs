using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Client
{
    [Parallel, Method(WindowNames.LogMessage)]
    public interface ILogMessageHandler : IJsonRpcNotificationHandler<LogMessageParams> { }

    public abstract class LogMessageHandler : ILogMessageHandler
    {
        public abstract Task<Unit> Handle(LogMessageParams request, CancellationToken cancellationToken);
    }

    public static class LogMessageHandlerExtensions
    {
        public static IDisposable OnLogMessage(
            this ILanguageServerRegistry registry,
            Action<LogMessageParams> handler)
        {
            return registry.AddHandler(WindowNames.LogMessage, NotificationHandler.For(handler));
        }

        public static IDisposable OnLogMessage(
            this ILanguageServerRegistry registry,
            Func<LogMessageParams, Task> handler)
        {
            return registry.AddHandler(WindowNames.LogMessage, NotificationHandler.For(handler));
        }
    }
}
