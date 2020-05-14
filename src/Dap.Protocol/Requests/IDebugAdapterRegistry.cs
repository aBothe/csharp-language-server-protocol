using System;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public interface IDebugAdapterRegistry : IJsonRpcHandlerRegistry
    {
        IDisposable AddHandler<T>(Func<IServiceProvider, T> handlerFunc) where T : IJsonRpcHandler;
    }
}
