using System;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public class ClientLanguageServer : ClientProxyBase, IClientLanguageServer
    {
        public ClientLanguageServer(IResponseRouter responseRouter, IServiceProvider serviceProvider) : base(responseRouter, serviceProvider) { }
    }
}
