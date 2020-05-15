using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.RestartFrame)]
    public interface IRestartFrameHandler : IJsonRpcRequestHandler<RestartFrameArguments, RestartFrameResponse>
    {
    }

    public abstract class RestartFrameHandler : IRestartFrameHandler
    {
        public abstract Task<RestartFrameResponse> Handle(RestartFrameArguments request,
            CancellationToken cancellationToken);
    }

    public static class RestartFrameHandlerExtensions
    {
        public static IDisposable OnRestartFrame(this IDebugAdapterRegistry registry,
            Func<RestartFrameArguments, CancellationToken, Task<RestartFrameResponse>> handler)
        {
            return registry.AddHandler(RequestNames.RestartFrame, RequestHandler.For(handler));
        }

        public static IDisposable OnRestartFrame(this IDebugAdapterRegistry registry,
            Func<RestartFrameArguments, Task<RestartFrameResponse>> handler)
        {
            return registry.AddHandler(RequestNames.RestartFrame, RequestHandler.For(handler));
        }
    }
}
