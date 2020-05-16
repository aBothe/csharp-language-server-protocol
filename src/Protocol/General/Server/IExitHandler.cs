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
    [Serial, Method(GeneralNames.Exit)]
    public interface IExitHandler : IJsonRpcNotificationHandler
    {
    }

    public abstract class ExitHandler : IExitHandler
    {
        public virtual async Task<Unit> Handle(EmptyRequest request, CancellationToken cancellationToken)
        {
            await Handle(cancellationToken);
            return Unit.Value;
        }

        protected abstract Task Handle(CancellationToken cancellationToken);
    }

    public static class ExitExtensions
    {
        public static IDisposable OnExit(this ILanguageServerRegistry registry, Action handler)
        {
            return registry.AddHandler(GeneralNames.Exit,
                NotificationHandler.For<EmptyRequest>(_ => handler()));
        }

        public static IDisposable OnExit(this ILanguageServerRegistry registry, Func<Task> handler)
        {
            return registry.AddHandler(GeneralNames.Exit,
                NotificationHandler.For<EmptyRequest>(_ => handler()));
        }

        public static IDisposable OnExit(this ILanguageServerRegistry registry, Action<CancellationToken> handler)
        {
            return registry.AddHandler(GeneralNames.Exit,
                NotificationHandler.For<EmptyRequest>((_, ct) => handler(ct)));
        }

        public static IDisposable OnExit(this ILanguageServerRegistry registry, Func<CancellationToken, Task> handler)
        {
            return registry.AddHandler(GeneralNames.Exit,
                NotificationHandler.For<EmptyRequest>((_, ct) => handler(ct)));
        }
    }
}
