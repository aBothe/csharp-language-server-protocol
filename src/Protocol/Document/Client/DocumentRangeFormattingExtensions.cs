using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Client
{
    public static class DocumentRangeFormattingExtensions
    {
        public static Task<TextEditContainer> DocumentRangeFormatting(this ITextDocumentLanguageClient mediator, DocumentRangeFormattingParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params,  cancellationToken);
        }
    }
}
