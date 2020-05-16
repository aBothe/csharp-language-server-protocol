using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.StackTrace)]
    public interface IStackTraceHandler : IJsonRpcRequestHandler<StackTraceArguments, StackTraceResponse>
    {
    }

    public abstract class StackTraceHandler : IStackTraceHandler
    {
        public abstract Task<StackTraceResponse> Handle(StackTraceArguments request,
            CancellationToken cancellationToken);
    }

    public static class StackTraceExtensions
    {
        public static IDisposable OnStackTrace(this IDebugAdapterRegistry registry,
            Func<StackTraceArguments, CancellationToken, Task<StackTraceResponse>> handler)
        {
            return registry.AddHandler(RequestNames.StackTrace, RequestHandler.For(handler));
        }

        public static IDisposable OnStackTrace(this IDebugAdapterRegistry registry,
            Func<StackTraceArguments, Task<StackTraceResponse>> handler)
        {
            return registry.AddHandler(RequestNames.StackTrace, RequestHandler.For(handler));
        }
    }
}
