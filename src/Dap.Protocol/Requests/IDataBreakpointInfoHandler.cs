using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.DataBreakpointInfo)]
    public interface
        IDataBreakpointInfoHandler : IJsonRpcRequestHandler<DataBreakpointInfoArguments, DataBreakpointInfoResponse>
    {
    }

    public abstract class DataBreakpointInfoHandler : IDataBreakpointInfoHandler
    {
        public abstract Task<DataBreakpointInfoResponse> Handle(DataBreakpointInfoArguments request,
            CancellationToken cancellationToken);
    }

    public static class DataBreakpointInfoExtensions
    {
        public static IDisposable OnDataBreakpointInfo(this IDebugAdapterRegistry registry,
            Func<DataBreakpointInfoArguments, CancellationToken, Task<DataBreakpointInfoResponse>> handler)
        {
            return registry.AddHandler(RequestNames.DataBreakpointInfo, RequestHandler.For(handler));
        }

        public static IDisposable OnDataBreakpointInfo(this IDebugAdapterRegistry registry,
            Func<DataBreakpointInfoArguments, Task<DataBreakpointInfoResponse>> handler)
        {
            return registry.AddHandler(RequestNames.DataBreakpointInfo, RequestHandler.For(handler));
        }
    }
}
