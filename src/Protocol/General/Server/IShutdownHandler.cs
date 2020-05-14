using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Server
{
    [Serial, Method(GeneralNames.Shutdown)]
    public interface IShutdownHandler : IJsonRpcRequestHandler<EmptyRequest> { }

    public abstract class ShutdownHandler : IShutdownHandler
    {
        public abstract Task<Unit> Handle(EmptyRequest request, CancellationToken cancellationToken);
    }

    public static class ShutdownHandlerExtensions
    {
        public static IDisposable OnShutdown(this ILanguageServerRegistry registry, Func<Task<Unit>> handler)
        {
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler));
        }

        class DelegatingHandler : ShutdownHandler
        {
            private readonly Func<Task<Unit>> _handler;

            public DelegatingHandler(Func<Task<Unit>> handler)
            {
                _handler = handler;
            }

            public override Task<Unit> Handle(EmptyRequest request, CancellationToken cancellationToken) => _handler.Invoke();
        }
    }
}
