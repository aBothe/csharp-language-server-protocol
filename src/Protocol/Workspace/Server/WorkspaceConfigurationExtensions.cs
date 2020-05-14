using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Server
{
    public static class WorkspaceConfigurationExtensions
    {
        public static Task<Container<JToken>> WorkspaceConfiguration(this IWorkspaceLanguageServer router, ConfigurationParams @params, CancellationToken cancellationToken = default)
        {
            return router.SendRequest(@params, cancellationToken);
        }
    }
}
