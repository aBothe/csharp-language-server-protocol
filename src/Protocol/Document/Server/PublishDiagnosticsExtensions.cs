using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public static class PublishDiagnosticsExtensions
    {
        public static void PublishDiagnostics(this ITextDocumentLanguageServer mediator, PublishDiagnosticsParams @params)
        {
            mediator.SendNotification(TextDocumentNames.PublishDiagnostics, @params);
        }
    }
}
