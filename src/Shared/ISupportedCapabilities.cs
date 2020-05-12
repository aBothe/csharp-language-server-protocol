using System;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageProtocolShared
{
    public interface ISupportedCapabilities
    {
        bool AllowsDynamicRegistration(Type capabilityType);
        void SetCapability(ILspHandlerDescriptor descriptor, IJsonRpcHandler handler);
    }
}