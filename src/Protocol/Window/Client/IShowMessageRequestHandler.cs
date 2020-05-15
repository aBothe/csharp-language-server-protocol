using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Client
{
    [Serial, Method(WindowNames.ShowMessageRequest)]
    public interface IShowMessageRequestHandler : IJsonRpcRequestHandler<ShowMessageRequestParams, MessageActionItem> { }

    public abstract class ShowMessageRequestHandler : IShowMessageRequestHandler
    {
        public abstract Task<MessageActionItem> Handle(ShowMessageRequestParams request, CancellationToken cancellationToken);
    }

    public static class ShowMessageRequestHandlerExtensions
    {
        public static IDisposable OnShowMessageRequest(
            this ILanguageClientRegistry registry,
            Func<ShowMessageRequestParams, CancellationToken, Task<MessageActionItem>>
                handler)
        {
            return registry.AddHandler(WindowNames.ShowMessageRequest, RequestHandler.For(handler));
        }

        public static IDisposable OnShowMessageRequest(
            this ILanguageClientRegistry registry,
            Func<ShowMessageRequestParams, Task<MessageActionItem>> handler)
        {
            return registry.AddHandler(WindowNames.ShowMessageRequest, RequestHandler.For(handler));
        }
    }
}
