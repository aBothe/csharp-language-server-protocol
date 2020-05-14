using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Pause)]
    public interface IPauseHandler : IJsonRpcRequestHandler<PauseArguments, PauseResponse> { }

    public abstract class PauseHandler : IPauseHandler
    {
        public abstract Task<PauseResponse> Handle(PauseArguments request, CancellationToken cancellationToken);
    }

    public static class PauseHandlerExtensions
    {
        public static IDisposable OnPause(this IDebugAdapterRegistry registry, Func<PauseArguments, CancellationToken, Task<PauseResponse>> handler)
        {
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler));
        }

        class DelegatingHandler : PauseHandler
        {
            private readonly Func<PauseArguments, CancellationToken, Task<PauseResponse>> _handler;

            public DelegatingHandler(Func<PauseArguments, CancellationToken, Task<PauseResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<PauseResponse> Handle(PauseArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
