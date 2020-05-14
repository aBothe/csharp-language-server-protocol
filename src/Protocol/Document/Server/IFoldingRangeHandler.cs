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
    public interface IFoldingRangeHandler : IJsonRpcRequestHandler<FoldingRangeRequestParam, Container<FoldingRange>>, IRegistration<FoldingRangeRegistrationOptions>, ICapability<FoldingRangeCapability> { }

    public abstract class FoldingRangeHandler : IFoldingRangeHandler
    {
        private readonly FoldingRangeRegistrationOptions _options;
        protected IWorkDoneProgressManager ProgressManager { get; }
        public FoldingRangeHandler(FoldingRangeRegistrationOptions registrationOptions, IWorkDoneProgressManager progressManager)
        {
            _options = registrationOptions;
            ProgressManager = progressManager;
        }

        public FoldingRangeRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Container<FoldingRange>> Handle(FoldingRangeRequestParam request, CancellationToken cancellationToken);
        public virtual void SetCapability(FoldingRangeCapability capability) => Capability = capability;
        protected FoldingRangeCapability Capability { get; private set; }
    }

    public static class FoldingRangeHandlerExtensions
    {
        public static IDisposable OnFoldingRange(
            this ILanguageServerRegistry registry,
            Func<FoldingRangeRequestParam, CancellationToken, Task<Container<FoldingRange>>> handler,
            FoldingRangeRegistrationOptions registrationOptions = null,
            Action<FoldingRangeCapability> setCapability = null)
        {
            registrationOptions ??= new FoldingRangeRegistrationOptions();
            setCapability ??= x => { };
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : FoldingRangeHandler
        {
            private readonly Func<FoldingRangeRequestParam, CancellationToken, Task<Container<FoldingRange>>> _handler;
            private readonly Action<FoldingRangeCapability> _setCapability;

            public DelegatingHandler(
                Func<FoldingRangeRequestParam, CancellationToken, Task<Container<FoldingRange>>> handler,
                IWorkDoneProgressManager progressManager,
                Action<FoldingRangeCapability> setCapability,
                FoldingRangeRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Container<FoldingRange>> Handle(FoldingRangeRequestParam request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(FoldingRangeCapability capability) => _setCapability?.Invoke(capability);
        }
    }
}
