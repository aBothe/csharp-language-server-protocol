using System;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Shared
{
    public interface IHandlerTypeDescriptor
    {
        string Method { get; }
        Direction Direction { get; }
        RequestProcessType? RequestProcessType { get; }
        Type InterfaceType { get; }
        bool IsNotification {get;}
        bool IsRequest {get;}
        Type HandlerType { get; }
        bool HasParamsType { get; }
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
