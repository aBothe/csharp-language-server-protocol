using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public class ProgressManager : IProgressHandler, IProgressManager
    {
        protected IResponseRouter Router { get; private set; }
        protected ISerializer Serializer { get; private set; }

        private readonly ConcurrentDictionary<ProgressToken, ProgressObserver> _activeObservers
            = new ConcurrentDictionary<ProgressToken, ProgressObserver>();

        public void Initialized(IResponseRouter router, ISerializer serializer)
        {
            Router = router;
            Serializer = serializer;
        }

        /// <summary>
        /// Creates a <see cref="IObserver{WorkDoneProgressReport}" /> that will send all of its progress information to the same source.
        /// The other side can cancel this, so the <see cref="CancellationToken" /> should be respected.
        /// </summary>
        public ProgressObserver<T> For<T>(ProgressToken token, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                return ProgressObserver<T>.Noop;
            }

            if (_activeObservers.TryGetValue(token, out var item))
            {
                return (ProgressObserver<T>) item;
            }

            var observer = ProgressObserver.Create<T>(token, Router, Serializer, cancellationToken);
            _activeObservers.TryAdd(token, observer);

            observer.CancellationToken.Register(() => {
                if (_activeObservers.TryRemove(token, out var o) && o is ProgressObserver<T> po)
                    po.OnCompleted();
            });

            return observer;
        }

        public ProgressObserver<Container<T>> For<T>(IPartialResultParams<T> request,
            CancellationToken cancellationToken)
        {
            // This can be null.
            // If you use partial results then your final result must be empty as per the spec
            if (request.PartialResultToken == null) return null;
            return For<Container<T>>(request.PartialResultToken, cancellationToken);
        }

        public ProgressObserver<Container<T>> For<T>(IPartialItems<T> request,
            CancellationToken cancellationToken)
        {
            // This can be null.
            // If you use partial results then your final result must be empty as per the spec
            if (request.PartialResultToken == null) return null;
            return For<Container<T>>(request.PartialResultToken, cancellationToken);
        }

        public ProgressObserver<T> For<T>(IPartialItem<T> request, CancellationToken cancellationToken)
        {
            // This can be null.
            // If you use partial results then your final result must be empty as per the spec
            if (request.PartialResultToken == null) return null;
            return For<T>(request.PartialResultToken, cancellationToken);
        }

        Task<MediatR.Unit> IRequestHandler<ProgressParams, MediatR.Unit>.Handle(ProgressParams request,
            CancellationToken cancellationToken)
        {
            if (this._activeObservers.TryGetValue(request.Token, out var subject))
            {
                subject.OnNext(request.Value);
            }

            return MediatR.Unit.Task;
        }
    }
}
