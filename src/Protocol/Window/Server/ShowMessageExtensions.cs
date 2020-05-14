using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public static class ShowMessageExtensions
    {
        public static void ShowMessage(this IWindowLanguageServer mediator, ShowMessageParams @params)
        {
            mediator.SendNotification(WindowNames.ShowMessage, @params);
        }

        public static void Show(this IWindowLanguageServer mediator, ShowMessageParams @params)
        {
            mediator.ShowMessage(@params);
        }

        public static void ShowError(this IWindowLanguageServer mediator, string message)
        {
            mediator.ShowMessage(new ShowMessageParams() { Type = MessageType.Error, Message = message });
        }

        public static void Show(this IWindowLanguageServer mediator, string message)
        {
            mediator.ShowMessage(new ShowMessageParams() { Type = MessageType.Log, Message = message });
        }

        public static void ShowWarning(this IWindowLanguageServer mediator, string message)
        {
            mediator.ShowMessage(new ShowMessageParams() { Type = MessageType.Warning, Message = message });
        }

        public static void ShowInfo(this IWindowLanguageServer mediator, string message)
        {
            mediator.ShowMessage(new ShowMessageParams() { Type = MessageType.Info, Message = message });
        }
    }
}
