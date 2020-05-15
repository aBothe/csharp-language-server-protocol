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
    [Parallel, Method(TextDocumentNames.DocumentHighlight)]
    public interface IDocumentHighlightHandler : IJsonRpcRequestHandler<DocumentHighlightParams, DocumentHighlightContainer>, IRegistration<DocumentHighlightRegistrationOptions>, ICapability<DocumentHighlightCapability> { }

    public abstract class DocumentHighlightHandler : IDocumentHighlightHandler
    {
        private readonly DocumentHighlightRegistrationOptions _options;
        protected IWorkDoneProgressManager ProgressManager { get; }
        public DocumentHighlightHandler(DocumentHighlightRegistrationOptions registrationOptions, IWorkDoneProgressManager progressManager)
        {
            _options = registrationOptions;
            ProgressManager = progressManager;
        }

        public DocumentHighlightRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<DocumentHighlightContainer> Handle(DocumentHighlightParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DocumentHighlightCapability capability) => Capability = capability;
        protected DocumentHighlightCapability Capability { get; private set; }
    }

    public static class DocumentHighlightHandlerExtensions
    {
        public static IDisposable OnDocumentHighlight(
            this ILanguageServerRegistry registry,
            Func<DocumentHighlightParams, DocumentHighlightCapability, CancellationToken, Task<DocumentHighlightContainer>>
                handler,
            DocumentHighlightRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentHighlightRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentHighlight,
                new LanguageProtocolDelegatingHandlers.Request<DocumentHighlightParams, DocumentHighlightContainer, DocumentHighlightCapability,
                    DocumentHighlightRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDocumentHighlight(
            this ILanguageServerRegistry registry,
            Func<DocumentHighlightParams, CancellationToken, Task<DocumentHighlightContainer>> handler,
            DocumentHighlightRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentHighlightRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentHighlight,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentHighlightParams, DocumentHighlightContainer,
                    DocumentHighlightRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDocumentHighlight(
            this ILanguageServerRegistry registry,
            Func<DocumentHighlightParams, Task<DocumentHighlightContainer>> handler,
            DocumentHighlightRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentHighlightRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentHighlight,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentHighlightParams, DocumentHighlightContainer,
                    DocumentHighlightRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDocumentHighlight(
            this ILanguageServerRegistry registry,
            Action<DocumentHighlightParams, IObserver<Container<DocumentHighlight>>, DocumentHighlightCapability,
                CancellationToken> handler,
            DocumentHighlightRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentHighlightRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentHighlight,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DocumentHighlightParams, DocumentHighlightContainer,
                        DocumentHighlight, DocumentHighlightCapability, DocumentHighlightRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>()));
        }

        public static IDisposable OnDocumentHighlight(
            this ILanguageServerRegistry registry,
            Action<DocumentHighlightParams, IObserver<Container<DocumentHighlight>>, DocumentHighlightCapability>
                handler,
            DocumentHighlightRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentHighlightRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentHighlight,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DocumentHighlightParams, DocumentHighlightContainer,
                        DocumentHighlight, DocumentHighlightCapability, DocumentHighlightRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>()));
        }

        public static IDisposable OnDocumentHighlight(
            this ILanguageServerRegistry registry,
            Action<DocumentHighlightParams, IObserver<Container<DocumentHighlight>>, CancellationToken> handler,
            DocumentHighlightRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentHighlightRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentHighlight,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DocumentHighlightParams, DocumentHighlightContainer,
                        DocumentHighlight, DocumentHighlightRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>()));
        }

        public static IDisposable OnDocumentHighlight(
            this ILanguageServerRegistry registry,
            Action<DocumentHighlightParams, IObserver<Container<DocumentHighlight>>> handler,
            DocumentHighlightRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentHighlightRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentHighlight,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DocumentHighlightParams, DocumentHighlightContainer,
                        DocumentHighlight, DocumentHighlightRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>()));
        }
    }
}
