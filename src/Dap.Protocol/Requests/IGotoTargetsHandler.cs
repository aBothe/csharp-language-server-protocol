using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.GotoTargets)]
    public interface IGotoTargetsHandler : IJsonRpcRequestHandler<GotoTargetsArguments, GotoTargetsResponse>
    {
    }


    public abstract class GotoTargetsHandler : IGotoTargetsHandler
    {
        public abstract Task<GotoTargetsResponse> Handle(GotoTargetsArguments request,
            CancellationToken cancellationToken);
    }

    public static class GotoTargetsExtensions
    {
        public static IDisposable OnGotoTargets(this IDebugAdapterRegistry registry,
            Func<GotoTargetsArguments, CancellationToken, Task<GotoTargetsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.GotoTargets, RequestHandler.For(handler));
        }

        public static IDisposable OnGotoTargets(this IDebugAdapterRegistry registry,
            Func<GotoTargetsArguments, Task<GotoTargetsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.GotoTargets, RequestHandler.For(handler));
        }
    }
}
