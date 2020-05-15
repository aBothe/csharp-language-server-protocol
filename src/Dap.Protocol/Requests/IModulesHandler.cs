using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Modules)]
    public interface IModulesHandler : IJsonRpcRequestHandler<ModulesArguments, ModulesResponse>
    {
    }

    public abstract class ModulesHandler : IModulesHandler
    {
        public abstract Task<ModulesResponse> Handle(ModulesArguments request, CancellationToken cancellationToken);
    }

    public static class ModulesHandlerExtensions
    {
        public static IDisposable OnModules(this IDebugAdapterRegistry registry,
            Func<ModulesArguments, CancellationToken, Task<ModulesResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Modules, RequestHandler.For(handler));
        }

        public static IDisposable OnModules(this IDebugAdapterRegistry registry,
            Func<ModulesArguments, Task<ModulesResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Modules, RequestHandler.For(handler));
        }
    }
}
