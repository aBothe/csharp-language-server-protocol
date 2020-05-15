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
    [Serial, Method(ClientNames.RegisterCapability)]
    public interface IRegisterCapabilityHandler : IJsonRpcRequestHandler<RegistrationParams>
    {
    }

    public abstract class RegisterCapabilityHandler : IRegisterCapabilityHandler
    {
        public abstract Task<Unit> Handle(RegistrationParams request, CancellationToken cancellationToken);
    }

    public static class RegisterCapabilityHandlerExtensions
    {
        public static IDisposable OnRegisterCapability(this ILanguageClientRegistry registry,
            Func<RegistrationParams, CancellationToken, Task<Unit>> handler)
        {
            return registry.AddHandler(ClientNames.RegisterCapability, RequestHandler.For(handler));
        }

        public static IDisposable OnRegisterCapability(this ILanguageClientRegistry registry,
            Func<RegistrationParams, Task<Unit>> handler)
        {
            return registry.AddHandler(ClientNames.RegisterCapability, RequestHandler.For(handler));
        }
    }
}
