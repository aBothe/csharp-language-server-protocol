using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Variables)]
    public interface IVariablesHandler : IJsonRpcRequestHandler<VariablesArguments, VariablesResponse> { }

    public abstract class VariablesHandler : IVariablesHandler
    {
        public abstract Task<VariablesResponse> Handle(VariablesArguments request, CancellationToken cancellationToken);
    }

    public static class VariablesHandlerExtensions
    {
        public static IDisposable OnVariables(this IDebugAdapterRegistry registry, Func<VariablesArguments, CancellationToken, Task<VariablesResponse>> handler)
        {
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler));
        }

        class DelegatingHandler : VariablesHandler
        {
            private readonly Func<VariablesArguments, CancellationToken, Task<VariablesResponse>> _handler;

            public DelegatingHandler(Func<VariablesArguments, CancellationToken, Task<VariablesResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<VariablesResponse> Handle(VariablesArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
