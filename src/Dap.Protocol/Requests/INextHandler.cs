using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Next)]
    public interface INextHandler : IJsonRpcRequestHandler<NextArguments, NextResponse> { }

    public abstract class NextHandler : INextHandler
    {
        public abstract Task<NextResponse> Handle(NextArguments request, CancellationToken cancellationToken);
    }

    public static class NextHandlerExtensions
    {
        public static IDisposable OnNext(this IDebugAdapterRegistry registry, Func<NextArguments, CancellationToken, Task<NextResponse>> handler)
        {
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler));
        }

        class DelegatingHandler : NextHandler
        {
            private readonly Func<NextArguments, CancellationToken, Task<NextResponse>> _handler;

            public DelegatingHandler(Func<NextArguments, CancellationToken, Task<NextResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<NextResponse> Handle(NextArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
