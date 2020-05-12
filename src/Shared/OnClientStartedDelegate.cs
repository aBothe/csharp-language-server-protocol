using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageProtocolShared
{
    public delegate Task OnClientStartedDelegate(ILanguageClient client, InitializeResult result, CancellationToken cancellationToken);
}