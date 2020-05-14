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
    [Parallel, Method(TextDocumentNames.CodeAction)]
    public interface ICodeActionHandler : IJsonRpcRequestHandler<CodeActionParams, CommandOrCodeActionContainer>,
        IRegistration<CodeActionRegistrationOptions>, ICapability<CodeActionCapability>
    {
    }

    public abstract class CodeActionHandler : ICodeActionHandler
    {
        private readonly CodeActionRegistrationOptions _options;
        protected IWorkDoneProgressManager ProgressManager { get; }

        public CodeActionHandler(CodeActionRegistrationOptions registrationOptions,
            IWorkDoneProgressManager progressManager)
        {
            _options = registrationOptions;
            ProgressManager = progressManager;
        }

        public CodeActionRegistrationOptions GetRegistrationOptions() => _options;

        public abstract Task<CommandOrCodeActionContainer> Handle(CodeActionParams request,
            CancellationToken cancellationToken);

        public virtual void SetCapability(CodeActionCapability capability) => Capability = capability;
        protected CodeActionCapability Capability { get; private set; }
    }

    public static class CodeActionHandlerExtensions
    {
        public static IDisposable OnCodeAction(
            this ILanguageServerRegistry registry,
            Func<CodeActionParams, CancellationToken, Task<CommandOrCodeActionContainer>> handler,
            CodeActionRegistrationOptions registrationOptions = null,
            Action<CodeActionCapability> setCapability = null)
        {
            registrationOptions ??= new CodeActionRegistrationOptions();
            setCapability ??= x => { };
            return registry.AddHandler(_ =>
                ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler, setCapability, registrationOptions));
        }

        internal class DelegatingHandler : CodeActionHandler
        {
            private readonly Func<CodeActionParams, CancellationToken, Task<CommandOrCodeActionContainer>> _handler;
            private readonly Action<CodeActionCapability> _setCapability;

            public DelegatingHandler(
                Func<CodeActionParams, CancellationToken, Task<CommandOrCodeActionContainer>> handler,
                IWorkDoneProgressManager progressManager,
                Action<CodeActionCapability> setCapability,
                CodeActionRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<CommandOrCodeActionContainer> Handle(CodeActionParams request,
                CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);

            public override void SetCapability(CodeActionCapability capability) => _setCapability?.Invoke(capability);
        }
    }
}
