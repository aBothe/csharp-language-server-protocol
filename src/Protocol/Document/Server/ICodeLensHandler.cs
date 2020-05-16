using System;
using System.Reactive.Disposables;
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
    [Parallel, Method(TextDocumentNames.CodeLens)]
    public interface ICodeLensHandler : IJsonRpcRequestHandler<CodeLensParams, CodeLensContainer>, IRegistration<CodeLensRegistrationOptions>, ICapability<CodeLensCapability> { }

    [Parallel, Method(TextDocumentNames.CodeLensResolve)]
    public interface ICodeLensResolveHandler : ICanBeResolvedHandler<CodeLens> { }

    public abstract class CodeLensHandler : ICodeLensHandler, ICodeLensResolveHandler
    {
        private readonly CodeLensRegistrationOptions _options;
        protected IWorkDoneProgressManager ProgressManager { get; }
        public CodeLensHandler(CodeLensRegistrationOptions registrationOptions, IWorkDoneProgressManager progressManager)
        {
            _options = registrationOptions;
            ProgressManager = progressManager;
        }

        public CodeLensRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<CodeLensContainer> Handle(CodeLensParams request, CancellationToken cancellationToken);
        public abstract Task<CodeLens> Handle(CodeLens request, CancellationToken cancellationToken);
        public abstract bool CanResolve(CodeLens value);
        public virtual void SetCapability(CodeLensCapability capability) => Capability = capability;
        protected CodeLensCapability Capability { get; private set; }
    }

    public static class CodeLensExtensions
    {
        public static IDisposable OnCodeLens(
            this ILanguageServerRegistry registry,
            Func<CodeLensParams, CodeLensCapability, CancellationToken, Task<CodeLensContainer>> handler,
            CodeLensRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeLensRegistrationOptions();

            return registry.AddHandler(TextDocumentNames.CodeLens,
                new LanguageProtocolDelegatingHandlers.Request<CodeLensParams, CodeLensContainer, CodeLensCapability,
                    CodeLensRegistrationOptions>(
                    handler,
                    registrationOptions));
        }

        public static IDisposable OnCodeLens(
            this ILanguageServerRegistry registry,
            Func<CodeLensParams, CodeLensCapability, CancellationToken, Task<CodeLensContainer>> handler,
            Func<CodeLens, bool> canResolve ,
            Func<CodeLens, CodeLensCapability, CancellationToken, Task<CodeLens>> resolveHandler,
            CodeLensRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeLensRegistrationOptions();
            registrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            canResolve ??= item => registrationOptions.ResolveProvider;
            resolveHandler ??= (link, cap, token) => Task.FromException<CodeLens>(new NotImplementedException());

            return new CompositeDisposable(
                registry.AddHandler(TextDocumentNames.CodeLens,
                    new LanguageProtocolDelegatingHandlers.Request<CodeLensParams, CodeLensContainer, CodeLensCapability,
                        CodeLensRegistrationOptions>(
                        handler,
                        registrationOptions)),
                registry.AddHandler(TextDocumentNames.CodeLensResolve,
                    new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeLens, CodeLensCapability, CodeLensRegistrationOptions>(
                        resolveHandler,
                        canResolve,
                        registrationOptions))
            );
        }
        public static IDisposable OnCodeLens(
            this ILanguageServerRegistry registry,
            Func<CodeLensParams, CancellationToken, Task<CodeLensContainer>> handler,
            CodeLensRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeLensRegistrationOptions();

            return registry.AddHandler(TextDocumentNames.CodeLens,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<CodeLensParams, CodeLensContainer,
                    CodeLensRegistrationOptions>(
                    handler,
                    registrationOptions));
        }

        public static IDisposable OnCodeLens(
            this ILanguageServerRegistry registry,
            Func<CodeLensParams, CancellationToken, Task<CodeLensContainer>> handler,
            Func<CodeLens, bool> canResolve ,
            Func<CodeLens, CancellationToken, Task<CodeLens>> resolveHandler,
            CodeLensRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeLensRegistrationOptions();
            registrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            canResolve ??= item => registrationOptions.ResolveProvider;
            resolveHandler ??= (link, token) => Task.FromException<CodeLens>(new NotImplementedException());

            return new CompositeDisposable(
                registry.AddHandler(TextDocumentNames.CodeLens,
                    new LanguageProtocolDelegatingHandlers.RequestRegistration<CodeLensParams, CodeLensContainer,
                        CodeLensRegistrationOptions>(
                        handler,
                        registrationOptions)),
                registry.AddHandler(TextDocumentNames.CodeLensResolve,
                    new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeLens, CodeLensRegistrationOptions>(
                        resolveHandler,
                        canResolve,
                        registrationOptions))
            );
        }
        public static IDisposable OnCodeLens(
            this ILanguageServerRegistry registry,
            Func<CodeLensParams, Task<CodeLensContainer>> handler,
            CodeLensRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeLensRegistrationOptions();

            return registry.AddHandler(TextDocumentNames.CodeLens,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<CodeLensParams, CodeLensContainer,
                    CodeLensRegistrationOptions>(
                    handler,
                    registrationOptions));
        }

        public static IDisposable OnCodeLens(
            this ILanguageServerRegistry registry,
            Func<CodeLensParams, Task<CodeLensContainer>> handler,
            Func<CodeLens, bool> canResolve ,
            Func<CodeLens, Task<CodeLens>> resolveHandler,
            CodeLensRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeLensRegistrationOptions();
            registrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            canResolve ??= item => registrationOptions.ResolveProvider;
            resolveHandler ??= (link) => Task.FromException<CodeLens>(new NotImplementedException());

            return new CompositeDisposable(
                registry.AddHandler(TextDocumentNames.CodeLens,
                    new LanguageProtocolDelegatingHandlers.RequestRegistration<CodeLensParams, CodeLensContainer,
                        CodeLensRegistrationOptions>(
                        handler,
                        registrationOptions)),
                registry.AddHandler(TextDocumentNames.CodeLensResolve,
                    new LanguageProtocolDelegatingHandlers.CanBeResolved<CodeLens, CodeLensRegistrationOptions>(
                        resolveHandler,
                        canResolve,
                        registrationOptions))
            );
        }
    }
}
