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
    [Parallel, Method(TextDocumentNames.References)]
    public interface IReferencesHandler : IJsonRpcRequestHandler<ReferenceParams, LocationContainer>, IRegistration<ReferenceRegistrationOptions>, ICapability<ReferenceCapability> { }

    public abstract class ReferencesHandler : IReferencesHandler
    {
        private readonly ReferenceRegistrationOptions _options;
        protected IWorkDoneProgressManager ProgressManager { get; }
        public ReferencesHandler(ReferenceRegistrationOptions registrationOptions, IWorkDoneProgressManager progressManager)
        {
            _options = registrationOptions;
            ProgressManager = progressManager;
        }

        public ReferenceRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<LocationContainer> Handle(ReferenceParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(ReferenceCapability capability) => Capability = capability;
        protected ReferenceCapability Capability { get; private set; }
    }

    public static class ReferencesHandlerExtensions
    {
        public static IDisposable OnReferences(
            this ILanguageServerRegistry registry,
            Func<ReferenceParams, CancellationToken, Task<LocationContainer>> handler,
            ReferenceRegistrationOptions registrationOptions = null,
            Action<ReferenceCapability> setCapability = null)
        {
            registrationOptions ??= new ReferenceRegistrationOptions();
            setCapability ??= x => { };
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : ReferencesHandler
        {
            private readonly Func<ReferenceParams, CancellationToken, Task<LocationContainer>> _handler;
            private readonly Action<ReferenceCapability> _setCapability;

            public DelegatingHandler(
                Func<ReferenceParams, CancellationToken, Task<LocationContainer>> handler,
                IWorkDoneProgressManager progressManager,
                Action<ReferenceCapability> setCapability,
                ReferenceRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<LocationContainer> Handle(ReferenceParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(ReferenceCapability capability) => _setCapability?.Invoke(capability);
        }
    }
}
