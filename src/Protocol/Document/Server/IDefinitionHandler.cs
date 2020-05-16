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

    public static class DefinitionExtensions
    {
        public static IDisposable OnDefinition(
            this ILanguageServerRegistry registry,
            Func<DefinitionParams, DefinitionCapability, CancellationToken, Task<LocationOrLocationLinks>>
                handler,
            DefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Definition,
                new LanguageProtocolDelegatingHandlers.Request<DefinitionParams, LocationOrLocationLinks, DefinitionCapability,
                    DefinitionRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDefinition(
            this ILanguageServerRegistry registry,
            Func<DefinitionParams, CancellationToken, Task<LocationOrLocationLinks>> handler,
            DefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Definition,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<DefinitionParams, LocationOrLocationLinks,
                    DefinitionRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDefinition(
            this ILanguageServerRegistry registry,
            Func<DefinitionParams, Task<LocationOrLocationLinks>> handler,
            DefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Definition,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<DefinitionParams, LocationOrLocationLinks,
                    DefinitionRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDefinition(
            this ILanguageServerRegistry registry,
            Action<DefinitionParams, IObserver<Container<LocationOrLocationLink>>, DefinitionCapability,
                CancellationToken> handler,
            DefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Definition,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DefinitionParams, LocationOrLocationLinks,
                        LocationOrLocationLink, DefinitionCapability, DefinitionRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>()));
        }

        public static IDisposable OnDefinition(
            this ILanguageServerRegistry registry,
            Action<DefinitionParams, IObserver<Container<LocationOrLocationLink>>, DefinitionCapability>
                handler,
            DefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Definition,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DefinitionParams, LocationOrLocationLinks,
                        LocationOrLocationLink, DefinitionCapability, DefinitionRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>()));
        }

        public static IDisposable OnDefinition(
            this ILanguageServerRegistry registry,
            Action<DefinitionParams, IObserver<Container<LocationOrLocationLink>>, CancellationToken> handler,
            DefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Definition,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DefinitionParams, LocationOrLocationLinks,
                        LocationOrLocationLink, DefinitionRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>()));
        }

        public static IDisposable OnDefinition(
            this ILanguageServerRegistry registry,
            Action<DefinitionParams, IObserver<Container<LocationOrLocationLink>>> handler,
            DefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Definition,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DefinitionParams, LocationOrLocationLinks,
                        LocationOrLocationLink, DefinitionRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>()));
        }
    }
}
