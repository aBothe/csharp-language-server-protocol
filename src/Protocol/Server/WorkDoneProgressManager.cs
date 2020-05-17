using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public class WorkDoneProgressManager : ProgressManager, IWorkDoneProgressCancelHandler, IWorkDoneProgressManager
    {
        private bool _supported;

        private readonly ConcurrentDictionary<ProgressToken, ProgressObserver> _activeObservers
            = new ConcurrentDictionary<ProgressToken, ProgressObserver>();

        public void Initialized(IResponseRouter router, ISerializer serializer,
            WindowClientCapabilities windowClientCapabilities)
        {
            _supported = windowClientCapabilities.WorkDoneProgress.IsSupported &&
                         windowClientCapabilities.WorkDoneProgress.Value;
        }

        public bool IsSupported => _supported;

        /// <summary>
        /// Creates a <see cref="IObserver{WorkDoneProgressReport}" /> that will send all of its progress information to the same source.
        /// The other side can cancel this, so the <see cref="CancellationToken" /> should be respected.
        /// </summary>
        public ProgressObserver<WorkDoneProgressReport> Create(WorkDoneProgressBegin begin,
            Func<Exception, WorkDoneProgressEnd> onError = null, Func<WorkDoneProgressEnd> onComplete = null,
            CancellationToken? cancellationToken = null)
        {
            if (!_supported)
            {
                return ProgressObserver<WorkDoneProgressReport>.Noop;
            }

            onError ??= error => new WorkDoneProgressEnd()
            {
                Message = error.ToString()
            };

            onComplete ??= () => new WorkDoneProgressEnd();

            var observer = ProgressObserver.CreateWorkDoneProgress(Router, Serializer, begin, onError, onComplete,
                cancellationToken ?? CancellationToken.None);
            _activeObservers.TryAdd(observer.Token, observer);
            var token = observer.Token;

            observer.CancellationToken.Register(() =>
            {
                if (_activeObservers.TryRemove(token, out var o))
                    o.OnCompleted();
            });

            return observer;
        }

        /// <summary>
        /// Creates a <see cref="ProgressObserver" /> that will send all of its progress information to the same source.
        /// </summary>
        public ProgressObserver<WorkDoneProgressReport> WorkDone(IWorkDoneProgressParams request,
            WorkDoneProgressBegin begin, Func<Exception, WorkDoneProgressEnd> onError = null,
            Func<WorkDoneProgressEnd> onComplete = null, CancellationToken? cancellationToken = null)
        {
            if (!_supported || request.WorkDoneToken == null)
            {
                return ProgressObserver<WorkDoneProgressReport>.Noop;
            }

            if (_activeObservers.TryGetValue(request.WorkDoneToken, out var item))
            {
                return (ProgressObserver<WorkDoneProgressReport>) item;
            }

            onError ??= error => new WorkDoneProgressEnd()
            {
                Message = error.ToString()
            };

            onComplete ??= () => new WorkDoneProgressEnd();

            var observer = ProgressObserver.CreateWorkDoneProgress(request.WorkDoneToken, Router, Serializer, begin,
                onError, onComplete, cancellationToken ?? CancellationToken.None);
            _activeObservers.TryAdd(observer.Token, observer);
            var token = observer.Token;

            observer.CancellationToken.Register(() =>
            {
                if (_activeObservers.TryRemove(token, out var o))
                    o.OnCompleted();
            });

            return observer;
        }

        Task<MediatR.Unit> IRequestHandler<WorkDoneProgressCancelParams, MediatR.Unit>.Handle(
            WorkDoneProgressCancelParams request, CancellationToken cancellationToken)
        {
            if (_activeObservers.TryGetValue(request.Token, out var item))
            {
                item.Dispose();
            }

            return MediatR.Unit.Task;
        }
    }
}
