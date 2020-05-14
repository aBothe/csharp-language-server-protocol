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
    public interface ISelectionRangeHandler : IJsonRpcRequestHandler<SelectionRangeParam, Container<SelectionRange>>, IRegistration<SelectionRangeRegistrationOptions>, ICapability<SelectionRangeCapability> { }

    public abstract class SelectionRangeHandler : ISelectionRangeHandler
    {
        private readonly SelectionRangeRegistrationOptions _options;
        protected IWorkDoneProgressManager ProgressManager { get; }
        public SelectionRangeHandler(SelectionRangeRegistrationOptions registrationOptions, IWorkDoneProgressManager progressManager)
        {
            _options = registrationOptions;
            ProgressManager = progressManager;
        }

        public SelectionRangeRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Container<SelectionRange>> Handle(SelectionRangeParam request, CancellationToken cancellationToken);
        public virtual void SetCapability(SelectionRangeCapability capability) => Capability = capability;
        protected SelectionRangeCapability Capability { get; private set; }
    }

    public static class SelectionRangeHandlerExtensions
    {
        public static IDisposable OnSelectionRange(
            this ILanguageServerRegistry registry,
            Func<SelectionRangeParam, CancellationToken, Task<Container<SelectionRange>>> handler,
            SelectionRangeRegistrationOptions registrationOptions = null,
            Action<SelectionRangeCapability> setCapability = null)
        {
            registrationOptions ??= new SelectionRangeRegistrationOptions();
            setCapability ??= x => { };
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : SelectionRangeHandler
        {
            private readonly Func<SelectionRangeParam, CancellationToken, Task<Container<SelectionRange>>> _handler;
            private readonly Action<SelectionRangeCapability> _setCapability;

            public DelegatingHandler(
                Func<SelectionRangeParam, CancellationToken, Task<Container<SelectionRange>>> handler,
                IWorkDoneProgressManager progressManager,
                Action<SelectionRangeCapability> setCapability,
                SelectionRangeRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Container<SelectionRange>> Handle(SelectionRangeParam request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(SelectionRangeCapability capability) => _setCapability?.Invoke(capability);
        }
    }
}
