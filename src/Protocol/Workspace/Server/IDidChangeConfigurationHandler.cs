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
    [Serial, Method(WorkspaceNames.DidChangeConfiguration)]
    public interface IDidChangeConfigurationHandler : IJsonRpcNotificationHandler<DidChangeConfigurationParams>, IRegistration<object>, ICapability<DidChangeConfigurationCapability> { }

    public abstract class DidChangeConfigurationHandler : IDidChangeConfigurationHandler
    {
        public object GetRegistrationOptions() => new object();
        public abstract Task<Unit> Handle(DidChangeConfigurationParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DidChangeConfigurationCapability capability) => Capability = capability;
        protected DidChangeConfigurationCapability Capability { get; private set; }
    }

    public static class DidChangeConfigurationHandlerExtensions
    {
        public static IDisposable OnDidChangeConfiguration(
            this ILanguageServerRegistry registry,
            Func<DidChangeConfigurationParams, CancellationToken, Task<Unit>> handler,
            Action<DidChangeConfigurationCapability> setCapability = null)
        {
            setCapability ??= x => { };
            return registry.AddHandler(_ => ActivatorUtilities.CreateInstance<DelegatingHandler>(_, handler, setCapability));
        }

        class DelegatingHandler : DidChangeConfigurationHandler
        {
            private readonly Func<DidChangeConfigurationParams, CancellationToken, Task<Unit>> _handler;
            private readonly Action<DidChangeConfigurationCapability> _setCapability;

            public DelegatingHandler(
                Func<DidChangeConfigurationParams, CancellationToken, Task<Unit>> handler,
                Action<DidChangeConfigurationCapability> setCapability) : base()
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Unit> Handle(DidChangeConfigurationParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(DidChangeConfigurationCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
