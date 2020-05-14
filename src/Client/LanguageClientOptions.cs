using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Reactive.Disposables;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Shared;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    public class LanguageClientOptions : ILanguageClientRegistry, IJsonRpcServerOptions
    {
        public LanguageClientOptions()
        {
        }

        public IProgressManager ProgressManager { get; } = new ProgressManager();

        public ClientCapabilities ClientCapabilities { get; set; } = new ClientCapabilities() {
            Experimental = new Dictionary<string, JToken>(),
            Window = new WindowClientCapabilities(),
            Workspace = new WorkspaceClientCapabilities(),
            TextDocument = new TextDocumentClientCapabilities()
        };

        public PipeReader Input { get; set; }
        public PipeWriter Output { get; set; }
        public ClientInfo ClientInfo { get; set; }
        public DocumentUri RootUri { get; set; }

        public string RootPath
        {
            get => RootUri.GetFileSystemPath();
            set => RootUri = DocumentUri.FromFileSystemPath(value);
        }

        public InitializeTrace Trace { get; set; }

        public object InitializationOptions { get; set; }

        public ISerializer Serializer { get; set; } =
            OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Serializer.Instance;

        public IRequestProcessIdentifier RequestProcessIdentifier { get; set; } = new RequestProcessIdentifier();
        public ILspClientReceiver Receiver { get; set; } = new LspClientReceiver();
        public IServiceCollection Services { get; set; } = new ServiceCollection();
        internal List<WorkspaceFolder> WorkspaceFolders { get; set; } = new List<WorkspaceFolder>();
        internal List<IJsonRpcHandler> Handlers { get; set; } = new List<IJsonRpcHandler>();

        internal List<ITextDocumentIdentifier> TextDocumentIdentifiers { get; set; } =
            new List<ITextDocumentIdentifier>();

        internal List<(string name, IJsonRpcHandler handler)> NamedHandlers { get; set; } =
            new List<(string name, IJsonRpcHandler handler)>();

        internal List<(string name, Func<IServiceProvider, IJsonRpcHandler> handlerFunc)>
            NamedServiceHandlers { get; set; } =
            new List<(string name, Func<IServiceProvider, IJsonRpcHandler> handlerFunc)>();

        internal List<Type> HandlerTypes { get; set; } = new List<Type>();
        internal List<Type> TextDocumentIdentifierTypes { get; set; } = new List<Type>();
        internal List<Assembly> HandlerAssemblies { get; set; } = new List<Assembly>();
        internal List<ICapability> SupportedCapabilities { get; set; } = new List<ICapability>();
        internal Action<ILoggingBuilder> LoggingBuilderAction { get; set; } = new Action<ILoggingBuilder>(_ => { });

        internal Action<IConfigurationBuilder> ConfigurationBuilderAction { get; set; } =
            new Action<IConfigurationBuilder>(_ => { });

        internal bool AddDefaultLoggingProvider { get; set; }
        public int? Concurrency { get; set; }

        // internal readonly List<InitializeDelegate> InitializeDelegates = new List<InitializeDelegate>();
        // internal readonly List<InitializedDelegate> InitializedDelegates = new List<InitializedDelegate>();
        internal readonly List<OnClientStartedDelegate> StartedDelegates = new List<OnClientStartedDelegate>();

        public IDisposable AddHandler(string method, IJsonRpcHandler handler)
        {
            NamedHandlers.Add((method, handler));
            return Disposable.Empty;
        }

        public IDisposable AddHandler(string method, Func<IServiceProvider, IJsonRpcHandler> handlerFunc)
        {
            NamedServiceHandlers.Add((method, handlerFunc));
            return Disposable.Empty;
        }

        public IDisposable AddHandler<T>(Func<IServiceProvider, T> handlerFunc) where T : IJsonRpcHandler
        {
            NamedServiceHandlers.Add((HandlerTypeHelper.GetMethodName<T>(), _ => handlerFunc(_)));
            return Disposable.Empty;
        }

        public IDisposable AddHandlers(params IJsonRpcHandler[] handlers)
        {
            Handlers.AddRange(handlers);
            return Disposable.Empty;
        }

        public IDisposable AddHandler<T>() where T : IJsonRpcHandler
        {
            HandlerTypes.Add(typeof(T));
            return Disposable.Empty;
        }

        public IDisposable AddTextDocumentIdentifier(params ITextDocumentIdentifier[] handlers)
        {
            TextDocumentIdentifiers.AddRange(handlers);
            return Disposable.Empty;
        }

        public IDisposable AddTextDocumentIdentifier<T>() where T : ITextDocumentIdentifier
        {
            TextDocumentIdentifierTypes.Add(typeof(T));
            return Disposable.Empty;
        }
    }
}
