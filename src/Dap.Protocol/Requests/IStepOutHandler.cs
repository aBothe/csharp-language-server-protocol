using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.StepOut)]
    public interface IStepOutHandler : IJsonRpcRequestHandler<StepOutArguments, StepOutResponse> { }

    public abstract class StepOutHandler : IStepOutHandler
    {
        public abstract Task<StepOutResponse> Handle(StepOutArguments request, CancellationToken cancellationToken);
    }

    public static class StepOutHandlerExtensions
    {
        public static IDisposable OnStepOut(this IDebugAdapterRegistry registry, Func<StepOutArguments, CancellationToken, Task<StepOutResponse>> handler)
        {
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler));
        }

        class DelegatingHandler : StepOutHandler
        {
            private readonly Func<StepOutArguments, CancellationToken, Task<StepOutResponse>> _handler;

            public DelegatingHandler(Func<StepOutArguments, CancellationToken, Task<StepOutResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<StepOutResponse> Handle(StepOutArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
