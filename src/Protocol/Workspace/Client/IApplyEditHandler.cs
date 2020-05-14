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
    public interface IApplyEditHandler : IJsonRpcRequestHandler<ApplyWorkspaceEditParams, ApplyWorkspaceEditResponse> { }

    public abstract class ApplyEditHandler : IApplyEditHandler
    {
        public abstract Task<ApplyWorkspaceEditResponse> Handle(ApplyWorkspaceEditParams request, CancellationToken cancellationToken);
    }

    public static class ApplyEditHandlerExtensions
    {
        public static IDisposable OnApplyEdit(this ILanguageClientRegistry registry, Func<ApplyWorkspaceEditParams, Task<ApplyWorkspaceEditResponse>> handler)
        {
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler));
        }

        class DelegatingHandler : ApplyEditHandler
        {
            private readonly Func<ApplyWorkspaceEditParams, Task<ApplyWorkspaceEditResponse>> _handler;

            public DelegatingHandler(Func<ApplyWorkspaceEditParams, Task<ApplyWorkspaceEditResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<ApplyWorkspaceEditResponse> Handle(ApplyWorkspaceEditParams request, CancellationToken cancellationToken) => _handler.Invoke(request);
        }
    }
}
