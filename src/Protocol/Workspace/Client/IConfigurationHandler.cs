using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Client
{
    [Parallel, Method(WorkspaceNames.WorkspaceConfiguration)]
    public interface IConfigurationHandler : IJsonRpcRequestHandler<ConfigurationParams, Container<JToken>> { }

    public abstract class ConfigurationHandler : IConfigurationHandler
    {
        public abstract Task<Container<JToken>> Handle(ConfigurationParams request, CancellationToken cancellationToken);
    }

    public static class ConfigurationHandlerExtensions
    {
        public static IDisposable OnConfiguration(
            this ILanguageClientRegistry registry,
            Func<ConfigurationParams, CancellationToken, Task<Container<JToken>>>
                handler)
        {
            return registry.AddHandler(WorkspaceNames.WorkspaceConfiguration, RequestHandler.For(handler));
        }

        public static IDisposable OnConfiguration(
            this ILanguageClientRegistry registry,
            Func<ConfigurationParams, Task<Container<JToken>>> handler)
        {
            return registry.AddHandler(WorkspaceNames.WorkspaceConfiguration, RequestHandler.For(handler));
        }
    }
}
