using System;

namespace OmniSharp.Extensions.LanguageServer.Shared
{
    public interface IHandlerTypeDescriptor
    {
        string Method { get; }
        Type InterfaceType { get; }
        Type HandlerType { get; }
        Type ParamsType { get; }
        bool HasResponseType { get; }
        Type ResponseType { get; }
        bool HasRegistration { get; }
        Type RegistrationType { get; }
        bool HasCapability { get; }
        Type CapabilityType { get; }
        bool IsDynamicCapability { get; }
    }
}
