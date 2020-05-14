using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public static class LogMessageExtensions
    {
        public static void LogMessage(this IWindowLanguageServer mediator, LogMessageParams @params)
        {
            mediator.SendNotification(WindowNames.LogMessage, @params);
        }

        public static void Log(this IWindowLanguageServer mediator, LogMessageParams @params)
        {
            mediator.LogMessage(@params);
        }

        public static void LogError(this IWindowLanguageServer mediator, string message)
        {
            mediator.LogMessage(new LogMessageParams() { Type = MessageType.Error, Message = message });
        }

        public static void Log(this IWindowLanguageServer mediator, string message)
        {
            mediator.LogMessage(new LogMessageParams() { Type = MessageType.Log, Message = message });
        }

        public static void LogWarning(this IWindowLanguageServer mediator, string message)
        {
            mediator.LogMessage(new LogMessageParams() { Type = MessageType.Warning, Message = message });
        }

        public static void LogInfo(this IWindowLanguageServer mediator, string message)
        {
            mediator.LogMessage(new LogMessageParams() { Type = MessageType.Info, Message = message });
        }
    }
}
