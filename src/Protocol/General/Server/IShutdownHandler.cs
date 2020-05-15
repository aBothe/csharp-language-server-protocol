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
    public interface IShutdownHandler : IJsonRpcRequestHandler<EmptyRequest>
    {
    }

    public abstract class ShutdownHandler : IShutdownHandler
    {
        public virtual async Task<Unit> Handle(EmptyRequest request, CancellationToken cancellationToken)
        {
            await Handle(cancellationToken);
            return Unit.Value;
        }

        protected abstract Task Handle(CancellationToken cancellationToken);
    }

    public static class ShutdownHandlerExtensions
    {
        public static IDisposable OnShutdown(
            this ILanguageServerRegistry registry,
            Func<CancellationToken, Task>
                handler)
        {
            return registry.AddHandler(GeneralNames.Shutdown,
                RequestHandler.For<EmptyRequest, Unit>(async (_, ct) => {
                    await handler(ct);
                    return Unit.Value;
                }));
        }

        public static IDisposable OnShutdown(
            this ILanguageServerRegistry registry,
            Func<Task>
                handler)
        {
            return registry.AddHandler(GeneralNames.Shutdown,
                RequestHandler.For<EmptyRequest, Unit>(async (_, ct) => {
                    await handler();
                    return Unit.Value;
                }));
        }
    }
}
