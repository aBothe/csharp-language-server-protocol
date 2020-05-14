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
    [Serial, Method(TextDocumentNames.DidSave)]
    public interface IDidSaveTextDocumentHandler : IJsonRpcNotificationHandler<DidSaveTextDocumentParams>, IRegistration<TextDocumentSaveRegistrationOptions>, ICapability<SynchronizationCapability> { }

    public abstract class DidSaveTextDocumentHandler : IDidSaveTextDocumentHandler
    {
        private readonly TextDocumentSaveRegistrationOptions _options;
        public DidSaveTextDocumentHandler(TextDocumentSaveRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public TextDocumentSaveRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(SynchronizationCapability capability) => Capability = capability;
        protected SynchronizationCapability Capability { get; private set; }
    }

    public static class DidSaveTextDocumentHandlerExtensions
    {
        public static IDisposable OnDidSaveTextDocument(
            this ILanguageServerRegistry registry,
            Func<DidSaveTextDocumentParams, CancellationToken, Task<Unit>> handler,
            TextDocumentSaveRegistrationOptions registrationOptions = null,
            Action<SynchronizationCapability> setCapability = null)
        {
            registrationOptions ??= new TextDocumentSaveRegistrationOptions();
            setCapability ??= x => { };
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : DidSaveTextDocumentHandler
        {
            private readonly Func<DidSaveTextDocumentParams, CancellationToken, Task<Unit>> _handler;
            private readonly Action<SynchronizationCapability> _setCapability;

            public DelegatingHandler(
                Func<DidSaveTextDocumentParams, CancellationToken, Task<Unit>> handler,
                Action<SynchronizationCapability> setCapability,
                TextDocumentSaveRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(SynchronizationCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
