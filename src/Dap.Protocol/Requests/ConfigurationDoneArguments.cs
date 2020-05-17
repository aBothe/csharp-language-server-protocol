using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.ConfigurationDone)]
    public class ConfigurationDoneArguments : IRequest<ConfigurationDoneResponse>
    {
    }

}
