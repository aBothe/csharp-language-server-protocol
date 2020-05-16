using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.StepOut)]
    public interface IStepOutHandler : IJsonRpcRequestHandler<StepOutArguments, StepOutResponse>
    {
    }

    public abstract class StepOutHandler : IStepOutHandler
    {
        public abstract Task<StepOutResponse> Handle(StepOutArguments request, CancellationToken cancellationToken);
    }

    public static class StepOutExtensions
    {
        public static IDisposable OnStepOut(this IDebugAdapterRegistry registry,
            Func<StepOutArguments, CancellationToken, Task<StepOutResponse>> handler)
        {
            return registry.AddHandler(RequestNames.StepOut, RequestHandler.For(handler));
        }

        public static IDisposable OnStepOut(this IDebugAdapterRegistry registry,
            Func<StepOutArguments, Task<StepOutResponse>> handler)
        {
            return registry.AddHandler(RequestNames.StepOut, RequestHandler.For(handler));
        }
    }
}
