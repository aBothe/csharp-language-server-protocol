using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Completions)]
    public interface ICompletionsHandler : IJsonRpcRequestHandler<CompletionsArguments, CompletionsResponse> { }

    public abstract class CompletionsHandler : ICompletionsHandler
    {
        public abstract Task<CompletionsResponse> Handle(CompletionsArguments request, CancellationToken cancellationToken);
    }

    public static class CompletionsHandlerExtensions
    {
        public static IDisposable OnCompletions(this IDebugAdapterRegistry registry, Func<CompletionsArguments, CancellationToken, Task<CompletionsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Completions, RequestHandler.For(handler));
        }
        public static IDisposable OnCompletions(this IDebugAdapterRegistry registry, Func<CompletionsArguments, Task<CompletionsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Completions, RequestHandler.For(handler));
        }
    }

}
