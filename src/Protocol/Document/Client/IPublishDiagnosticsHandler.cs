using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Client
{

    [Parallel, Method(TextDocumentNames.PublishDiagnostics)]
    public interface IPublishDiagnosticsHandler : IJsonRpcNotificationHandler<PublishDiagnosticsParams> { }

    public abstract class PublishDiagnosticsHandler : IPublishDiagnosticsHandler
    {
        public abstract Task<Unit> Handle(PublishDiagnosticsParams request, CancellationToken cancellationToken);
    }

    public static class PublishDiagnosticsExtensions
    {
        public static IDisposable OnPublishDiagnostics(this ILanguageClientRegistry registry,
            Func<PublishDiagnosticsParams, CancellationToken, Task> handler)
        {
            return registry.AddHandler(TextDocumentNames.PublishDiagnostics, RequestHandler.For(handler));
        }

        public static IDisposable OnPublishDiagnostics(this ILanguageClientRegistry registry,
            Func<PublishDiagnosticsParams, Task> handler)
        {
            return registry.AddHandler(TextDocumentNames.PublishDiagnostics, RequestHandler.For(handler));
        }
    }
}
