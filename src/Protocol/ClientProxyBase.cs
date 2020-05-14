using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public interface IClientProxy : IResponseRouter, IServiceProvider
    {
    }

    public abstract class ClientProxyBase : IClientProxy
    {
        private readonly IResponseRouter _responseRouter;
        private readonly IServiceProvider _serviceProvider;

        public ClientProxyBase(IResponseRouter responseRouter, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _responseRouter = responseRouter;
        }

        public void SendNotification(string method) => _responseRouter.SendNotification(method);

        public void SendNotification<T>(string method, T @params) => _responseRouter.SendNotification(method, @params);

        public void SendNotification(IRequest request) => _responseRouter.SendNotification(request);

        public IResponseRouterReturns SendRequest<T>(string method, T @params) => _responseRouter.SendRequest(method, @params);

        public IResponseRouterReturns SendRequest(string method) => _responseRouter.SendRequest(method);

        public Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken) => _responseRouter.SendRequest(request, cancellationToken);

        public TaskCompletionSource<JToken> GetRequest(long id) => _responseRouter.GetRequest(id);
        object IServiceProvider.GetService(Type serviceType) => _serviceProvider.GetService(serviceType);
    }
}
