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
    [Parallel, Method(TextDocumentNames.FoldingRange)]
    public interface IFoldingRangeHandler : IJsonRpcRequestHandler<FoldingRangeRequestParam, Container<FoldingRange>>,
        IRegistration<FoldingRangeRegistrationOptions>, ICapability<FoldingRangeCapability>
    {
    }

    public abstract class FoldingRangeHandler : IFoldingRangeHandler
    {
        private readonly FoldingRangeRegistrationOptions _options;
        protected IWorkDoneProgressManager ProgressManager { get; }

        public FoldingRangeHandler(FoldingRangeRegistrationOptions registrationOptions,
            IWorkDoneProgressManager progressManager)
        {
            _options = registrationOptions;
            ProgressManager = progressManager;
        }

        public FoldingRangeRegistrationOptions GetRegistrationOptions() => _options;

        public abstract Task<Container<FoldingRange>> Handle(FoldingRangeRequestParam request,
            CancellationToken cancellationToken);

        public virtual void SetCapability(FoldingRangeCapability capability) => Capability = capability;
        protected FoldingRangeCapability Capability { get; private set; }
    }

    public static class FoldingRangeHandlerExtensions
    {
        public static IDisposable OnFoldingRange(
            this ILanguageServerRegistry registry,
            Func<FoldingRangeRequestParam, FoldingRangeCapability, CancellationToken, Task<Container<FoldingRange>>>
                handler,
            FoldingRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new FoldingRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.FoldingRange,
                new LanguageProtocolDelegatingHandlers.Request<FoldingRangeRequestParam, Container<FoldingRange>, FoldingRangeCapability
                    ,
                    FoldingRangeRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnFoldingRange(
            this ILanguageServerRegistry registry,
            Func<FoldingRangeRequestParam, CancellationToken, Task<Container<FoldingRange>>> handler,
            FoldingRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new FoldingRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.FoldingRange,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<FoldingRangeRequestParam, Container<FoldingRange>,
                    FoldingRangeRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnFoldingRange(
            this ILanguageServerRegistry registry,
            Func<FoldingRangeRequestParam, Task<Container<FoldingRange>>> handler,
            FoldingRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new FoldingRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.FoldingRange,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<FoldingRangeRequestParam, Container<FoldingRange>,
                    FoldingRangeRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnFoldingRange(
            this ILanguageServerRegistry registry,
            Action<FoldingRangeRequestParam, IObserver<Container<FoldingRange>>, FoldingRangeCapability,
                CancellationToken> handler, FoldingRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new FoldingRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.FoldingRange,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<FoldingRangeRequestParam, Container<FoldingRange>,
                        FoldingRange, FoldingRangeCapability, FoldingRangeRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>()));
        }

        public static IDisposable OnFoldingRange(
            this ILanguageServerRegistry registry,
            Action<FoldingRangeRequestParam, IObserver<Container<FoldingRange>>, FoldingRangeCapability>
                handler,
            FoldingRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new FoldingRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.FoldingRange,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<FoldingRangeRequestParam, Container<FoldingRange>,
                        FoldingRange, FoldingRangeCapability, FoldingRangeRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>()));
        }

        public static IDisposable OnFoldingRange(
            this ILanguageServerRegistry registry,
            Action<FoldingRangeRequestParam, IObserver<Container<FoldingRange>>, CancellationToken> handler,
            FoldingRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new FoldingRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.FoldingRange,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<FoldingRangeRequestParam, Container<FoldingRange>,
                        FoldingRange, FoldingRangeRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>()));
        }

        public static IDisposable OnFoldingRange(
            this ILanguageServerRegistry registry,
            Action<FoldingRangeRequestParam, IObserver<Container<FoldingRange>>> handler,
            FoldingRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new FoldingRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.FoldingRange,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<FoldingRangeRequestParam, Container<FoldingRange>,
                        FoldingRange, FoldingRangeRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>()));
        }
    }
}
