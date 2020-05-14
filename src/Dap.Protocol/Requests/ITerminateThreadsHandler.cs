using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.TerminateThreads)]
    public interface ITerminateThreadsHandler : IJsonRpcRequestHandler<TerminateThreadsArguments, TerminateThreadsResponse> { }

    public abstract class TerminateThreadsHandler : ITerminateThreadsHandler
    {
        public abstract Task<TerminateThreadsResponse> Handle(TerminateThreadsArguments request, CancellationToken cancellationToken);
    }

    public static class TerminateThreadsHandlerExtensions
    {
        public static IDisposable OnTerminateThreads(this IDebugAdapterRegistry registry, Func<TerminateThreadsArguments, CancellationToken, Task<TerminateThreadsResponse>> handler)
        {
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler));
        }

        class DelegatingHandler : TerminateThreadsHandler
        {
            private readonly Func<TerminateThreadsArguments, CancellationToken, Task<TerminateThreadsResponse>> _handler;

            public DelegatingHandler(Func<TerminateThreadsArguments, CancellationToken, Task<TerminateThreadsResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<TerminateThreadsResponse> Handle(TerminateThreadsArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
