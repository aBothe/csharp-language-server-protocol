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
    [Parallel, Method(WindowNames.WorkDoneProgressCancel)]
    public interface IWorkDoneProgressCancelHandler : IJsonRpcNotificationHandler<WorkDoneProgressCancelParams> { }

    public abstract class WorkDoneProgressCancelHandler : IWorkDoneProgressCancelHandler
    {
        public abstract Task<Unit> Handle(WorkDoneProgressCancelParams request, CancellationToken cancellationToken);
    }

    public static class WorkDoneProgressCancelExtensions
    {
        public static IDisposable OnWorkDoneProgressCancel(
            this ILanguageServerRegistry registry,
            Action<WorkDoneProgressCancelParams, CancellationToken> handler)
        {
            return registry.AddHandler(WindowNames.WorkDoneProgressCancel, NotificationHandler.For(handler));
        }

        public static IDisposable OnWorkDoneProgressCancel(
            this ILanguageServerRegistry registry,
            Action<WorkDoneProgressCancelParams> handler)
        {
            return registry.AddHandler(WindowNames.WorkDoneProgressCancel, NotificationHandler.For(handler));
        }

        public static IDisposable OnWorkDoneProgressCancel(
            this ILanguageServerRegistry registry,
            Func<WorkDoneProgressCancelParams, CancellationToken, Task> handler)
        {
            return registry.AddHandler(WindowNames.WorkDoneProgressCancel, NotificationHandler.For(handler));
        }

        public static IDisposable OnWorkDoneProgressCancel(
            this ILanguageServerRegistry registry,
            Func<WorkDoneProgressCancelParams, Task> handler)
        {
            return registry.AddHandler(WindowNames.WorkDoneProgressCancel, NotificationHandler.For(handler));
        }
    }
}
