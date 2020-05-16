using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.SetFunctionBreakpoints)]
    public interface
        ISetFunctionBreakpointsHandler : IJsonRpcRequestHandler<SetFunctionBreakpointsArguments,
            SetFunctionBreakpointsResponse>
    {
    }

    public abstract class SetFunctionBreakpointsHandler : ISetFunctionBreakpointsHandler
    {
        public abstract Task<SetFunctionBreakpointsResponse> Handle(SetFunctionBreakpointsArguments request,
            CancellationToken cancellationToken);
    }

    public static class SetFunctionBreakpointsExtensions
    {
        public static IDisposable OnSetFunctionBreakpoints(this IDebugAdapterRegistry registry,
            Func<SetFunctionBreakpointsArguments, CancellationToken, Task<SetFunctionBreakpointsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.SetFunctionBreakpoints, RequestHandler.For(handler));
        }

        public static IDisposable OnSetFunctionBreakpoints(this IDebugAdapterRegistry registry,
            Func<SetFunctionBreakpointsArguments, Task<SetFunctionBreakpointsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.SetFunctionBreakpoints, RequestHandler.For(handler));
        }
    }
}
