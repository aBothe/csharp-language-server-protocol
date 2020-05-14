using System;
using System.Threading;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public interface IWorkDoneProgressManager : IProgressManager
    {
        void Initialized(IResponseRouter router, ISerializer serializer,
            WindowClientCapabilities windowClientCapabilities);

        bool IsSupported { get; }

        /// <summary>
        /// Creates a <see cref="IObserver{WorkDoneProgressReport}" /> that will send all of its progress information to the same source.
        /// The other side can cancel this, so the <see cref="CancellationToken" /> should be respected.
        /// </summary>
        ProgressObserver<WorkDoneProgressReport> Create(WorkDoneProgressBegin begin,
            Func<Exception, WorkDoneProgressEnd> onError = null, Func<WorkDoneProgressEnd> onComplete = null,
            CancellationToken? cancellationToken = null);

        /// <summary>
        /// Creates a <see cref="ProgressObserver" /> that will send all of its progress information to the same source.
        /// </summary>
        ProgressObserver<WorkDoneProgressReport> WorkDone(IWorkDoneProgressParams request,
            WorkDoneProgressBegin begin, Func<Exception, WorkDoneProgressEnd> onError = null,
            Func<WorkDoneProgressEnd> onComplete = null, CancellationToken? cancellationToken = null);
    }
}
