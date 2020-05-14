using System;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public class WindowLanguageClient : ClientProxyBase, IWindowLanguageClient
    {
        public WindowLanguageClient(IResponseRouter responseRouter, IServiceProvider serviceProvider) : base(responseRouter, serviceProvider)
        {
            Progress = new WindowProgressLanguageClient(responseRouter, serviceProvider);
        }

        public IWindowProgressLanguageClient Progress { get; }
    }
}
