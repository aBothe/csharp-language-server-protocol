using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Workspace
{
    [Parallel, Method(WorkspaceNames.WorkspaceSymbol, Direction.ClientToServer)]
    public interface IWorkspaceSymbolsHandler : IJsonRpcRequestHandler<WorkspaceSymbolParams, Container<SymbolInformation>>, ICapability<WorkspaceSymbolCapability>, IRegistration<WorkspaceSymbolRegistrationOptions> { }

    public abstract class WorkspaceSymbolsHandler : IWorkspaceSymbolsHandler
    {
        protected WorkspaceSymbolCapability Capability { get; private set; }
        private readonly WorkspaceSymbolRegistrationOptions _options;
        protected IWorkDoneProgressManager ProgressManager { get; }
        public WorkspaceSymbolsHandler(WorkspaceSymbolRegistrationOptions registrationOptions, IWorkDoneProgressManager progressManager)
        {
            _options = registrationOptions;
            ProgressManager = progressManager;
        }

        public WorkspaceSymbolRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Container<SymbolInformation>> Handle(WorkspaceSymbolParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(WorkspaceSymbolCapability capability) => Capability = capability;
    }

    public static class WorkspaceSymbolsExtensions
    {
        public static IDisposable OnWorkspaceSymbols(
            this ILanguageServerRegistry registry,
            Func<WorkspaceSymbolParams, WorkspaceSymbolCapability, CancellationToken, Task<Container<SymbolInformation>>>
                handler,
            WorkspaceSymbolRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new WorkspaceSymbolRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.WorkspaceSymbol,
                new LanguageProtocolDelegatingHandlers.Request<WorkspaceSymbolParams, Container<SymbolInformation>, WorkspaceSymbolCapability,
                    WorkspaceSymbolRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnWorkspaceSymbols(
            this ILanguageServerRegistry registry,
            Func<WorkspaceSymbolParams, WorkspaceSymbolCapability, Task<Container<SymbolInformation>>> handler,
            WorkspaceSymbolRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new WorkspaceSymbolRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.WorkspaceSymbol,
                new LanguageProtocolDelegatingHandlers.Request<WorkspaceSymbolParams, Container<SymbolInformation>, WorkspaceSymbolCapability,
                    WorkspaceSymbolRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnWorkspaceSymbols(
            this ILanguageServerRegistry registry,
            Func<WorkspaceSymbolParams, CancellationToken, Task<Container<SymbolInformation>>> handler,
            WorkspaceSymbolRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new WorkspaceSymbolRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.WorkspaceSymbol,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<WorkspaceSymbolParams, Container<SymbolInformation>,
                    WorkspaceSymbolRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnWorkspaceSymbols(
            this ILanguageServerRegistry registry,
            Func<WorkspaceSymbolParams, Task<Container<SymbolInformation>>> handler,
            WorkspaceSymbolRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new WorkspaceSymbolRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.WorkspaceSymbol,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<WorkspaceSymbolParams, Container<SymbolInformation>,
                    WorkspaceSymbolRegistrationOptions>(handler, registrationOptions));
        }

        public static Task<Container<SymbolInformation>> RequestWorkspaceSymbols(this IWorkspaceLanguageClient mediator, WorkspaceSymbolParams token, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(token, cancellationToken);
        }
    }
}
