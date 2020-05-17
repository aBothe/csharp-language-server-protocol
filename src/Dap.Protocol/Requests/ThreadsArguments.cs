using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.Threads)]
    public class ThreadsArguments : IRequest<ThreadsResponse>
    {
    }

}
