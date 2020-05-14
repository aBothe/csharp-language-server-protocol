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
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document.Server.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;
using OmniSharp.Extensions.LanguageServer.Shared;
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
    }

    public class DynamicRegistrationTests : IntegrationTestBase
    {
        public DynamicRegistrationTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task Should_Register_Dynamically_After_Initialization()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);

            // await WaitForUpdate(client.RegistrationManager.Registrations);
            await Task.Delay(1000);

            client.RegistrationManager.Registrations.Items.Should().Contain(x =>
                x.Method == TextDocumentNames.Completion &&
                (x.RegisterOptions as CompletionRegistrationOptions).DocumentSelector ==
                DocumentSelector.ForLanguage("csharp")
            );
        }

        [Fact]
        public async Task Should_Register_Dynamically_While_Server_Is_Running()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);

            // await WaitForUpdate(client.RegistrationManager.Registrations);
            await Task.Delay(1000);

            server.OnCompletion(
                (@params, token) => Task.FromResult(new CompletionList()),
                registrationOptions: new CompletionRegistrationOptions() {
                    DocumentSelector = DocumentSelector.ForLanguage("vb")
                });

            // await WaitForUpdate(client.RegistrationManager.Registrations);
            await Task.Delay(1000);

            client.RegistrationManager.Registrations.Items.Should().Contain(x =>
                x.Method == TextDocumentNames.Completion &&
                (x.RegisterOptions as CompletionRegistrationOptions).DocumentSelector ==
                DocumentSelector.ForLanguage("vb")
            );
        }

        [Fact]
        public async Task Should_Unregister_Dynamically_While_Server_Is_Running()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);

            // await WaitForUpdate(client.RegistrationManager.Registrations);
            await Task.Delay(1000);

            var disposable = server.OnCompletion(
                (@params, token) => Task.FromResult(new CompletionList()),
                registrationOptions: new CompletionRegistrationOptions() {
                    DocumentSelector = DocumentSelector.ForLanguage("vb")
                });

            // await WaitForUpdate(client.RegistrationManager.Registrations);
            await Task.Delay(1000);

            disposable.Dispose();

            // await WaitForUpdate(client.RegistrationManager.Registrations);
                        await Task.Delay(1000);

            client.RegistrationManager.Registrations.Items.Should().NotContain(x =>
                x.Method == TextDocumentNames.Completion &&
                (x.RegisterOptions as CompletionRegistrationOptions).DocumentSelector ==
                DocumentSelector.ForLanguage("vb")
            );
        }

        [Fact]
        public async Task Should_Gather_Static_Registrations()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);
            client.RegistrationManager.Registrations.Items.Should().Contain(x => x.Method == TextDocumentNames.SemanticTokens);
        }

        private void ConfigureClient(LanguageClientOptions options)
        {
            options.WithCapability(new CompletionCapability() {
                CompletionItem = new CompletionItemCapability() {
                    DeprecatedSupport = true,
                    DocumentationFormat = new[] {MarkupKind.Markdown},
                    PreselectSupport = true,
                    SnippetSupport = true,
                    TagSupport = new CompletionItemTagSupportCapability() {
                        ValueSet = new[] {
                            CompletionItemTag.Deprecated
                        }
                    },
                    CommitCharactersSupport = true
                },
                ContextSupport = true,
                DynamicRegistration = true,
                CompletionItemKind = new CompletionItemKindCapability() {
                    ValueSet = new Container<CompletionItemKind>(Enum.GetValues(typeof(CompletionItemKind))
                        .Cast<CompletionItemKind>())
                }
            });

            options.WithCapability(new SemanticTokensCapability() {
                TokenModifiers = SemanticTokenModifier.Defaults.ToArray(),
                TokenTypes = SemanticTokenType.Defaults.ToArray()
            });
        }

        private void ConfigureServer(LanguageServerOptions options)
        {
            options.OnCompletion(
                (@params, token) => Task.FromResult(new CompletionList()),
                registrationOptions: new CompletionRegistrationOptions() {
                    DocumentSelector = DocumentSelector.ForLanguage("csharp")
                });

            var semanticRegistrationOptions = new SemanticTokensRegistrationOptions() {
                Id = Guid.NewGuid().ToString(),
                Legend = new SemanticTokensLegend(),
                DocumentProvider = new SemanticTokensDocumentProviderOptions(),
                DocumentSelector = DocumentSelector.ForLanguage("csharp"),
                RangeProvider = true
            };

            // Our server only statically registers when it detects a server that does not support dynamic capabilities
            // This forces it to do that.
            options.OnInitialized(
                (server, request, response, token) => {
                    response.Capabilities.SemanticTokensProvider = SemanticTokensOptions.Of(semanticRegistrationOptions,
                        Enumerable.Empty<ILspHandlerDescriptor>());
                    response.Capabilities.SemanticTokensProvider.Id = semanticRegistrationOptions.Id;
                    return Task.CompletedTask;
                });

            options.OnSemanticTokensEdits(
                (builder, @params, ct) => { return Task.CompletedTask; },
                (@params, token) => { return Task.FromResult(new SemanticTokensDocument(new SemanticTokensLegend())); });
        }
    }
}
