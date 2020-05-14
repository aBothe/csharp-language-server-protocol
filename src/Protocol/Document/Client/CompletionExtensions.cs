using System.Threading;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Client
{
    public static class CompletionExtensions
    {
        public static Task<CompletionList> Completion(this ITextDocumentLanguageClient mediator, CompletionParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
