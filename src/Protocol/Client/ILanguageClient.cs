using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public interface ILanguageClient : ILanguageClientRegistry, IResponseRouter, IDisposable
    {
        InitializeParams ClientSettings { get; }
        InitializeResult ServerSettings { get; }
        ITextDocumentLanguageClient TextDocument { get; }
        IClientLanguageClient Client { get; }
        IWindowLanguageClient Window { get; }
        IWorkspaceLanguageClient Workspace { get; }
        IProgressManager ProgressManager { get; }
        IServiceProvider Services { get; }
        Task Initialize(CancellationToken token);
        Task Shutdown();
        IRegistrationManager RegistrationManager { get; }
        IWorkspaceFoldersManager WorkspaceFoldersManager { get; }
    }
}
