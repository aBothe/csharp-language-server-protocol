using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Client
{
    public static class DidChangeWatchedFilesExtensions
    {
        public static void DidChangeWatchedFiles(this IWorkspaceLanguageClient router, DidChangeWatchedFilesParams @params)
        {
            router.SendNotification(WorkspaceNames.DidChangeWatchedFiles, @params);
        }
    }
}
