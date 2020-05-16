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

    public static class DocumentSymbolExtensions
    {
        public static IDisposable OnDocumentSymbol(
            this ILanguageServerRegistry registry,
            Func<DocumentSymbolParams, DocumentSymbolCapability, CancellationToken, Task<SymbolInformationOrDocumentSymbolContainer>>
                handler,
            DocumentSymbolRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentSymbolRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentSymbol,
                new LanguageProtocolDelegatingHandlers.Request<DocumentSymbolParams, SymbolInformationOrDocumentSymbolContainer, DocumentSymbolCapability,
                    DocumentSymbolRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDocumentSymbol(
            this ILanguageServerRegistry registry,
            Func<DocumentSymbolParams, CancellationToken, Task<SymbolInformationOrDocumentSymbolContainer>> handler,
            DocumentSymbolRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentSymbolRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentSymbol,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentSymbolParams, SymbolInformationOrDocumentSymbolContainer,
                    DocumentSymbolRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDocumentSymbol(
            this ILanguageServerRegistry registry,
            Func<DocumentSymbolParams, Task<SymbolInformationOrDocumentSymbolContainer>> handler,
            DocumentSymbolRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentSymbolRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentSymbol,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentSymbolParams, SymbolInformationOrDocumentSymbolContainer,
                    DocumentSymbolRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDocumentSymbol(
            this ILanguageServerRegistry registry,
            Action<DocumentSymbolParams, IObserver<Container<SymbolInformationOrDocumentSymbol>>, DocumentSymbolCapability,
                CancellationToken> handler,
            DocumentSymbolRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentSymbolRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentSymbol,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DocumentSymbolParams, SymbolInformationOrDocumentSymbolContainer,
                        SymbolInformationOrDocumentSymbol, DocumentSymbolCapability, DocumentSymbolRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>()));
        }

        public static IDisposable OnDocumentSymbol(
            this ILanguageServerRegistry registry,
            Action<DocumentSymbolParams, IObserver<Container<SymbolInformationOrDocumentSymbol>>, DocumentSymbolCapability>
                handler,
            DocumentSymbolRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentSymbolRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentSymbol,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DocumentSymbolParams, SymbolInformationOrDocumentSymbolContainer,
                        SymbolInformationOrDocumentSymbol, DocumentSymbolCapability, DocumentSymbolRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>()));
        }

        public static IDisposable OnDocumentSymbol(
            this ILanguageServerRegistry registry,
            Action<DocumentSymbolParams, IObserver<Container<SymbolInformationOrDocumentSymbol>>, CancellationToken> handler,
            DocumentSymbolRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentSymbolRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentSymbol,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DocumentSymbolParams, SymbolInformationOrDocumentSymbolContainer,
                        SymbolInformationOrDocumentSymbol, DocumentSymbolRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>()));
        }

        public static IDisposable OnDocumentSymbol(
            this ILanguageServerRegistry registry,
            Action<DocumentSymbolParams, IObserver<Container<SymbolInformationOrDocumentSymbol>>> handler,
            DocumentSymbolRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentSymbolRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentSymbol,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DocumentSymbolParams, SymbolInformationOrDocumentSymbolContainer,
                        SymbolInformationOrDocumentSymbol, DocumentSymbolRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>()));
        }
    }
}
