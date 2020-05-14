using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public static class WorkDoneProgressExtensions
    {
        public static async Task Create(this IWindowProgressLanguageServer mediator, WorkDoneProgressCreateParams @params, CancellationToken cancellationToken = default)
        {
            await mediator.SendRequest(@params, cancellationToken);
        }
    }
}
