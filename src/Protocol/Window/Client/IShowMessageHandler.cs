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
    [Parallel, Method(WindowNames.ShowMessage)]
    public interface IShowMessageHandler : IJsonRpcNotificationHandler<ShowMessageParams> { }

    public abstract class ShowMessageHandler : IShowMessageHandler
    {
        public abstract Task<Unit> Handle(ShowMessageParams request, CancellationToken cancellationToken);
    }

    public static class ShowMessageHandlerExtensions
    {
        public static IDisposable OnShowMessage(
            this ILanguageServerRegistry registry,
            Func<ShowMessageParams, CancellationToken, Task<Unit>> handler)
        {
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler));
        }

        class DelegatingHandler : ShowMessageHandler
        {
            private readonly Func<ShowMessageParams, CancellationToken, Task<Unit>> _handler;

            public DelegatingHandler(Func<ShowMessageParams, CancellationToken, Task<Unit>> handler)
            {
                _handler = handler;
            }

            public override Task<Unit> Handle(ShowMessageParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
