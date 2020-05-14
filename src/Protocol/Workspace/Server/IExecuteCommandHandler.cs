using System;
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
    [Serial, Method(WorkspaceNames.ExecuteCommand)]
    public interface IExecuteCommandHandler : IJsonRpcRequestHandler<ExecuteCommandParams>, IRegistration<ExecuteCommandRegistrationOptions>, ICapability<ExecuteCommandCapability> { }

    public abstract class ExecuteCommandHandler : IExecuteCommandHandler
    {
        private readonly ExecuteCommandRegistrationOptions _options;
        protected IWorkDoneProgressManager ProgressManager { get; }

        public ExecuteCommandHandler(ExecuteCommandRegistrationOptions registrationOptions, IWorkDoneProgressManager progressManager)
        {
            _options = registrationOptions;
            ProgressManager = progressManager;
        }

        public ExecuteCommandRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Unit> Handle(ExecuteCommandParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(ExecuteCommandCapability capability) => Capability = capability;
        protected ExecuteCommandCapability Capability { get; private set; }
    }

    public static class ExecuteCommandHandlerExtensions
    {
        public static IDisposable OnExecuteCommand(
            this ILanguageServerRegistry registry,
            Func<ExecuteCommandParams, CancellationToken, Task<Unit>> handler,
            ExecuteCommandRegistrationOptions registrationOptions = null,
            Action<ExecuteCommandCapability> setCapability = null)
        {
            registrationOptions ??= new ExecuteCommandRegistrationOptions();
            setCapability ??= x => { };
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : ExecuteCommandHandler
        {
            private readonly Func<ExecuteCommandParams, CancellationToken, Task<Unit>> _handler;
            private readonly Action<ExecuteCommandCapability> _setCapability;

            public DelegatingHandler(Func<ExecuteCommandParams, CancellationToken, Task<Unit>> handler,
                IWorkDoneProgressManager progressManager,
                Action<ExecuteCommandCapability> setCapability,
                ExecuteCommandRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Unit> Handle(ExecuteCommandParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(ExecuteCommandCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
