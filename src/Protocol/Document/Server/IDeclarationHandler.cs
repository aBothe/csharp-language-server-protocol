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

    public static class DeclarationExtensions
    {
        public static IDisposable OnDeclaration(
            this ILanguageServerRegistry registry,
            Func<DeclarationParams, DeclarationCapability, CancellationToken, Task<LocationOrLocationLinks>>
                handler,
            DeclarationRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DeclarationRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Declaration,
                new LanguageProtocolDelegatingHandlers.Request<DeclarationParams, LocationOrLocationLinks, DeclarationCapability,
                    DeclarationRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDeclaration(
            this ILanguageServerRegistry registry,
            Func<DeclarationParams, DeclarationCapability, Task<LocationOrLocationLinks>> handler,
            DeclarationRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DeclarationRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Declaration,
                new LanguageProtocolDelegatingHandlers.Request<DeclarationParams, LocationOrLocationLinks, DeclarationCapability,
                    DeclarationRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDeclaration(
            this ILanguageServerRegistry registry,
            Func<DeclarationParams, CancellationToken, Task<LocationOrLocationLinks>> handler,
            DeclarationRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DeclarationRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Declaration,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<DeclarationParams, LocationOrLocationLinks,
                    DeclarationRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDeclaration(
            this ILanguageServerRegistry registry,
            Func<DeclarationParams, Task<LocationOrLocationLinks>> handler,
            DeclarationRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DeclarationRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Declaration,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<DeclarationParams, LocationOrLocationLinks,
                    DeclarationRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDeclaration(
            this ILanguageServerRegistry registry,
            Action<DeclarationParams, IObserver<Container<LocationLink>>, DeclarationCapability,
                CancellationToken> handler,
            DeclarationRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DeclarationRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Declaration,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DeclarationParams, LocationOrLocationLinks,
                        LocationLink, DeclarationCapability, DeclarationRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>()));
        }

        public static IDisposable OnDeclaration(
            this ILanguageServerRegistry registry,
            Action<DeclarationParams, IObserver<Container<LocationLink>>, DeclarationCapability>
                handler,
            DeclarationRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DeclarationRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Declaration,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DeclarationParams, LocationOrLocationLinks,
                        LocationLink, DeclarationCapability, DeclarationRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>()));
        }

        public static IDisposable OnDeclaration(
            this ILanguageServerRegistry registry,
            Action<DeclarationParams, IObserver<Container<LocationLink>>, CancellationToken> handler,
            DeclarationRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DeclarationRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Declaration,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DeclarationParams, LocationOrLocationLinks,
                        LocationLink, DeclarationRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>()));
        }

        public static IDisposable OnDeclaration(
            this ILanguageServerRegistry registry,
            Action<DeclarationParams, IObserver<Container<LocationLink>>> handler,
            DeclarationRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DeclarationRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Declaration,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DeclarationParams, LocationOrLocationLinks,
                        LocationLink, DeclarationRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>()));
        }
    }
}
