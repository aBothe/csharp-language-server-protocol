using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Scopes)]
    public interface IScopesHandler : IJsonRpcRequestHandler<ScopesArguments, ScopesResponse> { }

    public abstract class ScopesHandler : IScopesHandler
    {
        public abstract Task<ScopesResponse> Handle(ScopesArguments request, CancellationToken cancellationToken);
    }

    public static class ScopesHandlerExtensions
    {
        public static IDisposable OnScopes(this IDebugAdapterRegistry registry, Func<ScopesArguments, CancellationToken, Task<ScopesResponse>> handler)
        {
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler));
        }

        class DelegatingHandler : ScopesHandler
        {
            private readonly Func<ScopesArguments, CancellationToken, Task<ScopesResponse>> _handler;

            public DelegatingHandler(Func<ScopesArguments, CancellationToken, Task<ScopesResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<ScopesResponse> Handle(ScopesArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
