using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Client
{
    [Parallel, Method(WorkspaceNames.ApplyEdit)]
    public interface IApplyWorkspaceEditHandler : IJsonRpcRequestHandler<ApplyWorkspaceEditParams, ApplyWorkspaceEditResponse> { }

    public abstract class ApplyWorkspaceEditHandler : IApplyWorkspaceEditHandler
    {
        public abstract Task<ApplyWorkspaceEditResponse> Handle(ApplyWorkspaceEditParams request, CancellationToken cancellationToken);
    }

    public static class ApplyWorkspaceEditExtensions
    {
        public static IDisposable OnApplyWorkspaceEdit(
            this ILanguageClientRegistry registry,
            Func<ApplyWorkspaceEditParams, CancellationToken, Task<ApplyWorkspaceEditResponse>>
                handler)
        {
            return registry.AddHandler(WorkspaceNames.ApplyEdit, RequestHandler.For(handler));
        }

        public static IDisposable OnApplyWorkspaceEdit(
            this ILanguageClientRegistry registry,
            Func<ApplyWorkspaceEditParams, Task<ApplyWorkspaceEditResponse>> handler)
        {
            return registry.AddHandler(WorkspaceNames.ApplyEdit, RequestHandler.For(handler));
        }
    }
}
