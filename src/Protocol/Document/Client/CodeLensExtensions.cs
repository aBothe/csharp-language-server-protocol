using System.Threading;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Client
{
    public static class CodeLensExtensions
    {
        public static Task<CodeLensContainer> CodeLens(this ITextDocumentLanguageClient mediator, CodeLensParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
