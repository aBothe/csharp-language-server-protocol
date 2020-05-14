using System.Threading;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Client
{
    public static class CodeActionExtensions
    {
        public static Task<CommandOrCodeActionContainer> CodeAction(this ITextDocumentLanguageClient mediator, CodeActionParams @params, CancellationToken cancellationToken )
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
