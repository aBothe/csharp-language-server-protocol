using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    [Parallel, Method(GeneralNames.Progress)]
    public interface IProgressHandler : IJsonRpcNotificationHandler<ProgressParams>
    {
    }

    public abstract class ProgressHandler : IProgressHandler
    {
        public abstract Task<Unit> Handle(ProgressParams request, CancellationToken cancellationToken);
    }

    public static class ProgressExtensions
    {
        public static IDisposable OnProgress(
            this ILanguageServerRegistry registry,
            Action<ProgressParams, CancellationToken> handler)
        {
            return registry.AddHandler(GeneralNames.Progress, NotificationHandler.For(handler));
        }

        public static IDisposable OnProgress(
            this ILanguageServerRegistry registry,
            Func<ProgressParams, CancellationToken, Task> handler)
        {
            return registry.AddHandler(GeneralNames.Progress, NotificationHandler.For(handler));
        }

        public static IDisposable OnProgress(
            this ILanguageServerRegistry registry,
            Action<ProgressParams> handler)
        {
            return registry.AddHandler(GeneralNames.Progress, NotificationHandler.For(handler));
        }

        public static IDisposable OnProgress(
            this ILanguageServerRegistry registry,
            Func<ProgressParams, Task> handler)
        {
            return registry.AddHandler(GeneralNames.Progress, NotificationHandler.For(handler));
        }

        public static IDisposable OnProgress(
            this ILanguageClientRegistry registry,
            Action<ProgressParams, CancellationToken> handler)
        {
            return registry.AddHandler(GeneralNames.Progress, NotificationHandler.For(handler));
        }

        public static IDisposable OnProgress(
            this ILanguageClientRegistry registry,
            Func<ProgressParams, CancellationToken, Task> handler)
        {
            return registry.AddHandler(GeneralNames.Progress, NotificationHandler.For(handler));
        }

        public static IDisposable OnProgress(
            this ILanguageClientRegistry registry,
            Action<ProgressParams> handler)
        {
            return registry.AddHandler(GeneralNames.Progress, NotificationHandler.For(handler));
        }

        public static IDisposable OnProgress(
            this ILanguageClientRegistry registry,
            Func<ProgressParams, Task> handler)
        {
            return registry.AddHandler(GeneralNames.Progress, NotificationHandler.For(handler));
        }
    }
}
