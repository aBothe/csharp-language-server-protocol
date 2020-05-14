using System;
using System.Threading;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public interface IProgressManager
    {
        void Initialized(IResponseRouter router, ISerializer serializer);

        /// <summary>
        /// Creates a <see cref="IObserver{T}" /> that will send all of its progress information to the same source.
        /// The other side can cancel this, so the <see cref="CancellationToken" /> should be respected.
        /// </summary>
        ProgressObserver<T> For<T>(ProgressToken token, CancellationToken? cancellationToken = null);

        ProgressObserver<Container<T>> For<T>(IPartialResultParams<T> request,
            CancellationToken? cancellationToken = null);

        ProgressObserver<Container<T>> For<T>(IPartialItems<T> request,
            CancellationToken? cancellationToken = null);

        ProgressObserver<T> For<T>(IPartialItem<T> request, CancellationToken? cancellationToken = null);
    }
}
