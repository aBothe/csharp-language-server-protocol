using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Window.Server
{
    [Parallel, Method(WindowNames.WorkDoneProgressCreate)]
    public interface IWorkDoneProgressCreateHandler : IJsonRpcRequestHandler<WorkDoneProgressCreateParams> { }

    public abstract class WorkDoneProgressCreateHandler : IWorkDoneProgressCreateHandler
    {
        public abstract Task<Unit> Handle(WorkDoneProgressCreateParams request, CancellationToken cancellationToken);
    }

    public static class WorkDoneProgressCreateHandlerExtensions
    {
        public static IDisposable OnWorkDoneProgressCreate(
            this ILanguageServerRegistry registry,
            Func<WorkDoneProgressCreateParams, CancellationToken, Task<Unit>> handler)
        {
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler));
        }
        public static IDisposable OnWorkDoneProgressCreate(
            this ILanguageClientRegistry registry,
            Func<WorkDoneProgressCreateParams, CancellationToken, Task<Unit>> handler)
        {
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler));
        }

        class DelegatingHandler : WorkDoneProgressCreateHandler
        {
            private readonly Func<WorkDoneProgressCreateParams, CancellationToken, Task<Unit>> _handler;

            public DelegatingHandler(
                Func<WorkDoneProgressCreateParams, CancellationToken, Task<Unit>> handler)
            {
                _handler = handler;
            }

            public override Task<Unit> Handle(WorkDoneProgressCreateParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);

        }
    }
}
