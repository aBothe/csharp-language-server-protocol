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
    [Serial, Method(TextDocumentNames.RangeFormatting)]
    public interface IDocumentRangeFormattingHandler : IJsonRpcRequestHandler<DocumentRangeFormattingParams, TextEditContainer>, IRegistration<DocumentRangeFormattingRegistrationOptions>, ICapability<DocumentRangeFormattingCapability> { }

    public abstract class DocumentRangeFormattingHandler : IDocumentRangeFormattingHandler
    {
        private readonly DocumentRangeFormattingRegistrationOptions _options;
        public DocumentRangeFormattingHandler(DocumentRangeFormattingRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public DocumentRangeFormattingRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<TextEditContainer> Handle(DocumentRangeFormattingParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DocumentRangeFormattingCapability capability) => Capability = capability;
        protected DocumentRangeFormattingCapability Capability { get; private set; }
    }

    public static class DocumentRangeFormattingExtensions
    {
        public static IDisposable OnDocumentRangeFormatting(
            this ILanguageServerRegistry registry,
            Func<DocumentRangeFormattingParams, DocumentRangeFormattingCapability, CancellationToken, Task<TextEditContainer>>
                handler,
            DocumentRangeFormattingRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentRangeFormattingRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.RangeFormatting,
                new LanguageProtocolDelegatingHandlers.Request<DocumentRangeFormattingParams, TextEditContainer, DocumentRangeFormattingCapability,
                    DocumentRangeFormattingRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDocumentRangeFormatting(
            this ILanguageServerRegistry registry,
            Func<DocumentRangeFormattingParams, CancellationToken, Task<TextEditContainer>> handler,
            DocumentRangeFormattingRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentRangeFormattingRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.RangeFormatting,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentRangeFormattingParams, TextEditContainer,
                    DocumentRangeFormattingRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDocumentRangeFormatting(
            this ILanguageServerRegistry registry,
            Func<DocumentRangeFormattingParams, Task<TextEditContainer>> handler,
            DocumentRangeFormattingRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentRangeFormattingRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.RangeFormatting,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentRangeFormattingParams, TextEditContainer,
                    DocumentRangeFormattingRegistrationOptions>(handler, registrationOptions));
        }
    }
}
