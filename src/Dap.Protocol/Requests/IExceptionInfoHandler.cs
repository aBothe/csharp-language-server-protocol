using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.ExceptionInfo)]
    public interface IExceptionInfoHandler : IJsonRpcRequestHandler<ExceptionInfoArguments, ExceptionInfoResponse>
    {
    }

    public abstract class ExceptionInfoHandler : IExceptionInfoHandler
    {
        public abstract Task<ExceptionInfoResponse> Handle(ExceptionInfoArguments request,
            CancellationToken cancellationToken);
    }

    public static class ExceptionInfoHandlerExtensions
    {
        public static IDisposable OnExceptionInfo(this IDebugAdapterRegistry registry,
            Func<ExceptionInfoArguments, CancellationToken, Task<ExceptionInfoResponse>> handler)
        {
            return registry.AddHandler(RequestNames.ExceptionInfo, RequestHandler.For(handler));
        }

        public static IDisposable OnExceptionInfo(this IDebugAdapterRegistry registry,
            Func<ExceptionInfoArguments, Task<ExceptionInfoResponse>> handler)
        {
            return registry.AddHandler(RequestNames.ExceptionInfo, RequestHandler.For(handler));
        }
    }
}
