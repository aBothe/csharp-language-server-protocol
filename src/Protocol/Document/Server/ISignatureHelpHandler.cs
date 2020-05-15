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
    [Parallel, Method(TextDocumentNames.SignatureHelp)]
    public interface ISignatureHelpHandler : IJsonRpcRequestHandler<SignatureHelpParams, SignatureHelp>, IRegistration<SignatureHelpRegistrationOptions>, ICapability<SignatureHelpCapability> { }

    public abstract class SignatureHelpHandler : ISignatureHelpHandler
    {
        private readonly SignatureHelpRegistrationOptions _options;
        public SignatureHelpHandler(SignatureHelpRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public SignatureHelpRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<SignatureHelp> Handle(SignatureHelpParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(SignatureHelpCapability capability) => Capability = capability;
        protected SignatureHelpCapability Capability { get; private set; }
    }

    public static class SignatureHelpHandlerExtensions
    {
        public static IDisposable OnSignatureHelp(
            this ILanguageServerRegistry registry,
            Func<SignatureHelpParams, SignatureHelpCapability, CancellationToken, Task<SignatureHelp>>
                handler,
            SignatureHelpRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new SignatureHelpRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.SignatureHelp,
                new LanguageProtocolDelegatingHandlers.Request<SignatureHelpParams, SignatureHelp, SignatureHelpCapability,
                    SignatureHelpRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnSignatureHelp(
            this ILanguageServerRegistry registry,
            Func<SignatureHelpParams, CancellationToken, Task<SignatureHelp>> handler,
            SignatureHelpRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new SignatureHelpRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.SignatureHelp,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<SignatureHelpParams, SignatureHelp,
                    SignatureHelpRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnSignatureHelp(
            this ILanguageServerRegistry registry,
            Func<SignatureHelpParams, Task<SignatureHelp>> handler,
            SignatureHelpRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new SignatureHelpRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.SignatureHelp,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<SignatureHelpParams, SignatureHelp,
                    SignatureHelpRegistrationOptions>(handler, registrationOptions));
        }
    }
}
