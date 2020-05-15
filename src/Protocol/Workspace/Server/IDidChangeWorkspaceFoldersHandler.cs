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
    public interface IDidChangeWorkspaceFoldersHandler : IJsonRpcNotificationHandler<DidChangeWorkspaceFoldersParams>, ICapability<DidChangeWorkspaceFolderCapability>, IRegistration<object> { }

    public abstract class DidChangeWorkspaceFoldersHandler : IDidChangeWorkspaceFoldersHandler
    {
        public object GetRegistrationOptions() => new object();
        public abstract Task<Unit> Handle(DidChangeWorkspaceFoldersParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DidChangeWorkspaceFolderCapability capability) => Capability = capability;
        protected DidChangeWorkspaceFolderCapability Capability { get; private set; }
    }

    public static class DidChangeWorkspaceFoldersHandlerExtensions
    {
        public static IDisposable OnDidChangeWorkspaceFolders(
            this ILanguageServerRegistry registry,
            Action<DidChangeWorkspaceFoldersParams, SynchronizationCapability> handler,
            TextDocumentChangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentChangeRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.DidChangeWorkspaceFolders,
                new LanguageProtocolDelegatingHandlers.Notification<DidChangeWorkspaceFoldersParams, SynchronizationCapability,
                    TextDocumentChangeRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidChangeWorkspaceFolders(
            this ILanguageServerRegistry registry,
            Action<DidChangeWorkspaceFoldersParams> handler,
            TextDocumentChangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentChangeRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.DidChangeWorkspaceFolders,
                new LanguageProtocolDelegatingHandlers.Notification<DidChangeWorkspaceFoldersParams,
                    TextDocumentChangeRegistrationOptions>(handler, registrationOptions));
        }
    }
}
