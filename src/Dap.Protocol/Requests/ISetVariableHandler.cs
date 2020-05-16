using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.SetVariable)]
    public interface ISetVariableHandler : IJsonRpcRequestHandler<SetVariableArguments, SetVariableResponse>
    {
    }

    public abstract class SetVariableHandler : ISetVariableHandler
    {
        public abstract Task<SetVariableResponse> Handle(SetVariableArguments request,
            CancellationToken cancellationToken);
    }

    public static class SetVariableExtensions
    {
        public static IDisposable OnSetVariable(this IDebugAdapterRegistry registry,
            Func<SetVariableArguments, CancellationToken, Task<SetVariableResponse>> handler)
        {
            return registry.AddHandler(RequestNames.SetVariable, RequestHandler.For(handler));
        }

        public static IDisposable OnSetVariable(this IDebugAdapterRegistry registry,
            Func<SetVariableArguments, Task<SetVariableResponse>> handler)
        {
            return registry.AddHandler(RequestNames.SetVariable, RequestHandler.For(handler));
        }
    }
}
