using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Shared
{
    [DebuggerDisplay("{" + nameof(Method) + "}")]
    class HandlerTypeDescriptor : IHandlerTypeDescriptor
    {
        public HandlerTypeDescriptor(Type handlerType)
        {
            var method = handlerType.GetCustomAttribute<MethodAttribute>();
            Method = method.Method;
            Direction = method.Direction;
            HandlerType = handlerType;
            InterfaceType = HandlerTypeDescriptorHelper.GetHandlerInterface(handlerType);

            ParamsType = InterfaceType.IsGenericType ? InterfaceType.GetGenericArguments()[0] : typeof(EmptyRequest);
            HasParamsType = ParamsType != null && ParamsType != typeof(EmptyRequest);

            IsNotification = typeof(IJsonRpcNotificationHandler).IsAssignableFrom(handlerType) || handlerType
                .GetInterfaces().Any(z =>
                    z.IsGenericType && typeof(IJsonRpcNotificationHandler<>).IsAssignableFrom(z.GetGenericTypeDefinition()));
            IsRequest = !IsNotification;

            var requestInterface = ParamsType?
                .GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>));
            if (requestInterface != null)
                ResponseType = requestInterface.GetGenericArguments()[0];
            HasResponseType = ResponseType != null && ResponseType != typeof(Unit);
            RegistrationType = HandlerTypeDescriptorHelper.UnwrapGenericType(typeof(IRegistration<>), handlerType);
            HasRegistration = RegistrationType != null && RegistrationType != typeof(object);
            if (!HasRegistration) RegistrationType = null;
            CapabilityType = HandlerTypeDescriptorHelper.UnwrapGenericType(typeof(ICapability<>), handlerType);
            HasCapability = CapabilityType != null;
            if (!HasCapability) CapabilityType = null;
            if (HasCapability)
                IsDynamicCapability = typeof(IDynamicCapability).GetTypeInfo().IsAssignableFrom(CapabilityType);
            RequestProcessType = HandlerType
                .GetCustomAttributes(true)
                .Concat(HandlerType.GetCustomAttributes(true))
                .Concat(InterfaceType.GetInterfaces().SelectMany(x => x.GetCustomAttributes(true)))
                .Concat(HandlerType.GetInterfaces().SelectMany(x => x.GetCustomAttributes(true)))
                .OfType<ProcessAttribute>()
                .FirstOrDefault()?.Type;
        }

        public string Method { get; }
        public Direction Direction { get; }
        public RequestProcessType? RequestProcessType { get; }
        public bool IsRequest { get; }
        public Type HandlerType { get; }
        public Type InterfaceType { get; }
        public bool IsNotification { get; }
        public bool HasParamsType { get; }
        public Type ParamsType { get; }
        public bool HasResponseType { get; }
        public Type ResponseType { get; }
        public bool HasRegistration { get; }
        public Type RegistrationType { get; }
        public bool HasCapability { get; }
        public Type CapabilityType { get; }
        public bool IsDynamicCapability { get; }
        public override string ToString() => $"{Method}";
    }
}
