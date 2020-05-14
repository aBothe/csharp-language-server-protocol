using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Client
{
    public static class DidChangeWorkspaceFoldersExtensions
    {
        public static void DidChangeWorkspaceFolders(this IWorkspaceLanguageClient router, DidChangeWorkspaceFoldersParams @params)
        {
            router.SendNotification(WorkspaceNames.DidChangeWorkspaceFolders, @params);
        }
    }
}
