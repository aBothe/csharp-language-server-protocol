using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Server
{
    /// <summary>
    /// InitializeError
    /// </summary>
    [Serial, Method(GeneralNames.Initialize)]
    public interface IInitializeHandler : IJsonRpcRequestHandler<InitializeParams, InitializeResult> { }

    public abstract class InitializeHandler : IInitializeHandler
    {
        public abstract Task<InitializeResult> Handle(InitializeParams request, CancellationToken cancellationToken);
    }

    public static class InitializeHandlerExtensions
    {
        public static IDisposable OnInitialize(
            this ILanguageServerRegistry registry,
            Func<InitializeParams, CancellationToken, Task<InitializeResult>>
                handler)
        {
            return registry.AddHandler(GeneralNames.Initialize, RequestHandler.For(handler));
        }

        public static IDisposable OnInitialize(
            this ILanguageServerRegistry registry,
            Func<InitializeParams, Task<InitializeResult>> handler)
        {
            return registry.AddHandler(GeneralNames.Initialize, RequestHandler.For(handler));
        }
    }
}
