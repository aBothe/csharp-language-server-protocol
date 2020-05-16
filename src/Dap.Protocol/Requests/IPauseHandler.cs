using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Pause)]
    public interface IPauseHandler : IJsonRpcRequestHandler<PauseArguments, PauseResponse>
    {
    }

    public abstract class PauseHandler : IPauseHandler
    {
        public abstract Task<PauseResponse> Handle(PauseArguments request, CancellationToken cancellationToken);
    }

    public static class PauseExtensions
    {
        public static IDisposable OnPause(this IDebugAdapterRegistry registry,
            Func<PauseArguments, CancellationToken, Task<PauseResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Pause, RequestHandler.For(handler));
        }

        public static IDisposable OnPause(this IDebugAdapterRegistry registry,
            Func<PauseArguments, Task<PauseResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Pause, RequestHandler.For(handler));
        }
    }
}
