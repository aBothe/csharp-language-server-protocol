using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
[Method(RequestNames.ExceptionInfo)]
    public class ExceptionInfoArguments : IRequest<ExceptionInfoResponse>
    {
        /// <summary>
        /// Thread for which exception information should be retrieved.
        /// </summary>
        public long ThreadId { get; set; }
    }

}
