using System.Threading;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public static class WorkspaceFoldersExtensions
    {
        public static Task<Container<WorkspaceFolder>> WorkspaceFolders(this IWorkspaceLanguageServer mediator, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(new WorkspaceFolderParams(), cancellationToken);
        }
    }
}

