using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Server
{
    [Serial, Method(WorkspaceNames.DidChangeWatchedFiles)]
    public interface IDidChangeWatchedFilesHandler : IJsonRpcNotificationHandler<DidChangeWatchedFilesParams>, IRegistration<DidChangeWatchedFilesRegistrationOptions>, ICapability<DidChangeWatchedFilesCapability> { }

    public abstract class DidChangeWatchedFilesHandler : IDidChangeWatchedFilesHandler
    {
        private readonly DidChangeWatchedFilesRegistrationOptions _options;
        public DidChangeWatchedFilesHandler(DidChangeWatchedFilesRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public DidChangeWatchedFilesRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Unit> Handle(DidChangeWatchedFilesParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DidChangeWatchedFilesCapability capability) => Capability = capability;
        protected DidChangeWatchedFilesCapability Capability { get; private set; }
    }

    public static class DidChangeWatchedFilesHandlerExtensions
    {
        public static IDisposable OnDidChangeWatchedFiles(
            this ILanguageServerRegistry registry,
            Action<DidChangeWatchedFilesParams, SynchronizationCapability> handler,
            TextDocumentChangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentChangeRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.DidChangeWatchedFiles,
                new LanguageProtocolDelegatingHandlers.Notification<DidChangeWatchedFilesParams, SynchronizationCapability,
                    TextDocumentChangeRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidChangeWatchedFiles(
            this ILanguageServerRegistry registry,
            Action<DidChangeWatchedFilesParams> handler,
            TextDocumentChangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentChangeRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.DidChangeWatchedFiles,
                new LanguageProtocolDelegatingHandlers.Notification<DidChangeWatchedFilesParams,
                    TextDocumentChangeRegistrationOptions>(handler, registrationOptions));
        }
    }
}
