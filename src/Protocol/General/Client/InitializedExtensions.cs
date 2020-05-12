using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class InitializedExtensions
    {
        public static void SendInitialized(this ILanguageClient mediator, InitializedParams @params)
        {
            mediator.SendNotification(GeneralNames.Initialized, @params);
        }
    }
}
