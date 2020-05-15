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
    [Serial, Method(WorkspaceNames.DidChangeConfiguration)]
    public interface IDidChangeConfigurationHandler : IJsonRpcNotificationHandler<DidChangeConfigurationParams>, IRegistration<object>, ICapability<DidChangeConfigurationCapability> { }

    public abstract class DidChangeConfigurationHandler : IDidChangeConfigurationHandler
    {
        public object GetRegistrationOptions() => new object();
        public abstract Task<Unit> Handle(DidChangeConfigurationParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DidChangeConfigurationCapability capability) => Capability = capability;
        protected DidChangeConfigurationCapability Capability { get; private set; }
    }

    public static class DidChangeConfigurationHandlerExtensions
    {
        public static IDisposable OnDidChangeConfiguration(
            this ILanguageServerRegistry registry,
            Action<DidChangeConfigurationParams, SynchronizationCapability> handler,
            TextDocumentChangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentChangeRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.DidChangeConfiguration,
                new LanguageProtocolDelegatingHandlers.Notification<DidChangeConfigurationParams, SynchronizationCapability,
                    TextDocumentChangeRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidChangeConfiguration(
            this ILanguageServerRegistry registry,
            Action<DidChangeConfigurationParams> handler,
            TextDocumentChangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentChangeRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.DidChangeConfiguration,
                new LanguageProtocolDelegatingHandlers.Notification<DidChangeConfigurationParams,
                    TextDocumentChangeRegistrationOptions>(handler, registrationOptions));
        }
    }
}
