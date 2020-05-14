using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(TextDocumentNames.DocumentHighlight)]
    public class DocumentHighlightParams : WorkDoneTextDocumentPositionParams, IRequest<DocumentHighlightContainer>, IPartialItems<DocumentHighlight>
    {
        /// <inheritdoc />
        [Optional]
        public ProgressToken PartialResultToken { get; set; }
    }
}
