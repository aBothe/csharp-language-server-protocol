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
            Func<DocumentColorParams, CancellationToken, Task<Container<ColorPresentation>>> handler,
            DocumentColorRegistrationOptions registrationOptions = null,
            Action<ColorProviderCapability> setCapability = null)
        {
            registrationOptions ??= new DocumentColorRegistrationOptions();
            setCapability ??= x => { };
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : DocumentColorHandler
        {
            private readonly Func<DocumentColorParams, CancellationToken, Task<Container<ColorPresentation>>> _handler;
            private readonly Action<ColorProviderCapability> _setCapability;

            public DelegatingHandler(
                Func<DocumentColorParams, CancellationToken, Task<Container<ColorPresentation>>> handler,
                IWorkDoneProgressManager progressManager,
                Action<ColorProviderCapability> setCapability,
                DocumentColorRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Container<ColorPresentation>> Handle(DocumentColorParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(ColorProviderCapability capability) => _setCapability?.Invoke(capability);
        }
    }
}
