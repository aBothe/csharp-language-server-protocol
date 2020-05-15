using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Restart)]
    public interface IRestartHandler : IJsonRpcRequestHandler<RestartArguments, RestartResponse>
    {
    }

    public abstract class RestartHandler : IRestartHandler
    {
        public abstract Task<RestartResponse> Handle(RestartArguments request, CancellationToken cancellationToken);
    }

    public static class RestartHandlerExtensions
    {
        public static IDisposable OnRestart(this IDebugAdapterRegistry registry,
            Func<RestartArguments, CancellationToken, Task<RestartResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Restart, RequestHandler.For(handler));
        }

        public static IDisposable OnRestart(this IDebugAdapterRegistry registry,
            Func<RestartArguments, Task<RestartResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Restart, RequestHandler.For(handler));
        }
    }
}
