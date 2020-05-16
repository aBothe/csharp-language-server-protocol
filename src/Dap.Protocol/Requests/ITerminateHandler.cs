using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Terminate)]
    public interface ITerminateHandler : IJsonRpcRequestHandler<TerminateArguments, TerminateResponse>
    {
    }

    public abstract class TerminateHandler : ITerminateHandler
    {
        public abstract Task<TerminateResponse> Handle(TerminateArguments request, CancellationToken cancellationToken);
    }

    public static class TerminateExtensions
    {
        public static IDisposable OnTerminate(this IDebugAdapterRegistry registry,
            Func<TerminateArguments, CancellationToken, Task<TerminateResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Terminate, RequestHandler.For(handler));
        }

        public static IDisposable OnTerminate(this IDebugAdapterRegistry registry,
            Func<TerminateArguments, Task<TerminateResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Terminate, RequestHandler.For(handler));
        }
    }
}
