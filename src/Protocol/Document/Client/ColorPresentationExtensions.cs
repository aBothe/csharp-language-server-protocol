using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Client
{
    public static class ColorPresentationExtensions
    {
        public static Task<Container<ColorPresentation>> ColorPresentation(this ITextDocumentLanguageClient mediator, ColorPresentationParams @params, CancellationToken cancellationToken= default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
