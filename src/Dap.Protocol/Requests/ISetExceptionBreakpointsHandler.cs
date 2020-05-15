using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.SetExceptionBreakpoints)]
    public interface ISetExceptionBreakpointsHandler : IJsonRpcRequestHandler<SetExceptionBreakpointsArguments,
        SetExceptionBreakpointsResponse>
    {
    }

    public abstract class SetExceptionBreakpointsHandler : ISetExceptionBreakpointsHandler
    {
        public abstract Task<SetExceptionBreakpointsResponse> Handle(SetExceptionBreakpointsArguments request,
            CancellationToken cancellationToken);
    }

    public static class SetExceptionBreakpointsHandlerExtensions
    {
        public static IDisposable OnSetExceptionBreakpoints(this IDebugAdapterRegistry registry,
            Func<SetExceptionBreakpointsArguments, CancellationToken, Task<SetExceptionBreakpointsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.SetExceptionBreakpoints, RequestHandler.For(handler));
        }

        public static IDisposable OnSetExceptionBreakpoints(this IDebugAdapterRegistry registry,
            Func<SetExceptionBreakpointsArguments, Task<SetExceptionBreakpointsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.SetExceptionBreakpoints, RequestHandler.For(handler));
        }
    }
}
