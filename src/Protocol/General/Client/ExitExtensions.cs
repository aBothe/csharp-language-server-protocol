

// ReSharper disable CheckNamespace

using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    public static class ExitExtensions
    {
        public static void RequestExit(this ILanguageClient mediator)
        {
            mediator.SendNotification(GeneralNames.Exit);
        }
    }
}
