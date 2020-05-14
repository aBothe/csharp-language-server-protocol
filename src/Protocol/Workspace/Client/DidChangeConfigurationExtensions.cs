using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Client
{
    public static class DidChangeConfigurationExtensions
    {
        public static void DidChangeConfiguration(this IWorkspaceLanguageClient router, DidChangeConfigurationParams @params)
        {
            router.SendNotification(WorkspaceNames.DidChangeConfiguration, @params);
        }
    }
}
