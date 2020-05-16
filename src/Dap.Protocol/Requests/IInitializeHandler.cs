using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Initialize)]
    public interface IInitializeHandler : IJsonRpcRequestHandler<InitializeRequestArguments, InitializeResponse>
    {
    }

    public abstract class InitializeHandler : IInitializeHandler
    {
        public abstract Task<InitializeResponse> Handle(InitializeRequestArguments request,
            CancellationToken cancellationToken);
    }

    public static class InitializeExtensions
    {
        public static IDisposable OnInitialize(this IDebugAdapterRegistry registry,
            Func<InitializeRequestArguments, CancellationToken, Task<InitializeResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Initialize, RequestHandler.For(handler));
        }

        public static IDisposable OnInitialize(this IDebugAdapterRegistry registry,
            Func<InitializeRequestArguments, Task<InitializeResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Initialize, RequestHandler.For(handler));
        }
    }
}
