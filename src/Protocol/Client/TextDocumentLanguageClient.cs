using System;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public class TextDocumentLanguageClient : ClientProxyBase, ITextDocumentLanguageClient
    {
        public TextDocumentLanguageClient(IResponseRouter responseRouter, IServiceProvider serviceProvider) : base(responseRouter, serviceProvider) { }
    }
}
