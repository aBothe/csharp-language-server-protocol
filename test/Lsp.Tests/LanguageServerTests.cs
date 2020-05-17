using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Client.Processes;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server;
using Xunit;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using OmniSharp.Extensions.LanguageServer.Shared;

namespace Lsp.Tests
{
    public class LanguageServerTests : AutoTestBase
    {
        public LanguageServerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact(Skip = "Doesn't work in CI :(")]
        public async Task Works_With_IWorkspaceSymbolsHandler()
        {
            var pipe = new Pipe();

            var process = new NamedPipeServerProcess(Guid.NewGuid().ToString("N"), LoggerFactory);
            await process.Start();
            var client = await LanguageClient.From(options => {
                options.Services.AddSingleton(LoggerFactory);
            });

            var handler = Substitute.For<IWorkspaceSymbolsHandler>();
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000 * 60 * 5);

            var serverStart = LanguageServer.From(x => x
                .WithInput(process.ClientOutputStream)
                .WithOutput(process.ClientInputStream)
                .ConfigureLogging(z => z.Services.AddSingleton(LoggerFactory)),
                cts.Token
            );

            await Task.WhenAll(client.Initialize(cts.Token), serverStart);
            using var server = await serverStart;
            server.AddHandlers(handler);
        }

        [Fact(Skip = "Doesn't work in CI :(")]
        public async Task GH141_CrashesWithEmptyInitializeParams()
        {
            var process = new NamedPipeServerProcess(Guid.NewGuid().ToString("N"), LoggerFactory);
            await process.Start();
            var server = LanguageServer.PreInit(x => x
                .WithInput(process.ClientOutputStream)
                .WithOutput(process.ClientInputStream)
                .ConfigureLogging(z => z.Services.AddSingleton(LoggerFactory))
                .AddHandlers(TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"), "csharp"))
            ) as IRequestHandler<InitializeParams, InitializeResult>;

            var handler = server as IRequestHandler<InitializeParams, InitializeResult>;

            Func<Task> a = async () => await handler.Handle(new InitializeParams() { }, CancellationToken.None);
            a.Should().NotThrow();
        }

        [Fact(Skip = "Doesn't work in CI :(")]
        public async Task TriggersStartedTask()
        {
            var startupInterface = Substitute.For(new [] {typeof(IOnServerStarted), typeof(IDidChangeConfigurationHandler) }, Array.Empty<object>()) as IOnServerStarted;
            var startedDelegate = Substitute.For<OnServerStartedDelegate>();
            startedDelegate(Arg.Any<ILanguageServer>(), Arg.Any<InitializeResult>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
            var process = new NamedPipeServerProcess(Guid.NewGuid().ToString("N"), LoggerFactory);
            await process.Start();
            var client = await LanguageClient.From(options => {
                options.Services.AddSingleton(LoggerFactory);
            });

            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(15));
            var serverStart = LanguageServer.From(x => x
                .OnStarted(startedDelegate)
                .OnStarted(startedDelegate)
                .OnStarted(startedDelegate)
                .OnStarted(startedDelegate)
                .WithHandler((IDidChangeConfigurationHandler) startupInterface)
                .WithInput(process.ClientOutputStream)
                .WithOutput(process.ClientInputStream)
                .ConfigureLogging(z => z.Services.AddSingleton(LoggerFactory))
                .AddHandlers(TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"), "csharp"))
            , cts.Token);

            await Task.WhenAll(client.Initialize(cts.Token), serverStart);
            using var server = await serverStart;

            _ = startedDelegate.Received(4)(Arg.Any<ILanguageServer>(), Arg.Any<InitializeResult>(), Arg.Any<CancellationToken>());
            _ = startupInterface.Received(1).OnStarted(Arg.Any<ILanguageServer>(), Arg.Any<InitializeResult>(), Arg.Any<CancellationToken>());
        }
    }
}
