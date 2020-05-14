using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public interface IWindowLanguageServer : IResponseRouter
    {
        IWindowProgressLanguageServer Progress { get; }
    }
}
