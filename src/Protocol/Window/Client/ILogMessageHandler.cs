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
            Func<LogMessageParams, CancellationToken, Task<Unit>> handler)
        {
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler));
        }

        class DelegatingHandler : LogMessageHandler
        {
            private readonly Func<LogMessageParams, CancellationToken, Task<Unit>> _handler;

            public DelegatingHandler(Func<LogMessageParams, CancellationToken, Task<Unit>> handler)
            {
                _handler = handler;
            }

            public override Task<Unit> Handle(LogMessageParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
