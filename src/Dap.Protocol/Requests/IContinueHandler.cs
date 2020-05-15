using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Continue)]
    public interface IContinueHandler : IJsonRpcRequestHandler<ContinueArguments, ContinueResponse>
    {
    }

    public abstract class ContinueHandler : IContinueHandler
    {
        public abstract Task<ContinueResponse> Handle(ContinueArguments request, CancellationToken cancellationToken);
    }

    public static class ContinueHandlerExtensions
    {
        public static IDisposable OnContinue(this IDebugAdapterRegistry registry,
            Func<ContinueArguments, CancellationToken, Task<ContinueResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Continue, RequestHandler.For(handler));
        }

        public static IDisposable OnContinue(this IDebugAdapterRegistry registry,
            Func<ContinueArguments, Task<ContinueResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Continue, RequestHandler.For(handler));
        }
    }
}
