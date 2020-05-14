using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.SetVariable)]
    public interface ISetVariableHandler : IJsonRpcRequestHandler<SetVariableArguments, SetVariableResponse> { }

    public abstract class SetVariableHandler : ISetVariableHandler
    {
        public abstract Task<SetVariableResponse> Handle(SetVariableArguments request, CancellationToken cancellationToken);
    }

    public static class SetVariableHandlerExtensions
    {
        public static IDisposable OnSetVariable(this IDebugAdapterRegistry registry, Func<SetVariableArguments, CancellationToken, Task<SetVariableResponse>> handler)
        {
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler));
        }

        class DelegatingHandler : SetVariableHandler
        {
            private readonly Func<SetVariableArguments, CancellationToken, Task<SetVariableResponse>> _handler;

            public DelegatingHandler(Func<SetVariableArguments, CancellationToken, Task<SetVariableResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<SetVariableResponse> Handle(SetVariableArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
