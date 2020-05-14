using OmniSharp.Extensions.LanguageServer.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class DocumentHighlightCapability : DynamicCapability, ConnectedCapability<IDocumentHighlightHandler> { }
}
