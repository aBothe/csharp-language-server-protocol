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
            Func<DocumentHighlightParams, CancellationToken, Task<DocumentHighlightContainer>> handler,
            DocumentHighlightRegistrationOptions registrationOptions = null,
            Action<DocumentHighlightCapability> setCapability = null)
        {
            registrationOptions ??= new DocumentHighlightRegistrationOptions();
            setCapability ??= x => { };
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : DocumentHighlightHandler
        {
            private readonly Func<DocumentHighlightParams, CancellationToken, Task<DocumentHighlightContainer>> _handler;
            private readonly Action<DocumentHighlightCapability> _setCapability;

            public DelegatingHandler(
                Func<DocumentHighlightParams, CancellationToken, Task<DocumentHighlightContainer>> handler,
                IWorkDoneProgressManager progressManager,
                Action<DocumentHighlightCapability> setCapability,
                DocumentHighlightRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<DocumentHighlightContainer> Handle(DocumentHighlightParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(DocumentHighlightCapability capability) => _setCapability?.Invoke(capability);
        }
    }
}
