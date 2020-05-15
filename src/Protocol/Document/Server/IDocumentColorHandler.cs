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
    [Parallel, Method(TextDocumentNames.DocumentColor)]
    public interface IDocumentColorHandler : IJsonRpcRequestHandler<DocumentColorParams, Container<ColorPresentation>>, IRegistration<DocumentColorRegistrationOptions>, ICapability<ColorProviderCapability> { }

    public abstract class DocumentColorHandler : IDocumentColorHandler
    {
        private readonly DocumentColorRegistrationOptions _options;
        protected IWorkDoneProgressManager ProgressManager { get; }
        public DocumentColorHandler(DocumentColorRegistrationOptions registrationOptions, IWorkDoneProgressManager progressManager)
        {
            _options = registrationOptions;
            ProgressManager = progressManager;
        }

        public DocumentColorRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Container<ColorPresentation>> Handle(DocumentColorParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(ColorProviderCapability capability) => Capability = capability;
        protected ColorProviderCapability Capability { get; private set; }
    }

    public static class DocumentColorHandlerExtensions
    {
        public static IDisposable OnDocumentColor(
            this ILanguageServerRegistry registry,
            Func<DocumentColorParams, ColorProviderCapability, CancellationToken, Task<Container<ColorPresentation>>>
                handler,
            DocumentColorRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentColorRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentColor,
                new LanguageProtocolDelegatingHandlers.Request<DocumentColorParams, Container<ColorPresentation>, ColorProviderCapability,
                    DocumentColorRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDocumentColor(
            this ILanguageServerRegistry registry,
            Func<DocumentColorParams, CancellationToken, Task<Container<ColorPresentation>>> handler,
            DocumentColorRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentColorRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentColor,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentColorParams, Container<ColorPresentation>,
                    DocumentColorRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDocumentColor(
            this ILanguageServerRegistry registry,
            Func<DocumentColorParams, Task<Container<ColorPresentation>>> handler,
            DocumentColorRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentColorRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentColor,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentColorParams, Container<ColorPresentation>,
                    DocumentColorRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDocumentColor(
            this ILanguageServerRegistry registry,
            Action<DocumentColorParams, IObserver<Container<ColorPresentation>>, ColorProviderCapability,
                CancellationToken> handler,
            DocumentColorRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentColorRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentColor,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DocumentColorParams, Container<ColorPresentation>,
                        ColorPresentation, ColorProviderCapability, DocumentColorRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>()));
        }

        public static IDisposable OnDocumentColor(
            this ILanguageServerRegistry registry,
            Action<DocumentColorParams, IObserver<Container<ColorPresentation>>, ColorProviderCapability>
                handler,
            DocumentColorRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentColorRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentColor,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DocumentColorParams, Container<ColorPresentation>,
                        ColorPresentation, ColorProviderCapability, DocumentColorRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>()));
        }

        public static IDisposable OnDocumentColor(
            this ILanguageServerRegistry registry,
            Action<DocumentColorParams, IObserver<Container<ColorPresentation>>, CancellationToken> handler,
            DocumentColorRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentColorRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentColor,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DocumentColorParams, Container<ColorPresentation>,
                        ColorPresentation, DocumentColorRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>()));
        }

        public static IDisposable OnDocumentColor(
            this ILanguageServerRegistry registry,
            Action<DocumentColorParams, IObserver<Container<ColorPresentation>>> handler,
            DocumentColorRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentColorRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentColor,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DocumentColorParams, Container<ColorPresentation>,
                        ColorPresentation, DocumentColorRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>()));
        }
    }
}
