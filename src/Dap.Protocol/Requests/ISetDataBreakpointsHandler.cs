using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.SetDataBreakpoints)]
    public interface
        ISetDataBreakpointsHandler : IJsonRpcRequestHandler<SetDataBreakpointsArguments, SetDataBreakpointsResponse>
    {
    }

    public abstract class SetDataBreakpointsHandler : ISetDataBreakpointsHandler
    {
        public abstract Task<SetDataBreakpointsResponse> Handle(SetDataBreakpointsArguments request,
            CancellationToken cancellationToken);
    }

    public static class SetDataBreakpointsHandlerExtensions
    {
        public static IDisposable OnSetDataBreakpoints(this IDebugAdapterRegistry registry,
            Func<SetDataBreakpointsArguments, CancellationToken, Task<SetDataBreakpointsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.SetDataBreakpoints, RequestHandler.For(handler));
        }

        public static IDisposable OnSetDataBreakpoints(this IDebugAdapterRegistry registry,
            Func<SetDataBreakpointsArguments, Task<SetDataBreakpointsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.SetDataBreakpoints, RequestHandler.For(handler));
        }
    }
}
