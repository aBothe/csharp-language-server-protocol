using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.StepInTargets)]
    public interface IStepInTargetsHandler : IJsonRpcRequestHandler<StepInTargetsArguments, StepInTargetsResponse>
    {
    }

    public abstract class StepInTargetsHandler : IStepInTargetsHandler
    {
        public abstract Task<StepInTargetsResponse> Handle(StepInTargetsArguments request,
            CancellationToken cancellationToken);
    }

    public static class StepInTargetsExtensions
    {
        public static IDisposable OnStepInTargets(this IDebugAdapterRegistry registry,
            Func<StepInTargetsArguments, CancellationToken, Task<StepInTargetsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.StepInTargets, RequestHandler.For(handler));
        }

        public static IDisposable OnStepInTargets(this IDebugAdapterRegistry registry,
            Func<StepInTargetsArguments, Task<StepInTargetsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.StepInTargets, RequestHandler.For(handler));
        }
    }
}
