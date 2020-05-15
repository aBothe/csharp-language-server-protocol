using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Server
{
    [Parallel, Method(TextDocumentNames.WillSave)]
    public interface IWillSaveTextDocumentHandler : IJsonRpcNotificationHandler<WillSaveTextDocumentParams>, IRegistration<TextDocumentRegistrationOptions>, ICapability<SynchronizationCapability> { }

    public abstract class WillSaveTextDocumentHandler : IWillSaveTextDocumentHandler
    {
        private readonly TextDocumentRegistrationOptions _options;
        public WillSaveTextDocumentHandler(TextDocumentRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public TextDocumentRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Unit> Handle(WillSaveTextDocumentParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(SynchronizationCapability capability) => Capability = capability;
        protected SynchronizationCapability Capability { get; private set; }
    }

    public static class WillSaveTextDocumentHandlerExtensions
    {
        public static IDisposable OnWillSaveTextDocument(
            this ILanguageServerRegistry registry,
            Func<WillSaveTextDocumentParams, SynchronizationCapability, CancellationToken, Task> handler,
            TextDocumentRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.WillSave,
                new LanguageProtocolDelegatingHandlers.Notification<WillSaveTextDocumentParams, SynchronizationCapability,
                    TextDocumentRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnWillSaveTextDocument(
            this ILanguageServerRegistry registry,
            Func<WillSaveTextDocumentParams, CancellationToken, Task> handler,
            TextDocumentRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.WillSave,
                new LanguageProtocolDelegatingHandlers.Notification<WillSaveTextDocumentParams,
                    TextDocumentRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnWillSaveTextDocument(
            this ILanguageServerRegistry registry,
            Func<WillSaveTextDocumentParams, Task> handler,
            TextDocumentRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.WillSave,
                new LanguageProtocolDelegatingHandlers.Notification<WillSaveTextDocumentParams,
                    TextDocumentRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnWillSaveTextDocument(
            this ILanguageServerRegistry registry,
            Action<WillSaveTextDocumentParams, SynchronizationCapability, CancellationToken> handler,
            TextDocumentRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.WillSave,
                new LanguageProtocolDelegatingHandlers.Notification<WillSaveTextDocumentParams, SynchronizationCapability,
                    TextDocumentRegistrationOptions>((p, c, ct) => {
                    handler(p, c, ct);
                    return Task.CompletedTask;
                }, registrationOptions));
        }

        public static IDisposable OnWillSaveTextDocument(
            this ILanguageServerRegistry registry,
            Action<WillSaveTextDocumentParams, CancellationToken> handler,
            TextDocumentRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.WillSave,
                new LanguageProtocolDelegatingHandlers.Notification<WillSaveTextDocumentParams,
                    TextDocumentRegistrationOptions>((p, ct) => {
                    handler(p, ct);
                    return Task.CompletedTask;
                }, registrationOptions));
        }

        public static IDisposable OnWillSaveTextDocument(
            this ILanguageServerRegistry registry,
            Action<WillSaveTextDocumentParams> handler,
            TextDocumentRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.WillSave,
                new LanguageProtocolDelegatingHandlers.Notification<WillSaveTextDocumentParams,
                    TextDocumentRegistrationOptions>((p, ct) => {
                    handler(p);
                    return Task.CompletedTask;
                }, registrationOptions));
        }
    }
}
