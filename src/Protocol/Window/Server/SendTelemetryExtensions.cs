

// ReSharper disable CheckNamespace

using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public static class SendTelemetryExtensions
    {
        public static void SendTelemetry(this IWindowLanguageServer mediator, object @params)
        {
            mediator.SendNotification(WindowNames.TelemetryEvent, @params);
        }
    }
}
