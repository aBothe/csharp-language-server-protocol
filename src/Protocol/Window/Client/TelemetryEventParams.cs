using System.Collections.Generic;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Client
{
    [Method(WindowNames.TelemetryEvent)]
    public class TelemetryEventParams : IRequest
    {
        [JsonExtensionData]
        private IDictionary<string, JToken> Data { get; set; }
    }
}
