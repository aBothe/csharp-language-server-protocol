using System;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageProtocolShared
{
    public interface ILspHandlerDescriptor : IHandlerDescriptor
    {
        Guid Id { get; }
        bool HasRegistration { get; }
        Type RegistrationType { get; }
        object RegistrationOptions { get; }
        bool AllowsDynamicRegistration { get; }

        bool HasCapability { get; }
        Type CapabilityType { get; }
        bool IsDynamicCapability { get; }
        Type CanBeResolvedHandlerType { get; }
        OnServerStartedDelegate OnServerStartedDelegate { get; }
        OnClientStartedDelegate OnClientStartedDelegate { get; }
    }
}
