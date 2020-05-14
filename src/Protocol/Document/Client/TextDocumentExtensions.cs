using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Client
{
    public static class TextDocumentExtensions
    {
        public static void DidChangeTextDocument(this ITextDocumentLanguageClient mediator, DidChangeTextDocumentParams @params)
        {
            mediator.SendNotification(TextDocumentNames.DidChange, @params);
        }

        public static void DidOpenTextDocument(this ITextDocumentLanguageClient mediator, DidOpenTextDocumentParams @params)
        {
            mediator.SendNotification(TextDocumentNames.DidOpen, @params);
        }

        public static void DidSaveTextDocument(this ITextDocumentLanguageClient mediator, DidSaveTextDocumentParams @params)
        {
            mediator.SendNotification(TextDocumentNames.DidSave, @params);
        }

        public static void DidCloseTextDocument(this ITextDocumentLanguageClient mediator, DidCloseTextDocumentParams @params)
        {
            mediator.SendNotification(TextDocumentNames.DidClose, @params);
        }
    }
}
