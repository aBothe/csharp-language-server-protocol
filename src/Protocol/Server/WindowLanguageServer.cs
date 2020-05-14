using System;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public class WindowLanguageServer : ClientProxyBase, IWindowLanguageServer
    {
        public WindowLanguageServer(IResponseRouter responseRouter, IServiceProvider serviceProvider) : base(responseRouter, serviceProvider)
        {
            Progress = new WindowProgressLanguageServer(responseRouter, serviceProvider);
        }

        public IWindowProgressLanguageServer Progress { get; }
    }
}
