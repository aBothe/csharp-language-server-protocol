using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static class AbstractHandlers
    {
        public abstract class Request<TParams, TResult, TCapability, TRegistrationOptions> :
            IJsonRpcRequestHandler<TParams, TResult>,
            IRegistration<TRegistrationOptions>, ICapability<TCapability>
            where TParams : IRequest<TResult>
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly TRegistrationOptions _registrationOptions;
            protected TCapability Capability { get; private set; }

            protected Request(TRegistrationOptions registrationOptions) => _registrationOptions = registrationOptions;

            public abstract Task<TResult> Handle(TParams request, CancellationToken cancellationToken);

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
            void ICapability<TCapability>.SetCapability(TCapability capability) => Capability = capability;
        }

        public abstract class PartialResult<TParams, TResult, TCapability, TRegistrationOptions> :
            IJsonRpcRequestHandler<TParams, TResult>,
            IRegistration<TRegistrationOptions>, ICapability<TCapability>
            where TParams : IRequest<TResult>, IPartialItem<TResult>
            where TResult : class, new()
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly TRegistrationOptions _registrationOptions;
            private readonly IProgressManager _progressManager;
            protected TCapability Capability { get; private set; }

            protected PartialResult(
                Action<TParams, IObserver<TResult>, CancellationToken> handler,
                TRegistrationOptions registrationOptions,
                IProgressManager progressManager)
            {
                _registrationOptions = registrationOptions;
                _progressManager = progressManager;
            }

            async Task<TResult> IRequestHandler<TParams, TResult>.Handle(TParams request,
                CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != null)
                {
                    Handle(request, observer, cancellationToken);
                    await observer.Completed;
                    return new TResult();
                }

                var subject = new AsyncSubject<TResult>();
                Handle(request, subject, cancellationToken);
                return await subject;
            }

            protected abstract void Handle(TParams request, IObserver<TResult> results,
                CancellationToken cancellationToken);

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
            void ICapability<TCapability>.SetCapability(TCapability capability) => Capability = capability;
        }

        public abstract class PartialResults<TParams, TResult, TItem, TCapability, TRegistrationOptions> :
            IJsonRpcRequestHandler<TParams, TResult>,
            IRegistration<TRegistrationOptions>, ICapability<TCapability>
            where TParams : IRequest<TResult>, IPartialItems<TItem>
            where TResult : class, new()
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly TRegistrationOptions _registrationOptions;
            private readonly IProgressManager _progressManager;
            protected TCapability Capability { get; private set; }

            protected PartialResults(
                TRegistrationOptions registrationOptions,
                IProgressManager progressManager)
            {
                _registrationOptions = registrationOptions;
                _progressManager = progressManager;
            }

            async Task<TResult> IRequestHandler<TParams, TResult>.Handle(TParams request,
                CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != null)
                {
                    Handle(request, observer, cancellationToken);
                    await observer.Completed;
                    return new TResult();
                }

                var subject = new Subject<Container<TItem>>();
                var task = subject.Aggregate(new List<TItem>(), (acc, items) => {
                        acc.AddRange(items);
                        return acc;
                    })
                    .Cast<IEnumerable<TItem>>()
                    .ToTask(cancellationToken);
                Handle(request, subject, cancellationToken);
                return (TResult) Activator.CreateInstance(typeof(TResult), new object[] {await task});
            }

            protected abstract void Handle(TParams request, IObserver<Container<TItem>> results,
                CancellationToken cancellationToken);

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
            void ICapability<TCapability>.SetCapability(TCapability capability) => Capability = capability;
        }

        public abstract class Notification<TParams, TCapability, TRegistrationOptions> :
            IJsonRpcRequestHandler<TParams>,
            IRegistration<TRegistrationOptions>, ICapability<TCapability>
            where TParams : IRequest
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly TRegistrationOptions _registrationOptions;
            protected TCapability Capability { get; private set; }

            protected Notification(
                TRegistrationOptions registrationOptions) =>
                _registrationOptions = registrationOptions;

            public Task<Unit> Handle(TParams request, CancellationToken cancellationToken)
            {
                Handle(request);
                return Unit.Task;
            }

            protected abstract void Handle(TParams request);

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
            void ICapability<TCapability>.SetCapability(TCapability capability) => Capability = capability;
        }
    }

    public static class LanguageProtocolDelegatingHandlers
    {
        public sealed class Request<TParams, TResult, TCapability, TRegistrationOptions> :
            IJsonRpcRequestHandler<TParams, TResult>,
            IRegistration<TRegistrationOptions>, ICapability<TCapability>
            where TParams : IRequest<TResult>
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly Func<TParams, TCapability, CancellationToken, Task<TResult>> _handler;
            private readonly TRegistrationOptions _registrationOptions;
            private TCapability _capability;

            public Request(
                Func<TParams, TCapability, Task<TResult>> handler,
                TRegistrationOptions registrationOptions) : this((a, c, ct) => handler(a, c), registrationOptions)
            {
            }

            public Request(
                Func<TParams, TCapability, CancellationToken, Task<TResult>> handler,
                TRegistrationOptions registrationOptions)
            {
                _handler = handler;
                _registrationOptions = registrationOptions;
            }

            Task<TResult> IRequestHandler<TParams, TResult>.
                Handle(TParams request, CancellationToken cancellationToken) =>
                _handler(request, _capability, cancellationToken);

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
            void ICapability<TCapability>.SetCapability(TCapability capability) => _capability = capability;
        }

        public sealed class CanBeResolved<TItem, TCapability, TRegistrationOptions> :
            IRegistration<TRegistrationOptions>,
            ICapability<TCapability>,
            ICanBeResolvedHandler<TItem>
            where TItem : ICanBeResolved, IRequest<TItem>
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly Func<TItem, bool> _canResolve;
            private readonly Func<TItem, TCapability, CancellationToken, Task<TItem>> _resolveHandler;
            private readonly TRegistrationOptions _registrationOptions;
            private TCapability _capability;

            public CanBeResolved(
                Func<TItem, TCapability, Task<TItem>> resolveHandler,
                Func<TItem, bool> canResolve,
                TRegistrationOptions registrationOptions) : this(
                (a, c, ct) => resolveHandler(a, c) ,
                canResolve,
                registrationOptions)
            {
            }

            public CanBeResolved(
                Func<TItem, TCapability, CancellationToken, Task<TItem>> resolveHandler,
                Func<TItem, bool> canResolve,
                TRegistrationOptions registrationOptions)
            {
                _canResolve = canResolve;
                _resolveHandler = resolveHandler;
                _registrationOptions = registrationOptions;
            }

            Task<TItem> IRequestHandler<TItem, TItem>.
                Handle(TItem request, CancellationToken cancellationToken) =>
                _resolveHandler(request, _capability, cancellationToken);

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
            void ICapability<TCapability>.SetCapability(TCapability capability) => _capability = capability;
            bool ICanBeResolvedHandler<TItem>.CanResolve(TItem value) => _canResolve(value);
        }

        public sealed class CanBeResolved<TItem, TRegistrationOptions> :
            IRegistration<TRegistrationOptions>,
            ICanBeResolvedHandler<TItem>
            where TItem : ICanBeResolved, IRequest<TItem>
            where TRegistrationOptions : class, new()
        {
            private readonly Func<TItem, bool> _canResolve;
            private readonly Func<TItem, CancellationToken, Task<TItem>> _resolveHandler;
            private readonly TRegistrationOptions _registrationOptions;

            public CanBeResolved(
                Func<TItem, Task<TItem>> resolveHandler,
                Func<TItem, bool> canResolve,
                TRegistrationOptions registrationOptions) : this(
                (a, c) => resolveHandler(a) ,
                canResolve,
                registrationOptions)
            {
            }

            public CanBeResolved(
                Func<TItem, CancellationToken, Task<TItem>> resolveHandler,
                Func<TItem, bool> canResolve,
                TRegistrationOptions registrationOptions)
            {
                _canResolve = canResolve;
                _resolveHandler = resolveHandler;
                _registrationOptions = registrationOptions;
            }

            Task<TItem> IRequestHandler<TItem, TItem>.
                Handle(TItem request, CancellationToken cancellationToken) =>
                _resolveHandler(request, cancellationToken);

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
            bool ICanBeResolvedHandler<TItem>.CanResolve(TItem value) => _canResolve(value);
        }

        public sealed class Request<TParams, TCapability, TRegistrationOptions> :
            IJsonRpcRequestHandler<TParams>,
            IRegistration<TRegistrationOptions>, ICapability<TCapability>
            where TParams : IRequest
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly Func<TParams, TCapability, CancellationToken, Task> _handler;
            private readonly TRegistrationOptions _registrationOptions;
            private TCapability _capability;

            public Request(
                Func<TParams, TCapability, Task> handler,
                TRegistrationOptions registrationOptions) : this((a, c, ct) => handler(a, c), registrationOptions)
            {
            }

            public Request(
                Func<TParams, TCapability, CancellationToken, Task> handler,
                TRegistrationOptions registrationOptions)
            {
                _handler = handler;
                _registrationOptions = registrationOptions;
            }

            async Task<Unit> IRequestHandler<TParams, Unit>.
                Handle(TParams request, CancellationToken cancellationToken)
            {
                await _handler(request, _capability, cancellationToken);
                return Unit.Value;
                ;
            }

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
            void ICapability<TCapability>.SetCapability(TCapability capability) => _capability = capability;
        }

        public sealed class RequestRegistration<TParams, TResult, TRegistrationOptions> :
            IJsonRpcRequestHandler<TParams, TResult>,
            IRegistration<TRegistrationOptions>
            where TParams : IRequest<TResult>
            where TRegistrationOptions : class, new()
        {
            private readonly Func<TParams, CancellationToken, Task<TResult>> _handler;
            private readonly TRegistrationOptions _registrationOptions;

            public RequestRegistration(
                Func<TParams, Task<TResult>> handler,
                TRegistrationOptions registrationOptions) : this((a, ct) => handler(a), registrationOptions)
            {
            }

            public RequestRegistration(
                Func<TParams, CancellationToken, Task<TResult>> handler,
                TRegistrationOptions registrationOptions)
            {
                _handler = handler;
                _registrationOptions = registrationOptions;
            }

            Task<TResult> IRequestHandler<TParams, TResult>.
                Handle(TParams request, CancellationToken cancellationToken) =>
                _handler(request, cancellationToken);

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
        }

        public sealed class RequestRegistration<TParams, TRegistrationOptions> :
            IJsonRpcRequestHandler<TParams>,
            IRegistration<TRegistrationOptions>
            where TParams : IRequest
            where TRegistrationOptions : class, new()
        {
            private readonly Func<TParams, CancellationToken, Task> _handler;
            private readonly TRegistrationOptions _registrationOptions;

            public RequestRegistration(
                Func<TParams, Task> handler,
                TRegistrationOptions registrationOptions) : this((a, ct) => handler(a), registrationOptions)
            {
            }

            public RequestRegistration(
                Func<TParams, CancellationToken, Task> handler,
                TRegistrationOptions registrationOptions)
            {
                _handler = handler;
                _registrationOptions = registrationOptions;
            }

            async Task<Unit> IRequestHandler<TParams, Unit>.
                Handle(TParams request, CancellationToken cancellationToken)
            {
                await _handler(request, cancellationToken);
                return Unit.Value;
            }

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
        }

        public sealed class RequestCapability<TParams, TResult, TCapability> :
            IJsonRpcRequestHandler<TParams, TResult>,
            ICapability<TCapability>
            where TParams : IRequest<TResult>
            where TCapability : ICapability
        {
            private readonly Func<TParams, TCapability, CancellationToken, Task<TResult>> _handler;
            private TCapability _capability;

            public RequestCapability(
                Func<TParams, TCapability, Task<TResult>> handler) : this((a, c, ct) => handler(a, c))
            {
            }

            public RequestCapability(
                Func<TParams, TCapability, CancellationToken, Task<TResult>> handler)
            {
                _handler = handler;
            }

            Task<TResult> IRequestHandler<TParams, TResult>.
                Handle(TParams request, CancellationToken cancellationToken) =>
                _handler(request, _capability, cancellationToken);

            void ICapability<TCapability>.SetCapability(TCapability capability) => _capability = capability;
        }

        public sealed class RequestCapability<TParams, TCapability> :
            IJsonRpcRequestHandler<TParams>,
            ICapability<TCapability>
            where TParams : IRequest
            where TCapability : ICapability
        {
            private readonly Func<TParams, TCapability, CancellationToken, Task> _handler;
            private TCapability _capability;

            public RequestCapability(
                Func<TParams, TCapability, Task> handler) : this((a, c, ct) => handler(a, c))
            {
            }

            public RequestCapability(
                Func<TParams, TCapability, CancellationToken, Task> handler)
            {
                _handler = handler;
            }

            async Task<Unit> IRequestHandler<TParams, Unit>.
                Handle(TParams request, CancellationToken cancellationToken)
            {
                await _handler(request, _capability, cancellationToken);
                return Unit.Value;
            }

            void ICapability<TCapability>.SetCapability(TCapability capability) => _capability = capability;
        }

        public sealed class PartialResult<TParams, TResult, TCapability, TRegistrationOptions> :
            IJsonRpcRequestHandler<TParams, TResult>,
            IRegistration<TRegistrationOptions>, ICapability<TCapability>
            where TParams : IRequest<TResult>, IPartialItem<TResult>
            where TResult : class, new()
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly Action<TParams, IObserver<TResult>, TCapability, CancellationToken> _handler;
            private readonly TRegistrationOptions _registrationOptions;
            private readonly IProgressManager _progressManager;
            private TCapability _capability;

            public PartialResult(
                Action<TParams, IObserver<TResult>, TCapability> handler,
                TRegistrationOptions registrationOptions,
                IProgressManager progressManager) : this((p, o, c, ct) => handler(p, o, c), registrationOptions,
                progressManager)
            {
            }

            public PartialResult(
                Action<TParams, IObserver<TResult>, TCapability, CancellationToken> handler,
                TRegistrationOptions registrationOptions,
                IProgressManager progressManager)
            {
                _handler = handler;
                _registrationOptions = registrationOptions;
                _progressManager = progressManager;
            }

            async Task<TResult> IRequestHandler<TParams, TResult>.Handle(TParams request,
                CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != null)
                {
                    _handler(request, observer, _capability, cancellationToken);
                    await observer.Completed;
                    return new TResult();
                }

                var subject = new AsyncSubject<TResult>();
                _handler(request, subject, _capability, cancellationToken);
                return await subject;
            }

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
            void ICapability<TCapability>.SetCapability(TCapability capability) => _capability = capability;
        }

        public sealed class PartialResult<TParams, TResult, TRegistrationOptions> :
            IJsonRpcRequestHandler<TParams, TResult>,
            IRegistration<TRegistrationOptions>
            where TParams : IRequest<TResult>, IPartialItem<TResult>
            where TResult : class, new()
            where TRegistrationOptions : class, new()
        {
            private readonly Action<TParams, IObserver<TResult>, CancellationToken> _handler;
            private readonly TRegistrationOptions _registrationOptions;
            private readonly IProgressManager _progressManager;

            public PartialResult(
                Action<TParams, IObserver<TResult>> handler,
                TRegistrationOptions registrationOptions,
                IProgressManager progressManager) : this((p, o, ct) => handler(p, o), registrationOptions,
                progressManager)
            {
            }

            public PartialResult(
                Action<TParams, IObserver<TResult>, CancellationToken> handler,
                TRegistrationOptions registrationOptions,
                IProgressManager progressManager)
            {
                _handler = handler;
                _registrationOptions = registrationOptions;
                _progressManager = progressManager;
            }

            async Task<TResult> IRequestHandler<TParams, TResult>.Handle(TParams request,
                CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != null)
                {
                    _handler(request, observer, cancellationToken);
                    await observer.Completed;
                    return new TResult();
                }

                var subject = new AsyncSubject<TResult>();
                _handler(request, subject, cancellationToken);
                return await subject;
            }

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
        }

        public sealed class PartialResultCapability<TParams, TResult, TCapability> :
            IJsonRpcRequestHandler<TParams, TResult>,
            ICapability<TCapability>
            where TParams : IRequest<TResult>, IPartialItem<TResult>
            where TResult : class, new()
            where TCapability : ICapability
        {
            private readonly Action<TParams, TCapability, IObserver<TResult>, CancellationToken> _handler;
            private readonly IProgressManager _progressManager;
            private TCapability _capability;

            public PartialResultCapability(
                Action<TParams, TCapability, IObserver<TResult>> handler,
                IProgressManager progressManager) : this((p, c, o, ct) => handler(p, c, o),
                progressManager)
            {
            }

            public PartialResultCapability(
                Action<TParams, TCapability, IObserver<TResult>, CancellationToken> handler,
                IProgressManager progressManager)
            {
                _handler = handler;
                _progressManager = progressManager;
            }

            async Task<TResult> IRequestHandler<TParams, TResult>.Handle(TParams request,
                CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != null)
                {
                    _handler(request, _capability, observer, cancellationToken);
                    await observer.Completed;
                    return new TResult();
                }

                var subject = new AsyncSubject<TResult>();
                _handler(request, _capability, subject, cancellationToken);
                return await subject;
            }

            void ICapability<TCapability>.SetCapability(TCapability capability) => _capability = capability;
        }

        public sealed class PartialResult<TParams, TResult> :
            IJsonRpcRequestHandler<TParams, TResult>
            where TParams : IRequest<TResult>, IPartialItem<TResult>
            where TResult : class, new()
        {
            private readonly Action<TParams, IObserver<TResult>, CancellationToken> _handler;
            private readonly IProgressManager _progressManager;

            public PartialResult(
                Action<TParams, IObserver<TResult>> handler,
                IProgressManager progressManager) : this((p, o, ct) => handler(p, o),
                progressManager)
            {
            }

            public PartialResult(
                Action<TParams, IObserver<TResult>, CancellationToken> handler,
                IProgressManager progressManager)
            {
                _handler = handler;
                _progressManager = progressManager;
            }

            async Task<TResult> IRequestHandler<TParams, TResult>.Handle(TParams request,
                CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != null)
                {
                    _handler(request, observer, cancellationToken);
                    await observer.Completed;
                    return new TResult();
                }

                var subject = new AsyncSubject<TResult>();
                _handler(request, subject, cancellationToken);
                return await subject;
            }
        }

        public sealed class PartialResults<TParams, TResult, TItem, TCapability, TRegistrationOptions> :
            IJsonRpcRequestHandler<TParams, TResult>,
            IRegistration<TRegistrationOptions>, ICapability<TCapability>
            where TParams : IRequest<TResult>, IPartialItems<TItem>
            where TResult : class, new()
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly Action<TParams, IObserver<Container<TItem>>, TCapability, CancellationToken> _handler;
            private readonly TRegistrationOptions _registrationOptions;
            private readonly IProgressManager _progressManager;
            private TCapability _capability;

            public PartialResults(
                Action<TParams, IObserver<Container<TItem>>, TCapability> handler,
                TRegistrationOptions registrationOptions,
                IProgressManager progressManager) : this((p, o, c, ct) => handler(p, o, c), registrationOptions,
                progressManager)
            {
            }

            public PartialResults(
                Action<TParams, IObserver<Container<TItem>>, TCapability, CancellationToken> handler,
                TRegistrationOptions registrationOptions,
                IProgressManager progressManager)
            {
                _handler = handler;
                _registrationOptions = registrationOptions;
                _progressManager = progressManager;
            }

            async Task<TResult> IRequestHandler<TParams, TResult>.Handle(TParams request,
                CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != null)
                {
                    _handler(request, observer, _capability, cancellationToken);
                    await observer.Completed;
                    return new TResult();
                }

                var subject = new Subject<Container<TItem>>();
                var task = subject.Aggregate(new List<TItem>(), (acc, items) => {
                        acc.AddRange(items);
                        return acc;
                    })
                    .Cast<IEnumerable<TItem>>()
                    .ToTask(cancellationToken);
                _handler(request, subject, _capability, cancellationToken);
                return (TResult) Activator.CreateInstance(typeof(TResult), new object[] {await task});
            }

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
            void ICapability<TCapability>.SetCapability(TCapability capability) => _capability = capability;
        }

        public sealed class PartialResults<TParams, TResult, TItem, TRegistrationOptions> :
            IJsonRpcRequestHandler<TParams, TResult>,
            IRegistration<TRegistrationOptions>
            where TParams : IRequest<TResult>, IPartialItems<TItem>
            where TResult : class, new()
            where TRegistrationOptions : class, new()
        {
            private readonly Action<TParams, IObserver<Container<TItem>>, CancellationToken> _handler;
            private readonly TRegistrationOptions _registrationOptions;
            private readonly IProgressManager _progressManager;

            public PartialResults(
                Action<TParams, IObserver<Container<TItem>>> handler,
                TRegistrationOptions registrationOptions,
                IProgressManager progressManager) : this((p, o, ct) => handler(p, o), registrationOptions,
                progressManager)
            {
            }

            public PartialResults(
                Action<TParams, IObserver<Container<TItem>>, CancellationToken> handler,
                TRegistrationOptions registrationOptions,
                IProgressManager progressManager)
            {
                _handler = handler;
                _registrationOptions = registrationOptions;
                _progressManager = progressManager;
            }

            async Task<TResult> IRequestHandler<TParams, TResult>.Handle(TParams request,
                CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != null)
                {
                    _handler(request, observer, cancellationToken);
                    await observer.Completed;
                    return new TResult();
                }

                var subject = new Subject<Container<TItem>>();
                var task = subject.Aggregate(new List<TItem>(), (acc, items) => {
                        acc.AddRange(items);
                        return acc;
                    })
                    .Cast<IEnumerable<TItem>>()
                    .ToTask(cancellationToken);
                _handler(request, subject, cancellationToken);
                return (TResult) Activator.CreateInstance(typeof(TResult), new object[] {await task});
            }

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
        }

        public sealed class PartialResultsCapability<TParams, TResult, TItem, TCapability> :
            IJsonRpcRequestHandler<TParams, TResult>, ICapability<TCapability>
            where TParams : IRequest<TResult>, IPartialItems<TItem>
            where TResult : class, new()
            where TCapability : ICapability
        {
            private readonly Action<TParams, TCapability, IObserver<Container<TItem>>, CancellationToken> _handler;
            private readonly IProgressManager _progressManager;
            private TCapability _capability;

            public PartialResultsCapability(
                Action<TParams, TCapability, IObserver<Container<TItem>>> handler,
                IProgressManager progressManager) : this((p, c, o, ct) => handler(p, c, o),
                progressManager)
            {
            }

            public PartialResultsCapability(
                Action<TParams, TCapability, IObserver<Container<TItem>>, CancellationToken> handler,
                IProgressManager progressManager)
            {
                _handler = handler;
                _progressManager = progressManager;
            }

            async Task<TResult> IRequestHandler<TParams, TResult>.Handle(TParams request,
                CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != null)
                {
                    _handler(request, _capability, observer, cancellationToken);
                    await observer.Completed;
                    return new TResult();
                }

                var subject = new Subject<Container<TItem>>();
                var task = subject.Aggregate(new List<TItem>(), (acc, items) => {
                        acc.AddRange(items);
                        return acc;
                    })
                    .Cast<IEnumerable<TItem>>()
                    .ToTask(cancellationToken);
                _handler(request, _capability, subject, cancellationToken);
                return (TResult) Activator.CreateInstance(typeof(TResult), new object[] {await task});
            }

            void ICapability<TCapability>.SetCapability(TCapability capability) => _capability = capability;
        }

        public sealed class PartialResults<TParams, TResult, TItem> :
            IJsonRpcRequestHandler<TParams, TResult>
            where TParams : IRequest<TResult>, IPartialItems<TItem>
            where TResult : class, new()
        {
            private readonly Action<TParams, IObserver<Container<TItem>>, CancellationToken> _handler;
            private readonly IProgressManager _progressManager;

            public PartialResults(
                Action<TParams, IObserver<Container<TItem>>> handler,
                IProgressManager progressManager) : this((p, o, ct) => handler(p, o),
                progressManager)
            {
            }

            public PartialResults(
                Action<TParams, IObserver<Container<TItem>>, CancellationToken> handler,
                IProgressManager progressManager)
            {
                _handler = handler;
                _progressManager = progressManager;
            }

            async Task<TResult> IRequestHandler<TParams, TResult>.Handle(TParams request,
                CancellationToken cancellationToken)
            {
                var observer = _progressManager.For(request, cancellationToken);
                if (observer != null)
                {
                    _handler(request, observer, cancellationToken);
                    await observer.Completed;
                    return new TResult();
                }

                var subject = new Subject<Container<TItem>>();
                var task = subject.Aggregate(new List<TItem>(), (acc, items) => {
                        acc.AddRange(items);
                        return acc;
                    })
                    .Cast<IEnumerable<TItem>>()
                    .ToTask(cancellationToken);
                _handler(request, subject, cancellationToken);
                return (TResult) Activator.CreateInstance(typeof(TResult), new object[] {await task});
            }
        }

        public sealed class Notification<TParams, TCapability, TRegistrationOptions> :
            IJsonRpcRequestHandler<TParams>,
            IRegistration<TRegistrationOptions>, ICapability<TCapability>
            where TParams : IRequest
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            private readonly Func<TParams, TCapability, CancellationToken, Task> _handler;
            private readonly TRegistrationOptions _registrationOptions;
            private TCapability _capability;

            public Notification(
                Action<TParams, TCapability> handler,
                TRegistrationOptions registrationOptions) : this((request, capability, ct) => {
                handler(request, capability);
                return Task.CompletedTask;
            }, registrationOptions)
            {
            }

            public Notification(
                Func<TParams, TCapability, Task> handler,
                TRegistrationOptions registrationOptions) : this((request, capability, ct) => handler(request, capability), registrationOptions)
            {
            }

            public Notification(
                Func<TParams, TCapability, CancellationToken, Task> handler,
                TRegistrationOptions registrationOptions)
            {
                _handler = handler;
                _registrationOptions = registrationOptions;
            }

            async Task<Unit> IRequestHandler<TParams, Unit>.Handle(TParams request, CancellationToken cancellationToken)
            {
                await                _handler(request, _capability, cancellationToken);
                return Unit.Value;
            }

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
            void ICapability<TCapability>.SetCapability(TCapability capability) => _capability = capability;
        }

        public sealed class Notification<TParams, TRegistrationOptions> :
            IJsonRpcRequestHandler<TParams>,
            IRegistration<TRegistrationOptions>
            where TParams : IRequest
            where TRegistrationOptions : class, new()
        {
            private readonly Func<TParams, CancellationToken, Task> _handler;
            private readonly TRegistrationOptions _registrationOptions;

            public Notification(
                Action<TParams> handler,
                TRegistrationOptions registrationOptions) : this((request, ct) => {
                handler(request);
                return Task.CompletedTask;
            }, registrationOptions)
            {
            }

            public Notification(
                Func<TParams, Task> handler,
                TRegistrationOptions registrationOptions) : this((request, ct) => handler(request), registrationOptions)
            {
            }

            public Notification(
                Func<TParams, CancellationToken, Task> handler,
                TRegistrationOptions registrationOptions)
            {
                _handler = handler;
                _registrationOptions = registrationOptions;
            }

            async Task<Unit> IRequestHandler<TParams, Unit>.Handle(TParams request, CancellationToken cancellationToken)
            {
                await _handler(request, cancellationToken);
                return Unit.Value;
            }

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions() => _registrationOptions;
        }

        public sealed class NotificationCapability<TParams, TCapability> :
            IJsonRpcRequestHandler<TParams>, ICapability<TCapability>
            where TParams : IRequest
            where TCapability : ICapability
        {
            private readonly Func<TParams, TCapability, CancellationToken, Task> _handler;
            private TCapability _capability;

            public NotificationCapability(
                Action<TParams, TCapability> handler) : this((request, capability, ct) => {
                handler(request, capability);
                return Task.CompletedTask;
            })
            {
            }

            public NotificationCapability(
                Func<TParams, TCapability, Task> handler) : this((request, capability, ct) => handler(request, capability))
            {
            }

            public NotificationCapability(
                Func<TParams, TCapability, CancellationToken, Task> handler)
            {
                _handler = handler;
            }

            async Task<Unit> IRequestHandler<TParams, Unit>.Handle(TParams request, CancellationToken cancellationToken)
            {
                await _handler(request, _capability, cancellationToken);
                return Unit.Value;
            }

            void ICapability<TCapability>.SetCapability(TCapability capability) => _capability = capability;
        }
    }
}
