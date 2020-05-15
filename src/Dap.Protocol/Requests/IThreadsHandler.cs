using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Threads)]
    public interface IThreadsHandler : IJsonRpcRequestHandler<ThreadsArguments, ThreadsResponse>
    {
    }

    public abstract class ThreadsHandler : IThreadsHandler
    {
        public abstract Task<ThreadsResponse> Handle(ThreadsArguments request, CancellationToken cancellationToken);
    }

    public static class ThreadsHandlerExtensions
    {
        public static IDisposable OnThreads(this IDebugAdapterRegistry registry,
            Func<ThreadsArguments, CancellationToken, Task<ThreadsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Threads, RequestHandler.For(handler));
        }

        public static IDisposable OnThreads(this IDebugAdapterRegistry registry,
            Func<ThreadsArguments, Task<ThreadsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Threads, RequestHandler.For(handler));
        }
    }
}
