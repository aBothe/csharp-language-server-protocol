using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Server
{
    [Parallel, Method(TextDocumentNames.CodeAction)]
    public interface ICodeActionHandler : IJsonRpcRequestHandler<CodeActionParams, CommandOrCodeActionContainer>,
        IRegistration<CodeActionRegistrationOptions>, ICapability<CodeActionCapability>
    {
    }

    public abstract class CodeActionHandler : ICodeActionHandler
    {
        private readonly CodeActionRegistrationOptions _options;
        protected IWorkDoneProgressManager ProgressManager { get; }

        public CodeActionHandler(CodeActionRegistrationOptions registrationOptions,
            IWorkDoneProgressManager progressManager)
        {
            _options = registrationOptions;
            ProgressManager = progressManager;
        }

        public CodeActionRegistrationOptions GetRegistrationOptions() => _options;

        public abstract Task<CommandOrCodeActionContainer> Handle(CodeActionParams request,
            CancellationToken cancellationToken);

        public virtual void SetCapability(CodeActionCapability capability) => Capability = capability;
        protected CodeActionCapability Capability { get; private set; }
    }

    public static class CodeActionExtensions
    {
        public static IDisposable OnCodeAction(
            this ILanguageServerRegistry registry,
            Func<CodeActionParams, CodeActionCapability, CancellationToken, Task<CommandOrCodeActionContainer>>
                handler,
            CodeActionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeActionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.CodeAction,
                new LanguageProtocolDelegatingHandlers.Request<CodeActionParams, CommandOrCodeActionContainer, CodeActionCapability,
                    CodeActionRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnCodeAction(
            this ILanguageServerRegistry registry,
            Func<CodeActionParams, CancellationToken, Task<CommandOrCodeActionContainer>> handler,
            CodeActionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeActionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.CodeAction,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<CodeActionParams, CommandOrCodeActionContainer,
                    CodeActionRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnCodeAction(
            this ILanguageServerRegistry registry,
            Func<CodeActionParams, Task<CommandOrCodeActionContainer>> handler,
            CodeActionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeActionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.CodeAction,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<CodeActionParams, CommandOrCodeActionContainer,
                    CodeActionRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnCodeAction(
            this ILanguageServerRegistry registry,
            Action<CodeActionParams, IObserver<Container<CodeActionOrCommand>>, CodeActionCapability,
                CancellationToken> handler,
            CodeActionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeActionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.CodeAction,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<CodeActionParams, CommandOrCodeActionContainer,
                        CodeActionOrCommand, CodeActionCapability, CodeActionRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>()));
        }

        public static IDisposable OnCodeAction(
            this ILanguageServerRegistry registry,
            Action<CodeActionParams, IObserver<Container<CodeActionOrCommand>>, CodeActionCapability>
                handler,
            CodeActionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeActionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.CodeAction,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<CodeActionParams, CommandOrCodeActionContainer,
                        CodeActionOrCommand, CodeActionCapability, CodeActionRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>()));
        }

        public static IDisposable OnCodeAction(
            this ILanguageServerRegistry registry,
            Action<CodeActionParams, IObserver<Container<CodeActionOrCommand>>, CancellationToken> handler,
            CodeActionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeActionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.CodeAction,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<CodeActionParams, CommandOrCodeActionContainer,
                        CodeActionOrCommand, CodeActionRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>()));
        }

        public static IDisposable OnCodeAction(
            this ILanguageServerRegistry registry,
            Action<CodeActionParams, IObserver<Container<CodeActionOrCommand>>> handler,
            CodeActionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeActionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.CodeAction,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<CodeActionParams, CommandOrCodeActionContainer,
                        CodeActionOrCommand, CodeActionRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>()));
        }
    }
}
