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
    [Serial, Method(TextDocumentNames.DidOpen)]
    public interface IDidOpenTextDocumentHandler : IJsonRpcNotificationHandler<DidOpenTextDocumentParams>, IRegistration<TextDocumentRegistrationOptions>, ICapability<SynchronizationCapability> { }

    public abstract class DidOpenTextDocumentHandler : IDidOpenTextDocumentHandler
    {
        private readonly TextDocumentRegistrationOptions _options;
        public DidOpenTextDocumentHandler(TextDocumentRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public TextDocumentRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(SynchronizationCapability capability) => Capability = capability;
        protected SynchronizationCapability Capability { get; private set; }
    }

    public static class DidOpenTextDocumentHandlerExtensions
    {
        public static IDisposable OnDidOpenTextDocument(
            this ILanguageServerRegistry registry,
            Action<DidOpenTextDocumentParams, SynchronizationCapability> handler,
            TextDocumentChangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentChangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidOpen,
                new LanguageProtocolDelegatingHandlers.Notification<DidOpenTextDocumentParams, SynchronizationCapability,
                    TextDocumentChangeRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidOpenTextDocument(
            this ILanguageServerRegistry registry,
            Action<DidOpenTextDocumentParams> handler,
            TextDocumentChangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentChangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidOpen,
                new LanguageProtocolDelegatingHandlers.Notification<DidOpenTextDocumentParams,
                    TextDocumentChangeRegistrationOptions>(handler, registrationOptions));
        }
    }
}
