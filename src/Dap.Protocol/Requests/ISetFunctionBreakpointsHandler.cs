using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.SetFunctionBreakpoints)]
    public interface ISetFunctionBreakpointsHandler : IJsonRpcRequestHandler<SetFunctionBreakpointsArguments, SetFunctionBreakpointsResponse> { }

    public abstract class SetFunctionBreakpointsHandler : ISetFunctionBreakpointsHandler
    {
        public abstract Task<SetFunctionBreakpointsResponse> Handle(SetFunctionBreakpointsArguments request, CancellationToken cancellationToken);
    }

    public static class SetFunctionBreakpointsHandlerExtensions
    {
        public static IDisposable OnSetFunctionBreakpoints(this IDebugAdapterRegistry registry, Func<SetFunctionBreakpointsArguments, CancellationToken, Task<SetFunctionBreakpointsResponse>> handler)
        {
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler));
        }

        class DelegatingHandler : SetFunctionBreakpointsHandler
        {
            private readonly Func<SetFunctionBreakpointsArguments, CancellationToken, Task<SetFunctionBreakpointsResponse>> _handler;

            public DelegatingHandler(Func<SetFunctionBreakpointsArguments, CancellationToken, Task<SetFunctionBreakpointsResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<SetFunctionBreakpointsResponse> Handle(SetFunctionBreakpointsArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
