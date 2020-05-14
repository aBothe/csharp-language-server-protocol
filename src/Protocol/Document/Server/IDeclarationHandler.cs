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
    [Parallel, Method(TextDocumentNames.Declaration)]
    public interface IDeclarationHandler : IJsonRpcRequestHandler<DeclarationParams, LocationOrLocationLinks>, IRegistration<DeclarationRegistrationOptions>, ICapability<DeclarationCapability> { }

    public abstract class DeclarationHandler : IDeclarationHandler
    {
        private readonly DeclarationRegistrationOptions _options;
        protected IWorkDoneProgressManager ProgressManager { get; }
        public DeclarationHandler(DeclarationRegistrationOptions registrationOptions, IWorkDoneProgressManager progressManager)
        {
            _options = registrationOptions;
            ProgressManager = progressManager;
        }

        public DeclarationRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<LocationOrLocationLinks> Handle(DeclarationParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DeclarationCapability capability) => Capability = capability;
        protected DeclarationCapability Capability { get; private set; }
    }

    public static class DeclarationHandlerExtensions
    {
        public static IDisposable OnDeclaration(
            this ILanguageServerRegistry registry,
            Func<DeclarationParams, CancellationToken, Task<LocationOrLocationLinks>> handler,
            DeclarationRegistrationOptions registrationOptions = null,
            Action<DeclarationCapability> setCapability = null)
        {
            registrationOptions ??= new DeclarationRegistrationOptions();
            setCapability ??= x => { };
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : DeclarationHandler
        {
            private readonly Func<DeclarationParams, CancellationToken, Task<LocationOrLocationLinks>> _handler;
            private readonly Action<DeclarationCapability> _setCapability;

            public DelegatingHandler(
                Func<DeclarationParams, CancellationToken, Task<LocationOrLocationLinks>> handler,
                IWorkDoneProgressManager progressManager,
                Action<DeclarationCapability> setCapability,
                DeclarationRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<LocationOrLocationLinks> Handle(DeclarationParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(DeclarationCapability capability) => _setCapability?.Invoke(capability);
        }
    }
}
