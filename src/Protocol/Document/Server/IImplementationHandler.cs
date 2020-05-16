using System;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
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

    public abstract class ImplementationHandler :
        AbstractHandlers.Request<ImplementationParams, LocationOrLocationLinks, ImplementationCapability,
            ImplementationRegistrationOptions>, IImplementationHandler
    {
        protected ImplementationHandler(ImplementationRegistrationOptions registrationOptions) : base(
            registrationOptions)
        {
        }
    }

    public static class ImplementationExtensions
    {
        public static IDisposable OnImplementation(
            this ILanguageServerRegistry registry,
            Func<ImplementationParams, ImplementationCapability, CancellationToken, Task<LocationOrLocationLinks>>
                handler,
            ImplementationRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new ImplementationRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Implementation,
                new LanguageProtocolDelegatingHandlers.Request<ImplementationParams, LocationOrLocationLinks, ImplementationCapability,
                    ImplementationRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnImplementation(
            this ILanguageServerRegistry registry,
            Func<ImplementationParams, CancellationToken, Task<LocationOrLocationLinks>> handler,
            ImplementationRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new ImplementationRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Implementation,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<ImplementationParams, LocationOrLocationLinks,
                    ImplementationRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnImplementation(
            this ILanguageServerRegistry registry,
            Func<ImplementationParams, Task<LocationOrLocationLinks>> handler,
            ImplementationRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new ImplementationRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Implementation,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<ImplementationParams, LocationOrLocationLinks,
                    ImplementationRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnImplementation(
            this ILanguageServerRegistry registry,
            Action<ImplementationParams, IObserver<Container<LocationOrLocationLink>>, ImplementationCapability,
                CancellationToken> handler,
            ImplementationRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new ImplementationRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Implementation,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<ImplementationParams, LocationOrLocationLinks,
                        LocationOrLocationLink, ImplementationCapability, ImplementationRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>()));
        }

        public static IDisposable OnImplementation(
            this ILanguageServerRegistry registry,
            Action<ImplementationParams, IObserver<Container<LocationOrLocationLink>>, ImplementationCapability>
                handler,
            ImplementationRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new ImplementationRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Implementation,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<ImplementationParams, LocationOrLocationLinks,
                        LocationOrLocationLink, ImplementationCapability, ImplementationRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>()));
        }

        public static IDisposable OnImplementation(
            this ILanguageServerRegistry registry,
            Action<ImplementationParams, IObserver<Container<LocationOrLocationLink>>, CancellationToken> handler,
            ImplementationRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new ImplementationRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Implementation,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<ImplementationParams, LocationOrLocationLinks,
                        LocationOrLocationLink, ImplementationRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>()));
        }

        public static IDisposable OnImplementation(
            this ILanguageServerRegistry registry,
            Action<ImplementationParams, IObserver<Container<LocationOrLocationLink>>> handler,
            ImplementationRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new ImplementationRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Implementation,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<ImplementationParams, LocationOrLocationLinks,
                        LocationOrLocationLink, ImplementationRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>()));
        }
    }
}
