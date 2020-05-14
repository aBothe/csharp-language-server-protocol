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
        public static IDisposable OnDocumentOnTypeFormat(
            this ILanguageServerRegistry registry,
            Func<DocumentOnTypeFormattingParams, CancellationToken, Task<TextEditContainer>> handler,
            DocumentOnTypeFormattingRegistrationOptions registrationOptions = null,
            Action<DocumentOnTypeFormattingCapability> setCapability = null)
        {
            registrationOptions ??= new DocumentOnTypeFormattingRegistrationOptions();
            setCapability ??= x => { };
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : DocumentOnTypeFormatHandler
        {
            private readonly Func<DocumentOnTypeFormattingParams, CancellationToken, Task<TextEditContainer>> _handler;
            private readonly Action<DocumentOnTypeFormattingCapability> _setCapability;

            public DelegatingHandler(
                Func<DocumentOnTypeFormattingParams, CancellationToken, Task<TextEditContainer>> handler,
                Action<DocumentOnTypeFormattingCapability> setCapability,
                DocumentOnTypeFormattingRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<TextEditContainer> Handle(DocumentOnTypeFormattingParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(DocumentOnTypeFormattingCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
