using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Goto, Direction.ClientToServer)]
    public interface IGotoHandler : IJsonRpcRequestHandler<GotoArguments, GotoResponse>
    {
    }

    public abstract class GotoHandler : IGotoHandler
    {
        public abstract Task<GotoResponse> Handle(GotoArguments request, CancellationToken cancellationToken);
    }

    public static class GotoExtensions
    {
        public static IDisposable OnGoto(this IDebugAdapterRegistry registry,
            Func<GotoArguments, CancellationToken, Task<GotoResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Goto, RequestHandler.For(handler));
        }

        public static IDisposable OnGoto(this IDebugAdapterRegistry registry,
            Func<GotoArguments, Task<GotoResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Goto, RequestHandler.For(handler));
        }
    }
}
