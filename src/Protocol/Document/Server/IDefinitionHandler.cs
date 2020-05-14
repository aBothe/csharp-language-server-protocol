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
    [Parallel, Method(TextDocumentNames.Definition)]
    public interface IDefinitionHandler : IJsonRpcRequestHandler<DefinitionParams, LocationOrLocationLinks>, IRegistration<DefinitionRegistrationOptions>, ICapability<DefinitionCapability> { }

    public abstract class DefinitionHandler : IDefinitionHandler
    {
        private readonly DefinitionRegistrationOptions _options;
        protected IWorkDoneProgressManager ProgressManager { get; }
        public DefinitionHandler(DefinitionRegistrationOptions registrationOptions, IWorkDoneProgressManager progressManager)
        {
            _options = registrationOptions;
            ProgressManager = ProgressManager;
        }

        public DefinitionRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<LocationOrLocationLinks> Handle(DefinitionParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DefinitionCapability capability) => Capability = capability;
        protected DefinitionCapability Capability { get; private set; }
    }

    public static class DefinitionHandlerExtensions
    {
        public static IDisposable OnDefinition(
            this ILanguageServerRegistry registry,
            Func<DefinitionParams, CancellationToken, Task<LocationOrLocationLinks>> handler,
            DefinitionRegistrationOptions registrationOptions = null,
            Action<DefinitionCapability> setCapability = null)
        {
            registrationOptions ??= new DefinitionRegistrationOptions();
            setCapability ??= x => { };
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : DefinitionHandler
        {
            private readonly Func<DefinitionParams, CancellationToken, Task<LocationOrLocationLinks>> _handler;
            private readonly Action<DefinitionCapability> _setCapability;

            public DelegatingHandler(
                Func<DefinitionParams, CancellationToken, Task<LocationOrLocationLinks>> handler,
                IWorkDoneProgressManager progressManager,
                Action<DefinitionCapability> setCapability,
                DefinitionRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<LocationOrLocationLinks> Handle(DefinitionParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(DefinitionCapability capability) => _setCapability?.Invoke(capability);
        }
    }
}
