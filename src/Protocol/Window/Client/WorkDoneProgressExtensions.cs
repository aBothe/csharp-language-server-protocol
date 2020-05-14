using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Client
{
    public static class WorkDoneProgressExtensions
    {
        public static void Cancel(this IWindowProgressLanguageClient mediator, ProgressToken token)
        {
            mediator.SendNotification(WindowNames.WorkDoneProgressCancel, new WorkDoneProgressCancelParams()
            {
                Token = token
            });
        }
    }
}
