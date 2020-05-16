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

    public static class WorkDoneProgressCreateExtensions
    {
        public static IDisposable OnWorkDoneProgressCreate(
            this ILanguageClientRegistry registry,
            Func<WorkDoneProgressCreateParams, CancellationToken, Task>
                handler)
        {
            return registry.AddHandler(WindowNames.WorkDoneProgressCreate,                 RequestHandler.For(handler));
        }
        public static IDisposable OnWorkDoneProgressCreate(
            this ILanguageClientRegistry registry,
            Func<WorkDoneProgressCreateParams, Task>
                handler)
        {
            return registry.AddHandler(WindowNames.WorkDoneProgressCreate,                 RequestHandler.For(handler));
        }
    }
}
