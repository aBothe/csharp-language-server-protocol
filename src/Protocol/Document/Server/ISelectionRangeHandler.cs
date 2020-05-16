using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable once CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Server
{
    [Parallel, Method(TextDocumentNames.SelectionRange)]
    public interface ISelectionRangeHandler : IJsonRpcRequestHandler<SelectionRangeParams, Container<SelectionRange>>,
        IRegistration<SelectionRangeRegistrationOptions>, ICapability<SelectionRangeCapability>
    {
    }

    public abstract class SelectionRangeHandler : ISelectionRangeHandler
    {
        private readonly SelectionRangeRegistrationOptions _options;
        protected IWorkDoneProgressManager ProgressManager { get; }

        public SelectionRangeHandler(SelectionRangeRegistrationOptions registrationOptions,
            IWorkDoneProgressManager progressManager)
        {
            _options = registrationOptions;
            ProgressManager = progressManager;
        }

        public SelectionRangeRegistrationOptions GetRegistrationOptions() => _options;

        public abstract Task<Container<SelectionRange>> Handle(SelectionRangeParams request,
            CancellationToken cancellationToken);

        public virtual void SetCapability(SelectionRangeCapability capability) => Capability = capability;
        protected SelectionRangeCapability Capability { get; private set; }
    }

    public static class SelectionRangeExtensions
    {
        public static IDisposable OnSelectionRange(
            this ILanguageServerRegistry registry,
            Func<SelectionRangeParams, SelectionRangeCapability, CancellationToken, Task<Container<SelectionRange>>>
                handler,
            SelectionRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new SelectionRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.SelectionRange,
                new LanguageProtocolDelegatingHandlers.Request<SelectionRangeParams, Container<SelectionRange>, SelectionRangeCapability
                    ,
                    SelectionRangeRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnSelectionRange(
            this ILanguageServerRegistry registry,
            Func<SelectionRangeParams, CancellationToken, Task<Container<SelectionRange>>> handler,
            SelectionRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new SelectionRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.SelectionRange,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<SelectionRangeParams, Container<SelectionRange>,
                    SelectionRangeRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnSelectionRange(
            this ILanguageServerRegistry registry,
            Func<SelectionRangeParams, Task<Container<SelectionRange>>> handler,
            SelectionRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new SelectionRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.SelectionRange,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<SelectionRangeParams, Container<SelectionRange>,
                    SelectionRangeRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnSelectionRange(
            this ILanguageServerRegistry registry,
            Action<SelectionRangeParams, IObserver<Container<SelectionRange>>, SelectionRangeCapability,
                CancellationToken> handler,
            SelectionRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new SelectionRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.SelectionRange,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<SelectionRangeParams, Container<SelectionRange>,
                        SelectionRange, SelectionRangeCapability, SelectionRangeRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>()));
        }

        public static IDisposable OnSelectionRange(
            this ILanguageServerRegistry registry,
            Action<SelectionRangeParams, IObserver<Container<SelectionRange>>, SelectionRangeCapability>
                handler,
            SelectionRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new SelectionRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.SelectionRange,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<SelectionRangeParams, Container<SelectionRange>,
                        SelectionRange, SelectionRangeCapability, SelectionRangeRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>()));
        }

        public static IDisposable OnSelectionRange(
            this ILanguageServerRegistry registry,
            Action<SelectionRangeParams, IObserver<Container<SelectionRange>>, CancellationToken> handler,
            SelectionRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new SelectionRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.SelectionRange,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<SelectionRangeParams, Container<SelectionRange>,
                        SelectionRange, SelectionRangeRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>()));
        }

        public static IDisposable OnSelectionRange(
            this ILanguageServerRegistry registry,
            Action<SelectionRangeParams, IObserver<Container<SelectionRange>>> handler,
            SelectionRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new SelectionRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.SelectionRange,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<SelectionRangeParams, Container<SelectionRange>,
                        SelectionRange, SelectionRangeRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>()));
        }
    }
}
