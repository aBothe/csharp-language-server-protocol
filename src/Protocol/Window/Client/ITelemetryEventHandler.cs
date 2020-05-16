using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Client
{
    [Parallel, Method(WindowNames.TelemetryEvent)]
    public interface ITelemetryEventHandler : IJsonRpcNotificationHandler<TelemetryEventParams> { }

    public abstract class TelemetryEventHandler : ITelemetryEventHandler
    {
        public abstract Task<Unit> Handle(TelemetryEventParams request, CancellationToken cancellationToken);
    }

    public static class TelemetryEventExtensions
    {
        public static IDisposable OnTelemetryEvent(
            this ILanguageClientRegistry registry,
            Action<TelemetryEventParams> handler)
        {
            return registry.AddHandler(WindowNames.TelemetryEvent, NotificationHandler.For(handler));
        }

        public static IDisposable OnTelemetryEvent(
            this ILanguageClientRegistry registry,
            Action<TelemetryEventParams, CancellationToken> handler)
        {
            return registry.AddHandler(WindowNames.TelemetryEvent, NotificationHandler.For(handler));
        }

        public static IDisposable OnTelemetryEvent(
            this ILanguageClientRegistry registry,
            Func<TelemetryEventParams, Task> handler)
        {
            return registry.AddHandler(WindowNames.TelemetryEvent, NotificationHandler.For(handler));
        }

        public static IDisposable OnTelemetryEvent(
            this ILanguageClientRegistry registry,
            Func<TelemetryEventParams, CancellationToken, Task> handler)
        {
            return registry.AddHandler(WindowNames.TelemetryEvent, NotificationHandler.For(handler));
        }
    }
}
