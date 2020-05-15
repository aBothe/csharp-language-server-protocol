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
    [Parallel, Method(TextDocumentNames.Completion)]
    public interface ICompletionHandler : IJsonRpcRequestHandler<CompletionParams, CompletionList>, IRegistration<CompletionRegistrationOptions>, ICapability<CompletionCapability> { }

    [Parallel, Method(TextDocumentNames.CompletionResolve)]
    public interface ICompletionResolveHandler : ICanBeResolvedHandler<CompletionItem> { }

    public abstract class CompletionHandler : ICompletionHandler, ICompletionResolveHandler
    {
        private readonly CompletionRegistrationOptions _options;
        protected IWorkDoneProgressManager ProgressManager { get; }
        public CompletionHandler(CompletionRegistrationOptions registrationOptions, IWorkDoneProgressManager progressManager)
        {
            _options = registrationOptions;
            ProgressManager = progressManager;
        }

        public CompletionRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken);
        public abstract Task<CompletionItem> Handle(CompletionItem request, CancellationToken cancellationToken);
        public abstract bool CanResolve(CompletionItem value);
        public virtual void SetCapability(CompletionCapability capability) => Capability = capability;
        protected CompletionCapability Capability { get; private set; }
    }

    public static class CompletionHandlerExtensions
    {
        public static IDisposable OnCompletion(
            this ILanguageServerRegistry registry,
            Func<CompletionParams, CompletionCapability, CancellationToken, Task<CompletionList>> handler,
            CompletionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CompletionRegistrationOptions();

            return registry.AddHandler(TextDocumentNames.Completion,
                new LanguageProtocolDelegatingHandlers.Request<CompletionParams, CompletionList, CompletionCapability,
                    CompletionRegistrationOptions>(
                    handler,
                    registrationOptions));
        }

        public static IDisposable OnCompletion(
            this ILanguageServerRegistry registry,
            Func<CompletionParams, CompletionCapability, CancellationToken, Task<CompletionList>> handler,
            Func<CompletionItem, bool> canResolve ,
            Func<CompletionItem, CompletionCapability, CancellationToken, Task<CompletionItem>> resolveHandler,
            CompletionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CompletionRegistrationOptions();
            registrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            canResolve ??= item => registrationOptions.ResolveProvider;
            resolveHandler ??= (link, cap, token) => Task.FromException<CompletionItem>(new NotImplementedException());

            return new CompositeDisposable(
                registry.AddHandler(TextDocumentNames.Completion,
                    new LanguageProtocolDelegatingHandlers.Request<CompletionParams, CompletionList, CompletionCapability,
                        CompletionRegistrationOptions>(
                        handler,
                        registrationOptions)),
                registry.AddHandler(TextDocumentNames.CompletionResolve,
                    new LanguageProtocolDelegatingHandlers.CanBeResolved<CompletionItem, CompletionCapability, CompletionRegistrationOptions>(
                        resolveHandler,
                        canResolve,
                        registrationOptions))
            );
        }

        public static IDisposable OnCompletion(
            this ILanguageServerRegistry registry,
            Func<CompletionParams, CancellationToken, Task<CompletionList>> handler,
            CompletionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CompletionRegistrationOptions();

            return registry.AddHandler(TextDocumentNames.Completion,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<CompletionParams, CompletionList,
                    CompletionRegistrationOptions>(
                    handler,
                    registrationOptions));
        }

        public static IDisposable OnCompletion(
            this ILanguageServerRegistry registry,
            Func<CompletionParams, CancellationToken, Task<CompletionList>> handler,
            Func<CompletionItem, bool> canResolve ,
            Func<CompletionItem, CancellationToken, Task<CompletionItem>> resolveHandler,
            CompletionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CompletionRegistrationOptions();
            registrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            canResolve ??= item => registrationOptions.ResolveProvider;
            resolveHandler ??= (link, token) => Task.FromException<CompletionItem>(new NotImplementedException());

            return new CompositeDisposable(
                registry.AddHandler(TextDocumentNames.Completion,
                    new LanguageProtocolDelegatingHandlers.RequestRegistration<CompletionParams, CompletionList,
                        CompletionRegistrationOptions>(
                        handler,
                        registrationOptions)),
                registry.AddHandler(TextDocumentNames.CompletionResolve,
                    new LanguageProtocolDelegatingHandlers.CanBeResolved<CompletionItem, CompletionRegistrationOptions>(
                        resolveHandler,
                        canResolve,
                        registrationOptions))
            );
        }
        public static IDisposable OnCompletion(
            this ILanguageServerRegistry registry,
            Func<CompletionParams, Task<CompletionList>> handler,
            CompletionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CompletionRegistrationOptions();

            return registry.AddHandler(TextDocumentNames.Completion,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<CompletionParams, CompletionList,
                    CompletionRegistrationOptions>(
                    handler,
                    registrationOptions));
        }

        public static IDisposable OnCompletion(
            this ILanguageServerRegistry registry,
            Func<CompletionParams, Task<CompletionList>> handler,
            Func<CompletionItem, bool> canResolve ,
            Func<CompletionItem, Task<CompletionItem>> resolveHandler,
            CompletionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CompletionRegistrationOptions();
            registrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            canResolve ??= item => registrationOptions.ResolveProvider;
            resolveHandler ??= (link) => Task.FromException<CompletionItem>(new NotImplementedException());

            return new CompositeDisposable(
                registry.AddHandler(TextDocumentNames.Completion,
                    new LanguageProtocolDelegatingHandlers.RequestRegistration<CompletionParams, CompletionList,
                        CompletionRegistrationOptions>(
                        handler,
                        registrationOptions)),
                registry.AddHandler(TextDocumentNames.CompletionResolve,
                    new LanguageProtocolDelegatingHandlers.CanBeResolved<CompletionItem, CompletionRegistrationOptions>(
                        resolveHandler,
                        canResolve,
                        registrationOptions))
            );
        }
    }
}
