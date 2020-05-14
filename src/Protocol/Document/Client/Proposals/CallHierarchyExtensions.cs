using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document.Client.Proposals
{
    [Obsolete(Constants.Proposal)]
    public static class CallHierarchyExtensions
    {
        public static Task<Container<CallHierarchyItem>> PrepareCallHierarchy(this ITextDocumentLanguageClient mediator,
            CallHierarchyPrepareParams @params, CancellationToken cancellationToken)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }

        public static Task<Container<CallHierarchyIncomingCall>> CallHierarchyIncomingCalls(
            this ITextDocumentLanguageClient mediator, CallHierarchyIncomingCallsParams @params,
            CancellationToken cancellationToken)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }

        public static Task<Container<CallHierarchyOutgoingCall>> CallHierarchyOutgoingCalls(
            this ITextDocumentLanguageClient mediator, CallHierarchyOutgoingCallsParams @params,
            CancellationToken cancellationToken)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
