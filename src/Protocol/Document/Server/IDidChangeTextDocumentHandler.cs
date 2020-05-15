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
    [Serial, Method(TextDocumentNames.DidChange)]
    public interface IDidChangeTextDocumentHandler : IJsonRpcNotificationHandler<DidChangeTextDocumentParams>,
        IRegistration<TextDocumentChangeRegistrationOptions>, ICapability<SynchronizationCapability>
    { }

    public abstract class DidChangeTextDocumentHandler : IDidChangeTextDocumentHandler
    {
        private readonly TextDocumentChangeRegistrationOptions _options;
        public DidChangeTextDocumentHandler(TextDocumentChangeRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public TextDocumentChangeRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(SynchronizationCapability capability) => Capability = capability;
        protected SynchronizationCapability Capability { get; private set; }
    }

    public static class DidChangeTextDocumentHandlerExtensions
    {
        public static IDisposable OnDidChangeTextDocument(
            this ILanguageServerRegistry registry,
            Action<DidChangeTextDocumentParams, SynchronizationCapability> handler,
            TextDocumentChangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentChangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidChange,
                new LanguageProtocolDelegatingHandlers.Notification<DidChangeTextDocumentParams, SynchronizationCapability,
                    TextDocumentChangeRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidChangeTextDocument(
            this ILanguageServerRegistry registry,
            Action<DidChangeTextDocumentParams> handler,
            TextDocumentChangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentChangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidChange,
                new LanguageProtocolDelegatingHandlers.Notification<DidChangeTextDocumentParams,
                    TextDocumentChangeRegistrationOptions>(handler, registrationOptions));
        }
    }
}
