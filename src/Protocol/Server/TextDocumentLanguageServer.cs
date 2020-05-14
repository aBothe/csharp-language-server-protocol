using System;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public class TextDocumentLanguageServer : ClientProxyBase, ITextDocumentLanguageServer
    {
        public TextDocumentLanguageServer(IResponseRouter responseRouter, IServiceProvider serviceProvider) : base(responseRouter, serviceProvider) { }
    }
}
