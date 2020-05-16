using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Threading.Tasks;
using MediatR;
using System.Threading;
using System;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Server
{
    [Parallel, Method(WorkspaceNames.DidChangeWorkspaceFolders)]
    public interface IDidChangeWorkspaceFoldersHandler : IJsonRpcNotificationHandler<DidChangeWorkspaceFoldersParams>,
        ICapability<DidChangeWorkspaceFolderCapability>, IRegistration<object>
    {
    }

    public abstract class DidChangeWorkspaceFoldersHandler : IDidChangeWorkspaceFoldersHandler
    {
        public object GetRegistrationOptions() => new object();
        public abstract Task<Unit> Handle(DidChangeWorkspaceFoldersParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DidChangeWorkspaceFolderCapability capability) => Capability = capability;
        protected DidChangeWorkspaceFolderCapability Capability { get; private set; }
    }

    public static class DidChangeWorkspaceFoldersExtensions
    {
        public static IDisposable OnDidChangeWorkspaceFolders(
            this ILanguageServerRegistry registry,
            Action<DidChangeWorkspaceFoldersParams, DidChangeWorkspaceFolderCapability, CancellationToken> handler)
        {
            return registry.AddHandler(WorkspaceNames.DidChangeWorkspaceFolders,
                new LanguageProtocolDelegatingHandlers.NotificationCapability<DidChangeWorkspaceFoldersParams,
                    DidChangeWorkspaceFolderCapability>((r, c, ct) => {
                    handler(r, c, ct);
                    return Task.CompletedTask;
                }));
        }

        public static IDisposable OnDidChangeWorkspaceFolders(
            this ILanguageServerRegistry registry,
            Action<DidChangeWorkspaceFoldersParams, DidChangeWorkspaceFolderCapability> handler)
        {
            return registry.AddHandler(WorkspaceNames.DidChangeWorkspaceFolders,
                new LanguageProtocolDelegatingHandlers.NotificationCapability<DidChangeWorkspaceFoldersParams,
                    DidChangeWorkspaceFolderCapability>(handler));
        }

        public static IDisposable OnDidChangeWorkspaceFolders(
            this ILanguageServerRegistry registry,
            Action<DidChangeWorkspaceFoldersParams, CancellationToken> handler)
        {
            return registry.AddHandler(WorkspaceNames.DidChangeWorkspaceFolders,
                NotificationHandler.For(handler));
        }

        public static IDisposable OnDidChangeWorkspaceFolders(
            this ILanguageServerRegistry registry,
            Action<DidChangeWorkspaceFoldersParams> handler)
        {
            return registry.AddHandler(WorkspaceNames.DidChangeWorkspaceFolders,
                NotificationHandler.For(handler));
        }

        public static IDisposable OnDidChangeWorkspaceFolders(
            this ILanguageServerRegistry registry,
            Func<DidChangeWorkspaceFoldersParams, DidChangeWorkspaceFolderCapability, CancellationToken, Task> handler)
        {
            return registry.AddHandler(WorkspaceNames.DidChangeWorkspaceFolders,
                new LanguageProtocolDelegatingHandlers.NotificationCapability<DidChangeWorkspaceFoldersParams,
                    DidChangeWorkspaceFolderCapability>(handler));
        }

        public static IDisposable OnDidChangeWorkspaceFolders(
            this ILanguageServerRegistry registry,
            Func<DidChangeWorkspaceFoldersParams, DidChangeWorkspaceFolderCapability, Task> handler)
        {
            return registry.AddHandler(WorkspaceNames.DidChangeWorkspaceFolders,
                new LanguageProtocolDelegatingHandlers.NotificationCapability<DidChangeWorkspaceFoldersParams,
                    DidChangeWorkspaceFolderCapability>(handler));
        }

        public static IDisposable OnDidChangeWorkspaceFolders(
            this ILanguageServerRegistry registry,
            Func<DidChangeWorkspaceFoldersParams, CancellationToken, Task> handler)
        {
            return registry.AddHandler(WorkspaceNames.DidChangeWorkspaceFolders,
                NotificationHandler.For(handler));
        }

        public static IDisposable OnDidChangeWorkspaceFolders(
            this ILanguageServerRegistry registry,
            Func<DidChangeWorkspaceFoldersParams, Task> handler)
        {
            return registry.AddHandler(WorkspaceNames.DidChangeWorkspaceFolders,
                NotificationHandler.For(handler));
        }
    }
}
