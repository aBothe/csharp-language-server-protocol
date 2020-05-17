using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Launch, Direction.ClientToServer)]
    public interface ILaunchHandler : IJsonRpcRequestHandler<LaunchRequestArguments, LaunchResponse>
    {
    }

    public abstract class LaunchHandler : ILaunchHandler
    {
        public abstract Task<LaunchResponse>
            Handle(LaunchRequestArguments request, CancellationToken cancellationToken);
    }

    public static class LaunchExtensions
    {
        public static IDisposable OnLaunch(this IDebugAdapterRegistry registry,
            Func<LaunchRequestArguments, CancellationToken, Task<LaunchResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Launch, RequestHandler.For(handler));
        }

        public static IDisposable OnLaunch(this IDebugAdapterRegistry registry,
            Func<LaunchRequestArguments, Task<LaunchResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Launch, RequestHandler.For(handler));
        }
    }
}
