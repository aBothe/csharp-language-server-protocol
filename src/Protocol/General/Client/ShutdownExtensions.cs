using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Client
{
    public static class ShutdownExtensions
    {
        public static Task RequestShutdown(this ILanguageClient mediator, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(new ShutdownParams(), cancellationToken);
        }
    }
}
