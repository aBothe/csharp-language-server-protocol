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

    public static class ReferencesExtensions
    {
        public static IDisposable OnReferences(
            this ILanguageServerRegistry registry,
            Func<ReferenceParams, ReferenceCapability, CancellationToken, Task<LocationContainer>>
                handler,
            ReferenceRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new ReferenceRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.References,
                new LanguageProtocolDelegatingHandlers.Request<ReferenceParams, LocationContainer, ReferenceCapability,
                    ReferenceRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnReferences(
            this ILanguageServerRegistry registry,
            Func<ReferenceParams, CancellationToken, Task<LocationContainer>> handler,
            ReferenceRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new ReferenceRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.References,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<ReferenceParams, LocationContainer,
                    ReferenceRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnReferences(
            this ILanguageServerRegistry registry,
            Func<ReferenceParams, Task<LocationContainer>> handler,
            ReferenceRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new ReferenceRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.References,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<ReferenceParams, LocationContainer,
                    ReferenceRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnReferences(
            this ILanguageServerRegistry registry,
            Action<ReferenceParams, IObserver<Container<Location>>, ReferenceCapability,
                CancellationToken> handler,
            ReferenceRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new ReferenceRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.References,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<ReferenceParams, LocationContainer,
                        Location, ReferenceCapability, ReferenceRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>()));
        }

        public static IDisposable OnReferences(
            this ILanguageServerRegistry registry,
            Action<ReferenceParams, IObserver<Container<Location>>, ReferenceCapability>
                handler,
            ReferenceRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new ReferenceRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.References,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<ReferenceParams, LocationContainer,
                        Location, ReferenceCapability, ReferenceRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>()));
        }

        public static IDisposable OnReferences(
            this ILanguageServerRegistry registry,
            Action<ReferenceParams, IObserver<Container<Location>>, CancellationToken> handler,
            ReferenceRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new ReferenceRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.References,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<ReferenceParams, LocationContainer,
                        Location, ReferenceRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>()));
        }

        public static IDisposable OnReferences(
            this ILanguageServerRegistry registry,
            Action<ReferenceParams, IObserver<Container<Location>>> handler,
            ReferenceRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new ReferenceRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.References,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<ReferenceParams, LocationContainer,
                        Location, ReferenceRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>()));
        }
    }
}
