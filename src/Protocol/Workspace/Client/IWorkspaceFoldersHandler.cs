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
    [Parallel, Method(WorkspaceNames.WorkspaceFolders)]
    public interface IWorkspaceFoldersHandler : IJsonRpcRequestHandler<WorkspaceFolderParams, Container<WorkspaceFolder>> { }

    public abstract class WorkspaceFoldersHandler : IWorkspaceFoldersHandler
    {
        public abstract Task<Container<WorkspaceFolder>> Handle(WorkspaceFolderParams request, CancellationToken cancellationToken);
    }

    public static class WorkspaceFoldersHandlerExtensions
    {
        public static IDisposable OnWorkspaceFolders(this ILanguageClientRegistry registry, Func<WorkspaceFolderParams, Task<Container<WorkspaceFolder>>> handler)
        {
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler));
        }

        class DelegatingHandler : WorkspaceFoldersHandler
        {
            private readonly Func<WorkspaceFolderParams, Task<Container<WorkspaceFolder>>> _handler;

            public DelegatingHandler(Func<WorkspaceFolderParams, Task<Container<WorkspaceFolder>>> handler)
            {
                _handler = handler;
            }

            public override Task<Container<WorkspaceFolder>> Handle(WorkspaceFolderParams request, CancellationToken cancellationToken) => _handler.Invoke(request);
        }
    }
}
