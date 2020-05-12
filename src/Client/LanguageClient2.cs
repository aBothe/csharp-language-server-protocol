using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nerdbank.Streams;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.LanguageProtocolShared;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using HandlerCollection = OmniSharp.Extensions.JsonRpc.HandlerCollection;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

namespace OmniSharp.Extensions.LanguageClient.Client
{
    public interface IAwaitableTermination
    {
        Task WasShutDown { get; }
        Task WaitForExit { get; }
    }

    public interface ILanguageClient : OmniSharp.Extensions.LanguageServer.Protocol.Client.ILanguageClient
    {
        InitializeParams ClientSettings { get; }
        InitializeResult ServerSettings { get; }

        IServiceProvider Services { get; }
        // ILanguageClientConfiguration Configuration { get; }

        Task WasShutDown { get; }
        Task WaitForExit { get; }
    }

    public interface ILspClientReceiver : IReceiver
    {
        void Initialized();
    }

    public class LspClientReceiver : Receiver, ILspClientReceiver
    {
        private bool _initialized;

        public override (IEnumerable<Renor> results, bool hasResponse) GetRequests(JToken container)
        {
            if (_initialized) return base.GetRequests(container);

            var newResults = new List<Renor>();

            // Based on https://github.com/Microsoft/language-server-protocol/blob/master/protocol.md#initialize-request
            var (results, hasResponse) = base.GetRequests(container);
            foreach (var item in results)
            {
                if (item.IsRequest &&
                    HandlerTypeHelper.IsMethodName(item.Request.Method, typeof(IShowMessageRequestHandler)))
                {
                    newResults.Add(item);
                }
                else if (item.IsResponse)
                {
                    newResults.Add(item);
                }
                else if (item.IsNotification &&
                         HandlerTypeHelper.IsMethodName(item.Request.Method,
                             typeof(IShowMessageHandler),
                             typeof(ILogMessageHandler),
                             typeof(ITelemetryEventHandler))
                )
                {
                    newResults.Add(item);
                }
            }

            return (newResults, hasResponse);
        }

        public void Initialized()
        {
            _initialized = true;
        }
    }

    public class LanguageClientOptions : ILanguageClientRegistry
    {
        public LanguageClientOptions()
        {
        }

        public ProgressManager ProgressManager { get; } = new ProgressManager();
        public Stream Input { get; set; }
        public Stream Output { get; set; }
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

    public static class LanguageClientOptionsExtensions
    {
        public static LanguageClientOptions WithInput(this LanguageClientOptions options, Stream input)
        {
            options.Input = input;
            return options;
        }

        public static LanguageClientOptions WithOutput(this LanguageClientOptions options, Stream output)
        {
            options.Output = output;
            return options;
        }

        public static LanguageClientOptions WithRequestProcessIdentifier(this LanguageClientOptions options,
            IRequestProcessIdentifier requestProcessIdentifier)
        {
            options.RequestProcessIdentifier = requestProcessIdentifier;
            return options;
        }

        public static LanguageClientOptions WithSerializer(this LanguageClientOptions options, ISerializer serializer)
        {
            options.Serializer = serializer;
            return options;
        }

        public static LanguageClientOptions WithReciever(this LanguageClientOptions options,
            ILspClientReceiver serverReceiver)
        {
            options.Receiver = serverReceiver;
            return options;
        }

        public static LanguageClientOptions WithHandler<T>(this LanguageClientOptions options)
            where T : class, IJsonRpcHandler
        {
            options.Services.AddSingleton<IJsonRpcHandler, T>();
            return options;
        }

        public static LanguageClientOptions WithHandler<T>(this LanguageClientOptions options, T handler)
            where T : IJsonRpcHandler
        {
            options.Services.AddSingleton<IJsonRpcHandler>(handler);
            return options;
        }

        public static LanguageClientOptions WithHandlersFrom(this LanguageClientOptions options, Type type)
        {
            options.HandlerTypes.Add(type);
            return options;
        }

        public static LanguageClientOptions WithHandlersFrom(this LanguageClientOptions options, TypeInfo typeInfo)
        {
            options.HandlerTypes.Add(typeInfo.AsType());
            return options;
        }

        public static LanguageClientOptions WithHandlersFrom(this LanguageClientOptions options, Assembly assembly)
        {
            options.HandlerAssemblies.Add(assembly);
            return options;
        }

        public static LanguageClientOptions WithServices(this LanguageClientOptions options,
            Action<IServiceCollection> servicesAction)
        {
            servicesAction(options.Services);
            return options;
        }

        public static LanguageClientOptions WithClientInfo(this LanguageClientOptions options, ClientInfo clientInfo)
        {
            options.ClientInfo = clientInfo;
            return options;
        }

        public static LanguageClientOptions WithRootUri(this LanguageClientOptions options, DocumentUri rootUri)
        {
            options.RootUri = rootUri;
            return options;
        }

        public static LanguageClientOptions WithRootPath(this LanguageClientOptions options, string rootPath)
        {
            options.RootPath = rootPath;
            return options;
        }

        public static LanguageClientOptions WithTrace(this LanguageClientOptions options, InitializeTrace trace)
        {
            options.Trace = trace;
            return options;
        }

        public static LanguageClientOptions WithInitializationOptions(this LanguageClientOptions options,
            object initializationOptions)
        {
            options.InitializationOptions = initializationOptions;
            return options;
        }

        /// <summary>
        /// Set maximum number of allowed parallel actions
        /// </summary>
        /// <param name="options"></param>
        /// <param name="concurrency"></param>
        /// <returns></returns>
        public static LanguageClientOptions WithConcurrency(this LanguageClientOptions options, int? concurrency)
        {
            options.Concurrency = concurrency;
            return options;
        }

        // public static LanguageClientOptions OnInitialize(this LanguageClientOptions options, InitializeDelegate @delegate)
        // {
        //     options.InitializeDelegates.Add(@delegate);
        //     return options;
        // }
        //
        //
        // public static LanguageClientOptions OnInitialized(this LanguageClientOptions options, InitializedDelegate @delegate)
        // {
        //     options.InitializedDelegates.Add(@delegate);
        //     return options;
        // }

        public static LanguageClientOptions OnStarted(this LanguageClientOptions options,
            OnClientStartedDelegate @delegate)
        {
            options.StartedDelegates.Add(@delegate);
            return options;
        }

        public static LanguageClientOptions ConfigureLogging(this LanguageClientOptions options,
            Action<ILoggingBuilder> builderAction)
        {
            options.LoggingBuilderAction = builderAction;
            return options;
        }

        public static LanguageClientOptions AddDefaultLoggingProvider(this LanguageClientOptions options)
        {
            options.AddDefaultLoggingProvider = true;
            return options;
        }

        public static LanguageClientOptions ConfigureConfiguration(this LanguageClientOptions options,
            Action<IConfigurationBuilder> builderAction)
        {
            options.ConfigurationBuilderAction = builderAction;
            return options;
        }
    }

    public class LanguageClient2 : ILanguageClient, IAwaitableTermination, IDisposable
    {
        private readonly Connection _connection;
        private ClientVersion? _clientVersion;
        private readonly ClientInfo _clientInfo;
        private readonly ProgressManager _progressManager;
        private readonly ILspClientReceiver _receiver;
        private readonly ISerializer _serializer;
        private readonly TextDocumentIdentifiers _textDocumentIdentifiers;

        private readonly IHandlerCollection _collection;

        // private readonly IEnumerable<InitializeDelegate> _initializeDelegates;
        // private readonly IEnumerable<InitializedDelegate> _initializedDelegates;
        private readonly IEnumerable<OnClientStartedDelegate> _startedDelegates;
        private readonly IResponseRouter _responseRouter;
        private readonly ISubject<InitializeResult> _initializeComplete = new AsyncSubject<InitializeResult>();
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private readonly IServiceProvider _serviceProvider;
        private readonly SupportedCapabilities _supportedCapabilities;
        private Task _initializingTask;
        private readonly ILanguageClientConfiguration _configuration;
        private readonly int? _concurrency;
        private readonly object _initializationOptions;
        private readonly List<WorkspaceFolder> _workspaceFolders;
        private readonly DocumentUri _rootUri;
        private readonly InitializeTrace _trace;

        public static Task<ILanguageClient> From(Action<LanguageClientOptions> optionsAction)
        {
            return From(optionsAction, CancellationToken.None);
        }

        public static Task<ILanguageClient> From(LanguageClientOptions options)
        {
            return From(options, CancellationToken.None);
        }

        public static Task<ILanguageClient> From(Action<LanguageClientOptions> optionsAction, CancellationToken token)
        {
            var options = new LanguageClientOptions();
            optionsAction(options);
            return From(options, token);
        }

        public static ILanguageClient PreInit(Action<LanguageClientOptions> optionsAction)
        {
            var options = new LanguageClientOptions();
            optionsAction(options);
            return PreInit(options);
        }

        public static async Task<ILanguageClient> From(LanguageClientOptions options, CancellationToken token)
        {
            var server = (LanguageClient) PreInit(options);
            await server.Initialize(token);

            return server;
        }

        /// <summary>
        /// Create the server without connecting to the client
        ///
        /// Mainly used for unit testing
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static ILanguageClient PreInit(LanguageClientOptions options)
        {
            return new LanguageClient(options);
        }

        internal LanguageClient(LanguageClientOptions options)
        {
            var services = options.Services;
            var configurationProvider = new DidChangeConfigurationProvider(this, options.ConfigurationBuilderAction);
            services.AddSingleton<IJsonRpcHandler>(configurationProvider);

            services.AddSingleton<IConfiguration>(configurationProvider);
            services.AddSingleton(_configuration = configurationProvider);

            services.AddLogging(builder => options.LoggingBuilderAction(builder));
            services.AddSingleton<IOptionsMonitor<LoggerFilterOptions>, LanguageClientLoggerFilterOptions>();

            _clientInfo = options.ClientInfo;
            _receiver = options.Receiver;
            _progressManager = options.ProgressManager;
            _serializer = options.Serializer;
            _supportedCapabilities = new SupportedCapabilities();
            _textDocumentIdentifiers = new TextDocumentIdentifiers();
            var collection = new SharedHandlerCollection(_supportedCapabilities, _textDocumentIdentifiers);
            _collection = collection;
            // _initializeDelegates = initializeDelegates;
            // _initializedDelegates = initializedDelegates;
            _startedDelegates = options.StartedDelegates;
            _rootUri = options.RootUri;
            _trace = options.Trace;
            _initializationOptions = options.InitializationOptions;
            _workspaceFolders = options.WorkspaceFolders;

            services.AddSingleton<IOutputHandler>(_ =>
                ActivatorUtilities.CreateInstance<OutputHandler>(_, options.Output.UsePipeWriter()));
            services.AddSingleton(_collection);
            services.AddSingleton(_textDocumentIdentifiers);
            services.AddSingleton(_serializer);
            services.AddSingleton<OmniSharp.Extensions.JsonRpc.ISerializer>(_serializer);
            services.AddSingleton(options.RequestProcessIdentifier);
            services.AddSingleton<OmniSharp.Extensions.JsonRpc.IReceiver>(options.Receiver);
            services.AddSingleton<ILspClientReceiver>(options.Receiver);

            foreach (var item in options.Handlers)
            {
                services.AddSingleton(item);
            }

            foreach (var item in options.TextDocumentIdentifiers)
            {
                services.AddSingleton(item);
            }

            foreach (var item in options.HandlerTypes)
            {
                services.AddSingleton(typeof(IJsonRpcHandler), item);
            }

            foreach (var item in options.TextDocumentIdentifierTypes)
            {
                services.AddSingleton(typeof(ITextDocumentIdentifier), item);
            }

            services.AddJsonRpcMediatR(options.HandlerAssemblies);
            services.AddSingleton<LanguageServer.Protocol.Client.ILanguageClient>(this);
            services.AddSingleton<ILanguageClient>(this);
            services.AddSingleton<OmniSharp.Extensions.LanguageServer.Protocol.Client.ILanguageClient>(this);
            services.AddSingleton<LspRequestRouter>();
            services.AddSingleton<IRequestRouter<ILspHandlerDescriptor>>(_ => _.GetRequiredService<LspRequestRouter>());
            services.AddSingleton<IRequestRouter<IHandlerDescriptor>>(_ => _.GetRequiredService<LspRequestRouter>());
            services.AddSingleton<IResponseRouter, ResponseRouter>();

            var foundHandlers = services
                .Where(x => typeof(IJsonRpcHandler).IsAssignableFrom(x.ServiceType) &&
                            x.ServiceType != typeof(IJsonRpcHandler))
                .ToArray();

            // Handlers are created at the start and maintained as a singleton
            foreach (var handler in foundHandlers)
            {
                services.Remove(handler);

                if (handler.ImplementationFactory != null)
                    services.Add(ServiceDescriptor.Singleton(typeof(IJsonRpcHandler), handler.ImplementationFactory));
                else if (handler.ImplementationInstance != null)
                    services.Add(ServiceDescriptor.Singleton(typeof(IJsonRpcHandler), handler.ImplementationInstance));
                else
                    services.Add(ServiceDescriptor.Singleton(typeof(IJsonRpcHandler), handler.ImplementationType));
            }

            services.AddSingleton(_progressManager);
            _serviceProvider = services.BuildServiceProvider();
            collection.SetServiceProvider(_serviceProvider);

            var requestRouter = _serviceProvider.GetRequiredService<IRequestRouter<ILspHandlerDescriptor>>();
            _responseRouter = _serviceProvider.GetRequiredService<IResponseRouter>();
            _connection = new Connection(
                options.Input.UsePipeReader(),
                _serviceProvider.GetRequiredService<IOutputHandler>(),
                options.Receiver,
                options.RequestProcessIdentifier,
                _serviceProvider.GetRequiredService<IRequestRouter<IHandlerDescriptor>>(),
                _responseRouter,
                _serviceProvider.GetRequiredService<ILoggerFactory>(),
                options.Serializer,
                options.Concurrency
            );

            // We need to at least create Window here in case any handler does loggin in their constructor
            Document = new LanguageClientDocument(_responseRouter);
            Client = new LanguageClientClient(_responseRouter);
            Window = new LanguageClientWindow(_responseRouter);
            Workspace = new LanguageClientWorkspace(_responseRouter);

            _disposable.Add(AddHandlers(new CancelRequestHandler<ILspHandlerDescriptor>(requestRouter),
                _progressManager));

            var serviceHandlers = _serviceProvider.GetServices<IJsonRpcHandler>().ToArray();
            var serviceIdentifiers = _serviceProvider.GetServices<ITextDocumentIdentifier>().ToArray();
            _disposable.Add(_textDocumentIdentifiers.Add(serviceIdentifiers));
            _disposable.Add(_collection.Add(serviceHandlers));

            foreach (var (name, handler) in options.NamedHandlers)
            {
                _disposable.Add(_collection.Add(name, handler));
            }

            foreach (var (name, handlerFunc) in options.NamedServiceHandlers)
            {
                _disposable.Add(_collection.Add(name, handlerFunc(_serviceProvider)));
            }
        }

        public ILanguageClientDocument Document { get; }
        public ILanguageClientClient Client { get; }
        public ILanguageClientWindow Window { get; }
        public ILanguageClientWorkspace Workspace { get; }
        public ProgressManager ProgressManager => _progressManager;

        public InitializeParams ClientSettings { get; private set; }
        public InitializeResult ServerSettings { get; private set; }

        public IServiceProvider Services => _serviceProvider;
        public ILanguageClientConfiguration Configuration => _configuration;

        public IDisposable AddHandler(string method, IJsonRpcHandler handler)
        {
            var handlerDisposable = _collection.Add(method, handler);
            return RegisterHandlers(handlerDisposable, CancellationToken.None);
        }

        public IDisposable AddHandler(string method, Func<IServiceProvider, IJsonRpcHandler> handlerFunc)
        {
            var handlerDisposable = _collection.Add(method, handlerFunc);
            return RegisterHandlers(handlerDisposable, CancellationToken.None);
        }

        public IDisposable AddHandlers(params IJsonRpcHandler[] handlers)
        {
            var handlerDisposable = _collection.Add(handlers);
            return RegisterHandlers(handlerDisposable, CancellationToken.None);
        }

        public IDisposable AddHandler(string method, Type handlerType)
        {
            var handlerDisposable = _collection.Add(method, handlerType);
            return RegisterHandlers(handlerDisposable, CancellationToken.None);
        }

        public IDisposable AddHandler<T>()
            where T : IJsonRpcHandler
        {
            return AddHandlers(typeof(T));
        }

        public IDisposable AddHandlers(params Type[] handlerTypes)
        {
            var handlerDisposable = _collection.Add(_serviceProvider, handlerTypes);
            return RegisterHandlers(handlerDisposable, CancellationToken.None);
        }

        public IDisposable AddTextDocumentIdentifier(params ITextDocumentIdentifier[] handlers)
        {
            var cd = new CompositeDisposable();
            foreach (var textDocumentIdentifier in handlers)
            {
                cd.Add(_textDocumentIdentifiers.Add(textDocumentIdentifier));
            }

            return cd;
        }

        public IDisposable AddTextDocumentIdentifier<T>() where T : ITextDocumentIdentifier
        {
            return _textDocumentIdentifiers.Add(ActivatorUtilities.CreateInstance<T>(_serviceProvider));
        }

        public async Task Initialize(InitializeParams @params, CancellationToken token)
        {
            @params.Trace = _trace;
            @params.ClientInfo = _clientInfo;
            @params.RootUri = _rootUri;
            @params.RootPath = _rootUri.GetFileSystemPath();
            @params.WorkspaceFolders = _workspaceFolders;
            @params.InitializationOptions = _initializationOptions;

            ClientSettings = @params;

            var serverParams = await this.SendInitialize(ClientSettings, token);

            ServerSettings = serverParams;

            serverParams.Capabilities.CompletionProvider


            if (_initializingTask != null)
            {
                try
                {
                    await _initializingTask;
                }
                catch
                {
                    // Swallow exceptions because the original initialization task will report errors if it fails (don't want to doubly report).
                }

                return;
            }

            _connection.Open();
            try
            {
                _initializingTask = _initializeComplete
                    .Select(result => _startedDelegates.Select(@delegate =>
                            Observable.FromAsync(() => @delegate(this, result, token))
                        )
                        .ToObservable()
                        .Merge()
                        .Select(z => result)
                    )
                    .Merge()
                    .LastAsync()
                    .ToTask(token);
                await _initializingTask;
            }
            catch (TaskCanceledException e)
            {
                _initializeComplete.OnError(e);
            }
            catch (Exception e)
            {
                _initializeComplete.OnError(e);
            }
        }

        private IDisposable RegisterHandlers(LspHandlerDescriptorDisposable handlerDisposable, CancellationToken token)
        {
            var registrations = new List<Registration>();
            foreach (var descriptor in handlerDisposable.Descriptors)
            {
                if (descriptor.AllowsDynamicRegistration)
                {
                    if (descriptor.RegistrationOptions is IWorkDoneProgressOptions wdpo)
                    {
                        wdpo.WorkDoneProgress = _progressManager.IsSupported;
                    }

                    registrations.Add(new Registration() {
                        Id = descriptor.Id.ToString(),
                        Method = descriptor.Method,
                        RegisterOptions = descriptor.RegistrationOptions
                    });
                }

                if (descriptor.OnClientStartedDelegate != null)
                {
                    // Fire and forget to initialize the handler
                    _initializeComplete
                        .Select(result =>
                            Observable.FromAsync(() => descriptor.OnClientStartedDelegate(this, result, token)))
                        .Merge()
                        .Subscribe();
                }
            }

            // Fire and forget
            DynamicallyRegisterHandlers(registrations.ToArray()).ToObservable().Subscribe();

            return new CompositeDisposable(
                handlerDisposable,
                Disposable.Create(() => {
                    Client.UnregisterCapability(new UnregistrationParams() {
                        Unregisterations = registrations.ToArray()
                    }).ToObservable().Subscribe();
                }));
        }

        public async Task<MediatR.Unit> Handle(InitializedParams @params, CancellationToken token)
        {
            if (_clientVersion == ClientVersion.Lsp3)
            {
                // Small delay to let client respond
                await Task.Delay(100, token);

                _initializeComplete.OnNext(ServerSettings);
                _initializeComplete.OnCompleted();
            }

            return MediatR.Unit.Value;
        }

        private async Task DynamicallyRegisterHandlers(Registration[] registrations)
        {
            if (registrations.Length == 0)
                return; // No dynamic registrations supported by client.

            var @params = new RegistrationParams() {Registrations = registrations};

            await _initializeComplete;

            await Client.RegisterCapability(@params);
        }

        public IObservable<InitializeResult> Start => _initializeComplete.AsObservable();

        public void SendNotification(string method)
        {
            _responseRouter.SendNotification(method);
        }

        public void SendNotification<T>(string method, T @params)
        {
            _responseRouter.SendNotification(method, @params);
        }

        public void SendNotification(IRequest @params)
        {
            _responseRouter.SendNotification(@params);
        }

        public IResponseRouterReturns SendRequest(string method)
        {
            return _responseRouter.SendRequest(method);
        }

        public IResponseRouterReturns SendRequest<T>(string method, T @params)
        {
            return _responseRouter.SendRequest<T>(method, @params);
        }

        public Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> @params, CancellationToken cancellationToken)
        {
            return _responseRouter.SendRequest(@params, cancellationToken);
        }

        public TaskCompletionSource<JToken> GetRequest(long id)
        {
            return _responseRouter.GetRequest(id);
        }

        public Task<InitializeResult> WasStarted => _initializeComplete.ToTask();

        public void Dispose()
        {
            _connection?.Dispose();
            _disposable?.Dispose();
        }

        public IDictionary<string, JToken> Experimental { get; } = new Dictionary<string, JToken>();
    }
}
