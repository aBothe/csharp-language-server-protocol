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
            Action<ShowMessageParams> handler)
        {
            return registry.AddHandler(WindowNames.ShowMessage, NotificationHandler.For(handler));
        }

        public static IDisposable OnShowMessage(
            this ILanguageServerRegistry registry,
            Func<ShowMessageParams, Task> handler)
        {
            return registry.AddHandler(WindowNames.ShowMessage, NotificationHandler.For(handler));
        }
    }
}
