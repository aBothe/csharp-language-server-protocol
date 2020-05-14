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
    [Parallel, Method(TextDocumentNames.Implementation)]
    public interface IImplementationHandler : IJsonRpcRequestHandler<ImplementationParams, LocationOrLocationLinks>,
        IRegistration<ImplementationRegistrationOptions>, ICapability<ImplementationCapability>
    {
    }

    public abstract class ImplementationHandler : IImplementationHandler
    {
        private readonly ImplementationRegistrationOptions _options;
        protected IWorkDoneProgressManager ProgressManager { get; }

        public ImplementationHandler(ImplementationRegistrationOptions registrationOptions,
            IWorkDoneProgressManager progressManager)
        {
            _options = registrationOptions;
            ProgressManager = progressManager;
        }

        public ImplementationRegistrationOptions GetRegistrationOptions() => _options;

        public abstract Task<LocationOrLocationLinks> Handle(ImplementationParams request,
            CancellationToken cancellationToken);

        public virtual void SetCapability(ImplementationCapability capability) => Capability = capability;
        protected ImplementationCapability Capability { get; private set; }
    }

    public static class ImplementationHandlerExtensions
    {
        public static IDisposable OnImplementation(
            this ILanguageServerRegistry registry,
            Func<ImplementationParams, CancellationToken, Task<LocationOrLocationLinks>> handler,
            ImplementationRegistrationOptions registrationOptions = null,
            Action<ImplementationCapability> setCapability = null)
        {
            registrationOptions ??= new ImplementationRegistrationOptions();
            setCapability ??= x => { };
            return registry.AddHandler(TextDocumentNames.Implementation,
                _ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler, setCapability,
                    registrationOptions));
        }

        class DelegatingHandler : ImplementationHandler
        {
            private readonly Func<ImplementationParams, CancellationToken, Task<LocationOrLocationLinks>> _handler;
            private readonly Action<ImplementationCapability> _setCapability;

            public DelegatingHandler(
                Func<ImplementationParams, CancellationToken, Task<LocationOrLocationLinks>> handler,
                IWorkDoneProgressManager progressManager,
                Action<ImplementationCapability> setCapability,
                ImplementationRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<LocationOrLocationLinks> Handle(ImplementationParams request,
                CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);

            public override void SetCapability(ImplementationCapability capability) =>
                _setCapability?.Invoke(capability);
        }
    }
}
