using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.TerminateThreads, Direction.ClientToServer)]
    public interface
        ITerminateThreadsHandler : IJsonRpcRequestHandler<TerminateThreadsArguments, TerminateThreadsResponse>
    {
    }

    public abstract class TerminateThreadsHandler : ITerminateThreadsHandler
    {
        public abstract Task<TerminateThreadsResponse> Handle(TerminateThreadsArguments request,
            CancellationToken cancellationToken);
    }

    public static class TerminateThreadsExtensions
    {
        public static IDisposable OnTerminateThreads(this IDebugAdapterRegistry registry,
            Func<TerminateThreadsArguments, CancellationToken, Task<TerminateThreadsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.TerminateThreads, RequestHandler.For(handler));
        }

        public static IDisposable OnTerminateThreads(this IDebugAdapterRegistry registry,
            Func<TerminateThreadsArguments, Task<TerminateThreadsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.TerminateThreads, RequestHandler.For(handler));
        }
    }
}
