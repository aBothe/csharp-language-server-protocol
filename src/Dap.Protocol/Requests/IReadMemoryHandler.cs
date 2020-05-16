using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.ReadMemory)]
    public interface IReadMemoryHandler : IJsonRpcRequestHandler<ReadMemoryArguments, ReadMemoryResponse>
    {
    }

    public abstract class ReadMemoryHandler : IReadMemoryHandler
    {
        public abstract Task<ReadMemoryResponse> Handle(ReadMemoryArguments request,
            CancellationToken cancellationToken);
    }

    public static class ReadMemoryExtensions
    {
        public static IDisposable OnReadMemory(this IDebugAdapterRegistry registry,
            Func<ReadMemoryArguments, CancellationToken, Task<ReadMemoryResponse>> handler)
        {
            return registry.AddHandler(RequestNames.ReadMemory, RequestHandler.For(handler));
        }

        public static IDisposable OnReadMemory(this IDebugAdapterRegistry registry,
            Func<ReadMemoryArguments, Task<ReadMemoryResponse>> handler)
        {
            return registry.AddHandler(RequestNames.ReadMemory, RequestHandler.For(handler));
        }
    }
}
