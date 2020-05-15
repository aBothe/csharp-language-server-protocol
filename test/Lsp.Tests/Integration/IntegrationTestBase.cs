using System;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Integration
{
    public abstract class IntegrationTestBase : IAsyncLifetime
    {
        private ILanguageClient _client;
        private readonly ILoggerFactory _clientLogger;
        private ILanguageServer _server;
        private readonly ILoggerFactory _serverLogger;
        private readonly CompositeDisposable _disposable;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public IntegrationTestBase(ITestOutputHelper outputHelper)
        {
            _disposable = new CompositeDisposable();
            _clientLogger = new TestLoggerFactory(outputHelper,
                "{Timestamp:yyyy-MM-dd HH:mm:ss} [Client] [{Level}] {Message}{NewLine}{Exception}");
            _serverLogger = new TestLoggerFactory(outputHelper,
                "{Timestamp:yyyy-MM-dd HH:mm:ss} [Server] [{Level}] {Message}{NewLine}{Exception}");
            _disposable.Add(_clientLogger);
            _disposable.Add(_serverLogger);

            _cancellationTokenSource = new CancellationTokenSource();
            if (!Debugger.IsAttached)
            {
                _cancellationTokenSource.CancelAfter(TimeSpan.FromMinutes(1));
            }
        }

        protected async Task<(ILanguageClient client, ILanguageServer server)> Initialize(
            Action<LanguageClientOptions> clientOptionsAction,
            Action<LanguageServerOptions> serverOptionsAction)
        {
            var clientPipe = new Pipe();
            var serverPipe = new Pipe();
            _client = LanguageClient.PreInit(options => {
                options.Services.AddSingleton(_clientLogger);
                options.WithInput(serverPipe.Reader).WithOutput(clientPipe.Writer);
                clientOptionsAction(options);
            });

            _server = LanguageServer.PreInit(options => {
                options.Services.AddSingleton(_serverLogger);
                options.WithInput(clientPipe.Reader).WithOutput(serverPipe.Writer);
                serverOptionsAction(options);
            });

            _disposable.Add(_client);
            _disposable.Add(_server);

            return await ObservableEx.ForkJoin(
                Observable.FromAsync(_client.Initialize),
                Observable.FromAsync(_server.Initialize),
                (a, b) => (_client, _server)
            ).ToTask(CancellationToken);
        }

        protected CancellationToken CancellationToken => _cancellationTokenSource.Token;

        public virtual Task InitializeAsync() => Task.CompletedTask;

        public virtual async Task DisposeAsync()
        {
            await _client.Shutdown();
            _disposable.Dispose();
        }

        protected Task WaitForUpdate<T>(IObservableList<T> list)
        {
            return list.Connect()
                .Timeout(TimeSpan.FromSeconds(2))
                .Take(list.Count > 0 ? 2 : 1)
                .ToTask(CancellationToken);
        }

        protected Task WaitForUpdate<T, K>(IObservableCache<T, K> list)
        {
            return list.Preview()
                .Timeout(TimeSpan.FromSeconds(2))
                .Take(list.Count > 0 ? 2 : 1)
                .ToTask(CancellationToken);
        }

        protected bool SelectorMatches(Registration registration, Func<DocumentFilter, bool> documentFilter)
        {
            return SelectorMatches(registration.RegisterOptions, documentFilter);
        }

        protected bool SelectorMatches(object options, Func<DocumentFilter, bool> documentFilter)
        {
            if (options is ITextDocumentRegistrationOptions tdro)
                return tdro.DocumentSelector.Any(documentFilter);
            if (options is DocumentSelector selector)
                return selector.Any(documentFilter);
            return false;
        }
    }
}
