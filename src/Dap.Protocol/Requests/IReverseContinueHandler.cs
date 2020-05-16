using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.ReverseContinue)]
    public interface IReverseContinueHandler : IJsonRpcRequestHandler<ReverseContinueArguments, ReverseContinueResponse>
    {
    }

    public abstract class ReverseContinueHandler : IReverseContinueHandler
    {
        public abstract Task<ReverseContinueResponse> Handle(ReverseContinueArguments request,
            CancellationToken cancellationToken);
    }

    public static class ReverseContinueExtensions
    {
        public static IDisposable OnReverseContinue(this IDebugAdapterRegistry registry,
            Func<ReverseContinueArguments, CancellationToken, Task<ReverseContinueResponse>> handler)
        {
            return registry.AddHandler(RequestNames.ReverseContinue, RequestHandler.For(handler));
        }

        public static IDisposable OnReverseContinue(this IDebugAdapterRegistry registry,
            Func<ReverseContinueArguments, Task<ReverseContinueResponse>> handler)
        {
            return registry.AddHandler(RequestNames.ReverseContinue, RequestHandler.For(handler));
        }
    }
}
