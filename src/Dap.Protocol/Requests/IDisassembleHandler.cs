using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Disassemble)]
    public interface IDisassembleHandler : IJsonRpcRequestHandler<DisassembleArguments, DisassembleResponse>
    {
    }

    public abstract class DisassembleHandler : IDisassembleHandler
    {
        public abstract Task<DisassembleResponse> Handle(DisassembleArguments request,
            CancellationToken cancellationToken);
    }

    public static class DisassembleHandlerExtensions
    {
        public static IDisposable OnDisassemble(this IDebugAdapterRegistry registry,
            Func<DisassembleArguments, CancellationToken, Task<DisassembleResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Disassemble, RequestHandler.For(handler));
        }

        public static IDisposable OnDisassemble(this IDebugAdapterRegistry registry,
            Func<DisassembleArguments, Task<DisassembleResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Disassemble, RequestHandler.For(handler));
        }
    }
}
