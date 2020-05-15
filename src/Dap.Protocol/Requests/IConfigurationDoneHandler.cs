using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.ConfigurationDone)]
    public interface
        IConfigurationDoneHandler : IJsonRpcRequestHandler<ConfigurationDoneArguments, ConfigurationDoneResponse>
    {
    }

    public abstract class ConfigurationDoneHandler : IConfigurationDoneHandler
    {
        public abstract Task<ConfigurationDoneResponse> Handle(ConfigurationDoneArguments request,
            CancellationToken cancellationToken);
    }

    public static class ConfigurationDoneHandlerExtensions
    {
        public static IDisposable OnConfigurationDone(this IDebugAdapterRegistry registry,
            Func<ConfigurationDoneArguments, CancellationToken, Task<ConfigurationDoneResponse>> handler)
        {
            return registry.AddHandler(RequestNames.ConfigurationDone, RequestHandler.For(handler));
        }

        public static IDisposable OnConfigurationDone(this IDebugAdapterRegistry registry,
            Func<ConfigurationDoneArguments, Task<ConfigurationDoneResponse>> handler)
        {
            return registry.AddHandler(RequestNames.ConfigurationDone, RequestHandler.For(handler));
        }
    }
}
