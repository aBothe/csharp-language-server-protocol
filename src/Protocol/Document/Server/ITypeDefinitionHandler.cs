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

    public static class TypeDefinitionExtensions
    {
        public static IDisposable OnTypeDefinition(
            this ILanguageServerRegistry registry,
            Func<TypeDefinitionParams, TypeDefinitionCapability, CancellationToken, Task<LocationOrLocationLinks>>
                handler,
            TypeDefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TypeDefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.TypeDefinition,
                new LanguageProtocolDelegatingHandlers.Request<TypeDefinitionParams, LocationOrLocationLinks, TypeDefinitionCapability,
                    TypeDefinitionRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnTypeDefinition(
            this ILanguageServerRegistry registry,
            Func<TypeDefinitionParams, CancellationToken, Task<LocationOrLocationLinks>> handler,
            TypeDefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TypeDefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.TypeDefinition,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<TypeDefinitionParams, LocationOrLocationLinks,
                    TypeDefinitionRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnTypeDefinition(
            this ILanguageServerRegistry registry,
            Func<TypeDefinitionParams, Task<LocationOrLocationLinks>> handler,
            TypeDefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TypeDefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.TypeDefinition,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<TypeDefinitionParams, LocationOrLocationLinks,
                    TypeDefinitionRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnTypeDefinition(
            this ILanguageServerRegistry registry,
            Action<TypeDefinitionParams, IObserver<Container<LocationOrLocationLink>>, TypeDefinitionCapability,
                CancellationToken> handler,
            TypeDefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TypeDefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.TypeDefinition,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<TypeDefinitionParams, LocationOrLocationLinks,
                        LocationOrLocationLink, TypeDefinitionCapability, TypeDefinitionRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>()));
        }

        public static IDisposable OnTypeDefinition(
            this ILanguageServerRegistry registry,
            Action<TypeDefinitionParams, IObserver<Container<LocationOrLocationLink>>, TypeDefinitionCapability>
                handler,
            TypeDefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TypeDefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.TypeDefinition,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<TypeDefinitionParams, LocationOrLocationLinks,
                        LocationOrLocationLink, TypeDefinitionCapability, TypeDefinitionRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>()));
        }

        public static IDisposable OnTypeDefinition(
            this ILanguageServerRegistry registry,
            Action<TypeDefinitionParams, IObserver<Container<LocationOrLocationLink>>, CancellationToken> handler,
            TypeDefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TypeDefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.TypeDefinition,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<TypeDefinitionParams, LocationOrLocationLinks,
                        LocationOrLocationLink, TypeDefinitionRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>()));
        }

        public static IDisposable OnTypeDefinition(
            this ILanguageServerRegistry registry,
            Action<TypeDefinitionParams, IObserver<Container<LocationOrLocationLink>>> handler,
            TypeDefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TypeDefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.TypeDefinition,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<TypeDefinitionParams, LocationOrLocationLinks,
                        LocationOrLocationLink, TypeDefinitionRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>()));
        }
    }
}
