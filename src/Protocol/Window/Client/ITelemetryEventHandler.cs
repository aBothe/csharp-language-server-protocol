using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
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

    public static class TelemetryEventHandlerExtensions
    {
        public static IDisposable OnTelemetryEvent(
            this ILanguageServerRegistry registry,
            Func<TelemetryEventParams, CancellationToken, Task<Unit>> handler)
        {
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler));
        }

        class DelegatingHandler : TelemetryEventHandler
        {
            private readonly Func<TelemetryEventParams, CancellationToken, Task<Unit>> _handler;

            public DelegatingHandler(Func<TelemetryEventParams, CancellationToken, Task<Unit>> handler)
            {
                _handler = handler;
            }

            public override Task<Unit> Handle(TelemetryEventParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
