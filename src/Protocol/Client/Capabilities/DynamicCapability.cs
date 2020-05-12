using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public interface IDynamicCapability
    {
        /// <summary>
        /// Whether completion supports dynamic registration.
        /// </summary>
        [Optional]
        bool DynamicRegistration { get; set; }
    }
    public class DynamicCapability : IDynamicCapability
    {
        /// <summary>
        /// Whether completion supports dynamic registration.
        /// </summary>
        [Optional]
        public bool DynamicRegistration { get; set; }
    }
}
