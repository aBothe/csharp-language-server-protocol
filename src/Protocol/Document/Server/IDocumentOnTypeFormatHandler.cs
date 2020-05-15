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
    [Serial, Method(TextDocumentNames.OnTypeFormatting)]
    public interface IDocumentOnTypeFormatHandler : IJsonRpcRequestHandler<DocumentOnTypeFormattingParams, TextEditContainer>, IRegistration<DocumentOnTypeFormattingRegistrationOptions>, ICapability<DocumentOnTypeFormattingCapability> { }

    public abstract class DocumentOnTypeFormatHandler : IDocumentOnTypeFormatHandler
    {
        private readonly DocumentOnTypeFormattingRegistrationOptions _options;
        public DocumentOnTypeFormatHandler(DocumentOnTypeFormattingRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public DocumentOnTypeFormattingRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<TextEditContainer> Handle(DocumentOnTypeFormattingParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DocumentOnTypeFormattingCapability capability) => Capability = capability;
        protected DocumentOnTypeFormattingCapability Capability { get; private set; }
    }

    public static class DocumentOnTypeFormatHandlerExtensions
    {
        public static IDisposable OnTypeFormatting(
            this ILanguageServerRegistry registry,
            Func<DocumentOnTypeFormattingParams, DocumentOnTypeFormattingCapability, CancellationToken, Task<TextEditContainer>>
                handler,
            DocumentOnTypeFormattingRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentOnTypeFormattingRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.OnTypeFormatting,
                new LanguageProtocolDelegatingHandlers.Request<DocumentOnTypeFormattingParams, TextEditContainer, DocumentOnTypeFormattingCapability,
                    DocumentOnTypeFormattingRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnTypeFormatting(
            this ILanguageServerRegistry registry,
            Func<DocumentOnTypeFormattingParams, CancellationToken, Task<TextEditContainer>> handler,
            DocumentOnTypeFormattingRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentOnTypeFormattingRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.OnTypeFormatting,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentOnTypeFormattingParams, TextEditContainer,
                    DocumentOnTypeFormattingRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnTypeFormatting(
            this ILanguageServerRegistry registry,
            Func<DocumentOnTypeFormattingParams, Task<TextEditContainer>> handler,
            DocumentOnTypeFormattingRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentOnTypeFormattingRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.OnTypeFormatting,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentOnTypeFormattingParams, TextEditContainer,
                    DocumentOnTypeFormattingRegistrationOptions>(handler, registrationOptions));
        }
    }
}
