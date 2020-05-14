using System;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public class WorkspaceLanguageClient : ClientProxyBase, IWorkspaceLanguageClient
    {
        public WorkspaceLanguageClient(IResponseRouter responseRouter, IServiceProvider serviceProvider) : base(responseRouter, serviceProvider) { }
    }
}
