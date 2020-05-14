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
    [Parallel, Method(TextDocumentNames.TypeDefinition)]
    public interface ITypeDefinitionHandler : IJsonRpcRequestHandler<TypeDefinitionParams, LocationOrLocationLinks>, IRegistration<TypeDefinitionRegistrationOptions>, ICapability<TypeDefinitionCapability> { }

    public abstract class TypeDefinitionHandler : ITypeDefinitionHandler
    {
        private readonly TypeDefinitionRegistrationOptions _options;
        protected IWorkDoneProgressManager ProgressManager { get; }
        public TypeDefinitionHandler(TypeDefinitionRegistrationOptions registrationOptions, IWorkDoneProgressManager progressManager)
        {
            _options = registrationOptions;
            ProgressManager = progressManager;
        }

        public TypeDefinitionRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<LocationOrLocationLinks> Handle(TypeDefinitionParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(TypeDefinitionCapability capability) => Capability = capability;
        protected TypeDefinitionCapability Capability { get; private set; }
    }

    public static class TypeDefinitionHandlerExtensions
    {
        public static IDisposable OnTypeDefinition(
            this ILanguageServerRegistry registry,
            Func<TypeDefinitionParams, CancellationToken, Task<LocationOrLocationLinks>> handler,
            TypeDefinitionRegistrationOptions registrationOptions = null,
            Action<TypeDefinitionCapability> setCapability = null)
        {
            registrationOptions ??= new TypeDefinitionRegistrationOptions();
            setCapability ??= x => { };
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : TypeDefinitionHandler
        {
            private readonly Func<TypeDefinitionParams, CancellationToken, Task<LocationOrLocationLinks>> _handler;
            private readonly Action<TypeDefinitionCapability> _setCapability;

            public DelegatingHandler(
                Func<TypeDefinitionParams, CancellationToken, Task<LocationOrLocationLinks>> handler,
                IWorkDoneProgressManager progressManager,
                Action<TypeDefinitionCapability> setCapability,
                TypeDefinitionRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<LocationOrLocationLinks> Handle(TypeDefinitionParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(TypeDefinitionCapability capability) => _setCapability?.Invoke(capability);
        }
    }
}
