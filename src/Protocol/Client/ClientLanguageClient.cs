using System;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public class ClientLanguageClient : ClientProxyBase, IClientLanguageClient
    {
        public ClientLanguageClient(IResponseRouter responseRouter, IServiceProvider serviceProvider) : base(responseRouter, serviceProvider) { }
    }
}
