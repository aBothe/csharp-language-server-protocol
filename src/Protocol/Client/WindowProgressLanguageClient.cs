using System;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public class WindowProgressLanguageClient : ClientProxyBase, IWindowProgressLanguageClient
    {
        public WindowProgressLanguageClient(IResponseRouter responseRouter, IServiceProvider serviceProvider) : base(responseRouter, serviceProvider) { }
    }
}
