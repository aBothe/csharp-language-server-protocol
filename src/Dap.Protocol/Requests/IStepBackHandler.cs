using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.StepBack)]
    public interface IStepBackHandler : IJsonRpcRequestHandler<StepBackArguments, StepBackResponse>
    {
    }

    public abstract class StepBackHandler : IStepBackHandler
    {
        public abstract Task<StepBackResponse> Handle(StepBackArguments request, CancellationToken cancellationToken);
    }

    public static class StepBackExtensions
    {
        public static IDisposable OnStepBack(this IDebugAdapterRegistry registry,
            Func<StepBackArguments, CancellationToken, Task<StepBackResponse>> handler)
        {
            return registry.AddHandler(RequestNames.StepBack, RequestHandler.For(handler));
        }

        public static IDisposable OnStepBack(this IDebugAdapterRegistry registry,
            Func<StepBackArguments, Task<StepBackResponse>> handler)
        {
            return registry.AddHandler(RequestNames.StepBack, RequestHandler.For(handler));
        }
    }
}
