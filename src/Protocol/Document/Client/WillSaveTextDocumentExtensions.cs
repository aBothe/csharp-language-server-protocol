using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Client
{
    public static class WillSaveTextDocumentExtensions
    {
        public static void WillSaveTextDocument(this ITextDocumentLanguageClient mediator, WillSaveTextDocumentParams @params)
        {
            mediator.SendNotification(TextDocumentNames.WillSave, @params);
        }
    }
}
