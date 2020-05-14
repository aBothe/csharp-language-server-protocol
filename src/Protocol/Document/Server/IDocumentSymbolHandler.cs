using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Server
{
    [Parallel, Method(TextDocumentNames.DocumentSymbol)]
    public interface IDocumentSymbolHandler : IJsonRpcRequestHandler<DocumentSymbolParams, SymbolInformationOrDocumentSymbolContainer>, IRegistration<DocumentSymbolRegistrationOptions>, ICapability<DocumentSymbolCapability> { }

    public abstract class DocumentSymbolHandler : IDocumentSymbolHandler
    {
        private readonly DocumentSymbolRegistrationOptions _options;
        protected IWorkDoneProgressManager ProgressManager { get; }
        public DocumentSymbolHandler(DocumentSymbolRegistrationOptions registrationOptions, IWorkDoneProgressManager progressManager)
        {
            _options = registrationOptions;
            ProgressManager = progressManager;
        }

        public DocumentSymbolRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<SymbolInformationOrDocumentSymbolContainer> Handle(DocumentSymbolParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DocumentSymbolCapability capability) => Capability = capability;
        protected DocumentSymbolCapability Capability { get; private set; }
    }

    public static class DocumentSymbolHandlerExtensions
    {
        public static IDisposable OnDocumentSymbol(
            this ILanguageServerRegistry registry,
            Func<DocumentSymbolParams, CancellationToken, Task<SymbolInformationOrDocumentSymbolContainer>> handler,
            DocumentSymbolRegistrationOptions registrationOptions = null,
            Action<DocumentSymbolCapability> setCapability = null)
        {
            registrationOptions ??= new DocumentSymbolRegistrationOptions();
            setCapability ??= x => { };
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : DocumentSymbolHandler
        {
            private readonly Func<DocumentSymbolParams, CancellationToken, Task<SymbolInformationOrDocumentSymbolContainer>> _handler;
            private readonly Action<DocumentSymbolCapability> _setCapability;

            public DelegatingHandler(
                Func<DocumentSymbolParams, CancellationToken, Task<SymbolInformationOrDocumentSymbolContainer>> handler,
                IWorkDoneProgressManager progressManager,
                Action<DocumentSymbolCapability> setCapability,
                DocumentSymbolRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<SymbolInformationOrDocumentSymbolContainer> Handle(DocumentSymbolParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(DocumentSymbolCapability capability) => _setCapability?.Invoke(capability);
        }
    }
}
