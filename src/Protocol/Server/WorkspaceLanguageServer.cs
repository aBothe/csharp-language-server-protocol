using System;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public class WorkspaceLanguageServer : ClientProxyBase, IWorkspaceLanguageServer
    {
        public WorkspaceLanguageServer(IResponseRouter responseRouter, IServiceProvider serviceProvider) : base(responseRouter, serviceProvider) { }
    }
}
