using System;
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
        Container<CallHierarchyIncomingCall>>
    {
    }

    [Obsolete(Constants.Proposal)]
    [Parallel, Method(TextDocumentNames.CallHierarchyOutgoing)]
    public interface ICallHierarchyOutgoingHandler : IJsonRpcRequestHandler<CallHierarchyOutgoingCallsParams,
        Container<CallHierarchyOutgoingCall>>
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
    public static class CallHierarchyHandlerExtensions
    {
        public static IDisposable OnCallHierarchy(
            this ILanguageServerRegistry registry,
            Func<CallHierarchyPrepareParams, CancellationToken, Task<Container<CallHierarchyItem>>> handler,
            Func<CallHierarchyIncomingCallsParams, CancellationToken, Task<Container<CallHierarchyIncomingCall>>>
                incomingHandler,
            Func<CallHierarchyOutgoingCallsParams, CancellationToken, Task<Container<CallHierarchyOutgoingCall>>>
                outgoingHandler,
            CallHierarchyRegistrationOptions registrationOptions = null,
            Action<CallHierarchyCapability> setCapability = null)
        {
            registrationOptions ??= new CallHierarchyRegistrationOptions();
            setCapability ??= x => { };
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler, incomingHandler, outgoingHandler, setCapability, registrationOptions));
        }

        class DelegatingHandler : CallHierarchyHandler
        {
            private readonly Func<CallHierarchyPrepareParams, CancellationToken, Task<Container<CallHierarchyItem>>>
                _handler;

            private readonly
                Func<CallHierarchyIncomingCallsParams, CancellationToken, Task<Container<CallHierarchyIncomingCall>>>
                _incomingHandler;

            private readonly
                Func<CallHierarchyOutgoingCallsParams, CancellationToken, Task<Container<CallHierarchyOutgoingCall>>>
                _outgoingHandler;

            private readonly Action<CallHierarchyCapability> _setCapability;
            private CallHierarchyHandler _callHierarchyHandlerImplementation;

            public DelegatingHandler(
                Func<CallHierarchyPrepareParams, CancellationToken, Task<Container<CallHierarchyItem>>> handler,
                Func<CallHierarchyIncomingCallsParams, CancellationToken, Task<Container<CallHierarchyIncomingCall>>>
                    incomingHandler,
                Func<CallHierarchyOutgoingCallsParams, CancellationToken, Task<Container<CallHierarchyOutgoingCall>>>
                    outgoingHandler,
                IWorkDoneProgressManager progressManager,
                Action<CallHierarchyCapability> setCapability,
                CallHierarchyRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _incomingHandler = incomingHandler;
                _outgoingHandler = outgoingHandler;
                _setCapability = setCapability;
            }

            public override Task<Container<CallHierarchyItem>> Handle(CallHierarchyPrepareParams request,
                CancellationToken cancellationToken) => _handler(request, cancellationToken);

            public override Task<Container<CallHierarchyIncomingCall>> Handle(CallHierarchyIncomingCallsParams request,
                CancellationToken cancellationToken) => _incomingHandler(request, cancellationToken);

            public override Task<Container<CallHierarchyOutgoingCall>> Handle(CallHierarchyOutgoingCallsParams request,
                CancellationToken cancellationToken) => _outgoingHandler(request, cancellationToken);

            public override void SetCapability(CallHierarchyCapability capability) =>
                _setCapability?.Invoke(capability);
        }
    }
}
