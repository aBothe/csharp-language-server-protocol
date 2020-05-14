using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(TextDocumentNames.Declaration)]
    public class DeclarationParams : WorkDoneTextDocumentPositionParams, IRequest<LocationOrLocationLinks>, IPartialItems<LocationLink>
    {
        /// <inheritdoc />
        public ProgressToken PartialResultToken { get; set; }
    }
}
