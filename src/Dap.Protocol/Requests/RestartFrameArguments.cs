using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.RestartFrame)]
    public class RestartFrameArguments : IRequest<RestartFrameResponse>
    {
        /// <summary>
        /// Restart this stackframe.
        /// </summary>
        public long FrameId { get; set; }
    }

}
