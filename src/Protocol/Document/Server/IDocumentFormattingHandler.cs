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
    [Serial, Method(TextDocumentNames.DocumentFormatting)]
    public interface IDocumentFormattingHandler : IJsonRpcRequestHandler<DocumentFormattingParams, TextEditContainer>, IRegistration<DocumentFormattingRegistrationOptions>, ICapability<DocumentFormattingCapability> { }

    public abstract class DocumentFormattingHandler : IDocumentFormattingHandler
    {
        private readonly DocumentFormattingRegistrationOptions _options;
        public DocumentFormattingHandler(DocumentFormattingRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public DocumentFormattingRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<TextEditContainer> Handle(DocumentFormattingParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DocumentFormattingCapability capability) => Capability = capability;
        protected DocumentFormattingCapability Capability { get; private set; }
    }

    public static class DocumentFormattingHandlerExtensions
    {
        public static IDisposable OnDocumentFormatting(
            this ILanguageServerRegistry registry,
            Func<DocumentFormattingParams, DocumentFormattingCapability, CancellationToken, Task<TextEditContainer>>
                handler,
            DocumentFormattingRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentFormattingRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentFormatting,
                new LanguageProtocolDelegatingHandlers.Request<DocumentFormattingParams, TextEditContainer, DocumentFormattingCapability,
                    DocumentFormattingRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDocumentFormatting(
            this ILanguageServerRegistry registry,
            Func<DocumentFormattingParams, CancellationToken, Task<TextEditContainer>> handler,
            DocumentFormattingRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentFormattingRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentFormatting,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentFormattingParams, TextEditContainer,
                    DocumentFormattingRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDocumentFormatting(
            this ILanguageServerRegistry registry,
            Func<DocumentFormattingParams, Task<TextEditContainer>> handler,
            DocumentFormattingRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentFormattingRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DocumentFormatting,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentFormattingParams, TextEditContainer,
                    DocumentFormattingRegistrationOptions>(handler, registrationOptions));
        }
    }
}
