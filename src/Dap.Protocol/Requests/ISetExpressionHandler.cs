using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.SetExpression)]
    public interface ISetExpressionHandler : IJsonRpcRequestHandler<SetExpressionArguments, SetExpressionResponse>
    {
    }

    public abstract class SetExpressionHandler : ISetExpressionHandler
    {
        public abstract Task<SetExpressionResponse> Handle(SetExpressionArguments request,
            CancellationToken cancellationToken);
    }

    public static class SetExpressionHandlerExtensions
    {
        public static IDisposable OnSetExpression(this IDebugAdapterRegistry registry,
            Func<SetExpressionArguments, CancellationToken, Task<SetExpressionResponse>> handler)
        {
            return registry.AddHandler(RequestNames.SetExpression, RequestHandler.For(handler));
        }

        public static IDisposable OnSetExpression(this IDebugAdapterRegistry registry,
            Func<SetExpressionArguments, Task<SetExpressionResponse>> handler)
        {
            return registry.AddHandler(RequestNames.SetExpression, RequestHandler.For(handler));
        }
    }
}
