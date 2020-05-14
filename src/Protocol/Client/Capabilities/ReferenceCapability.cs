using OmniSharp.Extensions.LanguageServer.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class ReferenceCapability : DynamicCapability, ConnectedCapability<IReferencesHandler> { }
}
