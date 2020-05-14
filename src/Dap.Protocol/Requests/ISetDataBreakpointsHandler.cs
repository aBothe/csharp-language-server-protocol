using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.SetDataBreakpoints)]
    public interface ISetDataBreakpointsHandler : IJsonRpcRequestHandler<SetDataBreakpointsArguments, SetDataBreakpointsResponse> { }

    public abstract class SetDataBreakpointsHandler : ISetDataBreakpointsHandler
    {
        public abstract Task<SetDataBreakpointsResponse> Handle(SetDataBreakpointsArguments request, CancellationToken cancellationToken);
    }

    public static class SetDataBreakpointsHandlerExtensions
    {
        public static IDisposable OnSetDataBreakpoints(this IDebugAdapterRegistry registry, Func<SetDataBreakpointsArguments, CancellationToken, Task<SetDataBreakpointsResponse>> handler)
        {
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler));
        }

        class DelegatingHandler : SetDataBreakpointsHandler
        {
            private readonly Func<SetDataBreakpointsArguments, CancellationToken, Task<SetDataBreakpointsResponse>> _handler;

            public DelegatingHandler(Func<SetDataBreakpointsArguments, CancellationToken, Task<SetDataBreakpointsResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<SetDataBreakpointsResponse> Handle(SetDataBreakpointsArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
