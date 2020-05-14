using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public static class ShowMessageRequestExtensions
    {
        public static Task<MessageActionItem> ShowMessage(this IWindowLanguageServer mediator, ShowMessageRequestParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }

        public static Task<MessageActionItem> Show(this IWindowLanguageServer mediator, ShowMessageRequestParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.ShowMessage(@params, cancellationToken);
        }

        public static Task<MessageActionItem> Request(this IWindowLanguageServer mediator, ShowMessageRequestParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.ShowMessage(@params, cancellationToken);
        }
    }
}
