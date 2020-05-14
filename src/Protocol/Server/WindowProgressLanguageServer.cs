using System;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public class WindowProgressLanguageServer : ClientProxyBase, IWindowProgressLanguageServer
    {
        public WindowProgressLanguageServer(IResponseRouter responseRouter, IServiceProvider serviceProvider) : base(responseRouter, serviceProvider) { }
    }
}
