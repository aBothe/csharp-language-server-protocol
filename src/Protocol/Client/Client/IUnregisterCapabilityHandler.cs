using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Client
{
    [Serial, Method(ClientNames.UnregisterCapability)]
    public interface IUnregisterCapabilityHandler : IJsonRpcRequestHandler<UnregistrationParams> { }

    public abstract class UnregisterCapabilityHandler : IUnregisterCapabilityHandler
    {
        public abstract Task<Unit> Handle(UnregistrationParams request, CancellationToken cancellationToken);
    }

    public static class UnregisterCapabilityHandlerExtensions
    {
        public static IDisposable OnUnregisterCapability(this ILanguageClientRegistry registry,
            Func<UnregistrationParams, CancellationToken, Task<Unit>> handler)
        {
            return registry.AddHandler(ClientNames.UnregisterCapability, RequestHandler.For(handler));
        }

        public static IDisposable OnUnregisterCapability(this ILanguageClientRegistry registry,
            Func<UnregistrationParams, Task<Unit>> handler)
        {
            return registry.AddHandler(ClientNames.UnregisterCapability, RequestHandler.For(handler));
        }
    }
}
