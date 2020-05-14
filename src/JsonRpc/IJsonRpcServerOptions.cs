using System.IO.Pipelines;
using Microsoft.Extensions.DependencyInjection;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IJsonRpcServerOptions
    {
        PipeReader Input { get; set; }
        PipeWriter Output { get; set; }
        IServiceCollection Services { get; set; }
        IRequestProcessIdentifier RequestProcessIdentifier { get; set; }
        int? Concurrency { get; set; }
    }
}
