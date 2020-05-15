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
    [Parallel, Method(TextDocumentNames.Hover)]
    public interface IHoverHandler : IJsonRpcRequestHandler<HoverParams, Hover>, IRegistration<HoverRegistrationOptions>, ICapability<HoverCapability> { }

    public abstract class HoverHandler : IHoverHandler
    {
        private readonly HoverRegistrationOptions _options;
        public HoverHandler(HoverRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public HoverRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Hover> Handle(HoverParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(HoverCapability capability) => Capability = capability;
        protected HoverCapability Capability { get; private set; }
    }

    public static class HoverHandlerExtensions
    {
        public static IDisposable OnHover(
            this ILanguageServerRegistry registry,
            Func<HoverParams, HoverCapability, CancellationToken, Task<Hover>>
                handler,
            HoverRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new HoverRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Hover,
                new LanguageProtocolDelegatingHandlers.Request<HoverParams, Hover, HoverCapability,
                    HoverRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnHover(
            this ILanguageServerRegistry registry,
            Func<HoverParams, CancellationToken, Task<Hover>> handler,
            HoverRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new HoverRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Hover,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<HoverParams, Hover,
                    HoverRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnHover(
            this ILanguageServerRegistry registry,
            Func<HoverParams, Task<Hover>> handler,
            HoverRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new HoverRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Hover,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<HoverParams, Hover,
                    HoverRegistrationOptions>(handler, registrationOptions));
        }
    }
}
