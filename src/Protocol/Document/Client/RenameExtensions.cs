using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Client
{
    public static class RenameExtensions
    {
        public static Task<WorkspaceEdit> Rename(this ITextDocumentLanguageClient mediator, RenameParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }

        public static Task<RangeOrPlaceholderRange> PrepareRename(this ITextDocumentLanguageClient mediator, PrepareRenameParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
