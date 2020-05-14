using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public interface ILanguageServer : ILanguageServerRegistry, IResponseRouter, IDisposable
    {
        ITextDocumentLanguageServer TextDocument { get; }
        IClientLanguageServer Client { get; }
        IWindowLanguageServer Window { get; }
        IWorkspaceLanguageServer Workspace { get; }
        IWorkDoneProgressManager ProgressManager { get; }
        IServiceProvider Services { get; }
        ILanguageServerConfiguration Configuration { get; }
        InitializeParams ClientSettings { get; }
        InitializeResult ServerSettings { get; }

        IObservable<InitializeResult> Start { get; }
        IObservable<bool> Shutdown { get; }
        IObservable<int> Exit { get; }
        Task<InitializeResult> WasStarted { get; }
        Task WasShutDown { get; }
        Task WaitForExit { get; }
        Task Initialize(CancellationToken token);
    }
}
