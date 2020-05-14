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
            Func<CompletionParams, CancellationToken, Task<CompletionList>> handler,
            Func<CompletionItem, CancellationToken, Task<CompletionItem>> resolveHandler = null,
            Func<CompletionItem, bool> canResolve = null,
            CompletionRegistrationOptions registrationOptions = null,
            Action<CompletionCapability> setCapability = null)
        {
            registrationOptions ??= new CompletionRegistrationOptions();
            registrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            setCapability ??= x => { };
            canResolve ??= item => registrationOptions.ResolveProvider;
            resolveHandler ??= (link, token) => Task.FromException<CompletionItem>(new NotImplementedException());
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler, resolveHandler, canResolve, setCapability, registrationOptions));
        }

        class DelegatingHandler : CompletionHandler
        {
            private readonly Func<CompletionParams, CancellationToken, Task<CompletionList>> _handler;
            private readonly Func<CompletionItem, CancellationToken, Task<CompletionItem>> _resolveHandler;
            private readonly Func<CompletionItem, bool> _canResolve;
            private readonly Action<CompletionCapability> _setCapability;

            public DelegatingHandler(
                Func<CompletionParams, CancellationToken, Task<CompletionList>> handler,
                Func<CompletionItem, CancellationToken, Task<CompletionItem>> resolveHandler,
                IWorkDoneProgressManager progressManager,
                Func<CompletionItem, bool> canResolve,
                Action<CompletionCapability> setCapability,
                CompletionRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _resolveHandler = resolveHandler;
                _canResolve = canResolve;
                _setCapability = setCapability;
            }

            public override Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override Task<CompletionItem> Handle(CompletionItem request, CancellationToken cancellationToken) => _resolveHandler.Invoke(request, cancellationToken);
            public override bool CanResolve(CompletionItem value) => _canResolve.Invoke(value);
            public override void SetCapability(CompletionCapability capability) => _setCapability?.Invoke(capability);
        }
    }
}
