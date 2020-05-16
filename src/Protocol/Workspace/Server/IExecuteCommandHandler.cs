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

    public static class ExecuteCommandExtensions
    {
        public static IDisposable OnExecuteCommand(
            this ILanguageServerRegistry registry,
            Func<ExecuteCommandParams, ExecuteCommandCapability, CancellationToken, Task>
                handler,
            ExecuteCommandRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new ExecuteCommandRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.ExecuteCommand,
                new LanguageProtocolDelegatingHandlers.Request<ExecuteCommandParams, ExecuteCommandCapability,
                    ExecuteCommandRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnExecuteCommand(
            this ILanguageServerRegistry registry,
            Func<ExecuteCommandParams, ExecuteCommandCapability, Task> handler,
            ExecuteCommandRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new ExecuteCommandRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.ExecuteCommand,
                new LanguageProtocolDelegatingHandlers.Request<ExecuteCommandParams, ExecuteCommandCapability,
                    ExecuteCommandRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnExecuteCommand(
            this ILanguageServerRegistry registry,
            Func<ExecuteCommandParams, CancellationToken, Task> handler,
            ExecuteCommandRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new ExecuteCommandRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.ExecuteCommand,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<ExecuteCommandParams,
                    ExecuteCommandRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnExecuteCommand(
            this ILanguageServerRegistry registry,
            Func<ExecuteCommandParams, Task> handler,
            ExecuteCommandRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new ExecuteCommandRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.ExecuteCommand,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<ExecuteCommandParams,
                    ExecuteCommandRegistrationOptions>(handler, registrationOptions));
        }
    }
}
