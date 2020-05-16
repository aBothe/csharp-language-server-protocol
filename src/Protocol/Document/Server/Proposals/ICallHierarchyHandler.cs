using System;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document.Server.Proposals
{
    [Obsolete(Constants.Proposal)]
    [Parallel, Method(TextDocumentNames.PrepareCallHierarchy)]
    public interface ICallHierarchyHandler :
        IJsonRpcRequestHandler<CallHierarchyPrepareParams, Container<CallHierarchyItem>>,
        IRegistration<CallHierarchyRegistrationOptions>, ICapability<CallHierarchyCapability>
    {
    }

    [Obsolete(Constants.Proposal)]
    [Parallel, Method(TextDocumentNames.CallHierarchyIncoming)]
    public interface ICallHierarchyIncomingHandler : IJsonRpcRequestHandler<CallHierarchyIncomingCallsParams,
            Container<CallHierarchyIncomingCall>>,
        IRegistration<CallHierarchyRegistrationOptions>, ICapability<CallHierarchyCapability>
    {
    }

    [Obsolete(Constants.Proposal)]
    [Parallel, Method(TextDocumentNames.CallHierarchyOutgoing)]
    public interface ICallHierarchyOutgoingHandler : IJsonRpcRequestHandler<CallHierarchyOutgoingCallsParams,
            Container<CallHierarchyOutgoingCall>>,
        IRegistration<CallHierarchyRegistrationOptions>, ICapability<CallHierarchyCapability>
    {
    }

    [Obsolete(Constants.Proposal)]
    public abstract class CallHierarchyHandler : ICallHierarchyHandler, ICallHierarchyIncomingHandler,
        ICallHierarchyOutgoingHandler
    {
        private readonly CallHierarchyRegistrationOptions _options;
        protected IWorkDoneProgressManager ProgressManager { get; }

        public CallHierarchyHandler(CallHierarchyRegistrationOptions registrationOptions,
            IWorkDoneProgressManager progressManager)
        {
            _options = registrationOptions;
            ProgressManager = progressManager;
        }

        public CallHierarchyRegistrationOptions GetRegistrationOptions() => _options;

        public abstract Task<Container<CallHierarchyItem>> Handle(CallHierarchyPrepareParams request,
            CancellationToken cancellationToken);

        public abstract Task<Container<CallHierarchyIncomingCall>> Handle(CallHierarchyIncomingCallsParams request,
            CancellationToken cancellationToken);

        public abstract Task<Container<CallHierarchyOutgoingCall>> Handle(CallHierarchyOutgoingCallsParams request,
            CancellationToken cancellationToken);

        public virtual void SetCapability(CallHierarchyCapability capability) => Capability = capability;
        protected CallHierarchyCapability Capability { get; private set; }
    }

    [Obsolete(Constants.Proposal)]
    public static class CallHierarchyExtensions
    {
        public static IDisposable OnCallHierarchy(
            this ILanguageServerRegistry registry,
            Func<CallHierarchyPrepareParams, CallHierarchyCapability, CancellationToken,
                    Task<Container<CallHierarchyItem>>>
                handler,
            Func<CallHierarchyIncomingCallsParams, CallHierarchyCapability, CancellationToken,
                    Task<Container<CallHierarchyIncomingCall>>>
                incomingHandler,
            Func<CallHierarchyOutgoingCallsParams, CallHierarchyCapability, CancellationToken,
                    Task<Container<CallHierarchyOutgoingCall>>>
                outgoingHandler,
            CallHierarchyRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CallHierarchyRegistrationOptions();
            return new CompositeDisposable(
                registry.AddHandler(TextDocumentNames.PrepareCallHierarchy,
                    new LanguageProtocolDelegatingHandlers.Request<CallHierarchyPrepareParams,
                        Container<CallHierarchyItem>,
                        CallHierarchyCapability,
                        CallHierarchyRegistrationOptions>(handler, registrationOptions)),
                registry.AddHandler(TextDocumentNames.CallHierarchyIncoming,
                    new LanguageProtocolDelegatingHandlers.Request<CallHierarchyIncomingCallsParams,
                        Container<CallHierarchyIncomingCall>,
                        CallHierarchyCapability,
                        CallHierarchyRegistrationOptions>(incomingHandler, registrationOptions)),
                registry.AddHandler(TextDocumentNames.CallHierarchyOutgoing,
                    new LanguageProtocolDelegatingHandlers.Request<CallHierarchyOutgoingCallsParams,
                        Container<CallHierarchyOutgoingCall>,
                        CallHierarchyCapability,
                        CallHierarchyRegistrationOptions>(outgoingHandler, registrationOptions))
            );
        }

        public static IDisposable OnCallHierarchy(
            this ILanguageServerRegistry registry,
            Func<CallHierarchyPrepareParams, CallHierarchyCapability,
                    Task<Container<CallHierarchyItem>>>
                handler,
            Func<CallHierarchyIncomingCallsParams, CallHierarchyCapability, Task<Container<CallHierarchyIncomingCall>>>
                incomingHandler,
            Func<CallHierarchyOutgoingCallsParams, CallHierarchyCapability, Task<Container<CallHierarchyOutgoingCall>>>
                outgoingHandler,
            CallHierarchyRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CallHierarchyRegistrationOptions();
            return new CompositeDisposable(
                registry.AddHandler(TextDocumentNames.PrepareCallHierarchy,
                    new LanguageProtocolDelegatingHandlers.Request<CallHierarchyPrepareParams,
                        Container<CallHierarchyItem>,
                        CallHierarchyCapability,
                        CallHierarchyRegistrationOptions>(handler, registrationOptions)),
                registry.AddHandler(TextDocumentNames.CallHierarchyIncoming,
                    new LanguageProtocolDelegatingHandlers.Request<CallHierarchyIncomingCallsParams,
                        Container<CallHierarchyIncomingCall>,
                        CallHierarchyCapability,
                        CallHierarchyRegistrationOptions>(incomingHandler, registrationOptions)),
                registry.AddHandler(TextDocumentNames.CallHierarchyOutgoing,
                    new LanguageProtocolDelegatingHandlers.Request<CallHierarchyOutgoingCallsParams,
                        Container<CallHierarchyOutgoingCall>,
                        CallHierarchyCapability,
                        CallHierarchyRegistrationOptions>(outgoingHandler, registrationOptions))
            );
        }

        public static IDisposable OnCallHierarchy(
            this ILanguageServerRegistry registry,
            Func<CallHierarchyPrepareParams, CancellationToken,
                    Task<Container<CallHierarchyItem>>>
                handler,
            Func<CallHierarchyIncomingCallsParams, CancellationToken, Task<Container<CallHierarchyIncomingCall>>>
                incomingHandler,
            Func<CallHierarchyOutgoingCallsParams, CancellationToken, Task<Container<CallHierarchyOutgoingCall>>>
                outgoingHandler,
            CallHierarchyRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CallHierarchyRegistrationOptions();
            return new CompositeDisposable(
                registry.AddHandler(TextDocumentNames.PrepareCallHierarchy,
                    new LanguageProtocolDelegatingHandlers.RequestRegistration<CallHierarchyPrepareParams,
                        Container<CallHierarchyItem>,
                        CallHierarchyRegistrationOptions>(handler, registrationOptions)),
                registry.AddHandler(TextDocumentNames.CallHierarchyIncoming,
                    new LanguageProtocolDelegatingHandlers.RequestRegistration<CallHierarchyIncomingCallsParams,
                        Container<CallHierarchyIncomingCall>,
                        CallHierarchyRegistrationOptions>(incomingHandler, registrationOptions)),
                registry.AddHandler(TextDocumentNames.CallHierarchyOutgoing,
                    new LanguageProtocolDelegatingHandlers.RequestRegistration<CallHierarchyOutgoingCallsParams,
                        Container<CallHierarchyOutgoingCall>,
                        CallHierarchyRegistrationOptions>(outgoingHandler, registrationOptions))
            );
        }

        public static IDisposable OnCallHierarchy(
            this ILanguageServerRegistry registry,
            Func<CallHierarchyPrepareParams,
                    Task<Container<CallHierarchyItem>>>
                handler,
            Func<CallHierarchyIncomingCallsParams, Task<Container<CallHierarchyIncomingCall>>>
                incomingHandler,
            Func<CallHierarchyOutgoingCallsParams, Task<Container<CallHierarchyOutgoingCall>>>
                outgoingHandler,
            CallHierarchyRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CallHierarchyRegistrationOptions();
            return new CompositeDisposable(
                registry.AddHandler(TextDocumentNames.PrepareCallHierarchy,
                    new LanguageProtocolDelegatingHandlers.RequestRegistration<CallHierarchyPrepareParams,
                        Container<CallHierarchyItem>,
                        CallHierarchyRegistrationOptions>(handler, registrationOptions)),
                registry.AddHandler(TextDocumentNames.CallHierarchyIncoming,
                    new LanguageProtocolDelegatingHandlers.RequestRegistration<CallHierarchyIncomingCallsParams,
                        Container<CallHierarchyIncomingCall>,
                        CallHierarchyRegistrationOptions>(incomingHandler, registrationOptions)),
                registry.AddHandler(TextDocumentNames.CallHierarchyOutgoing,
                    new LanguageProtocolDelegatingHandlers.RequestRegistration<CallHierarchyOutgoingCallsParams,
                        Container<CallHierarchyOutgoingCall>,
                        CallHierarchyRegistrationOptions>(outgoingHandler, registrationOptions))
            );
        }
    }
}
