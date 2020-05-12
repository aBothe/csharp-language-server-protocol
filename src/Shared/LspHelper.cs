using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace OmniSharp.Extensions.LanguageProtocolShared
{
    public interface IHandlerTypeDescriptor
    {
        string Method { get; }
        Type InterfaceType { get; }
        Type ParamsType { get; }
        bool HasResponseType { get; }
        Type ResponseType { get; }
        bool HasRegistration { get; }
        Type RegistrationType { get; }
        bool HasCapability { get; }
        Type CapabilityType { get; }
        bool IsDynamicCapability { get; }
    }

    class HandlerTypeDescriptor : IHandlerTypeDescriptor
    {
        public HandlerTypeDescriptor(Type handlerType)
        {
            Method = HandlerTypeHelper.GetMethodName(handlerType);
            InterfaceType = handlerType;
            ParamsType = handlerType.GetTypeInfo().GetGenericArguments()[0];
            var requestInterface = ParamsType?.GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>));
            if (requestInterface != null)
                ResponseType = requestInterface.GetGenericArguments()[0];
            HasResponseType = ResponseType != null;
            RegistrationType = HandlerTypeHelpers.UnwrapGenericType(typeof(IRegistration<>), handlerType);
            HasRegistration = RegistrationType != null;
            CapabilityType = HandlerTypeHelpers.UnwrapGenericType(typeof(ICapability<>), handlerType);
            HasCapability = CapabilityType != null;
            IsDynamicCapability = typeof(IDynamicCapability).GetTypeInfo().IsAssignableFrom(CapabilityType);
        }

        public string Method { get; }
        public Type InterfaceType { get; }
        public Type ParamsType { get; }
        public bool HasResponseType { get; }
        public Type ResponseType { get; }
        public bool HasRegistration { get; }
        public Type RegistrationType { get; }
        public bool HasCapability { get; }
        public Type CapabilityType { get; }
        public bool IsDynamicCapability { get; }
    }

    public static class HandlerTypeHelper
    {
        private static readonly ConcurrentDictionary<Type, string> MethodNames =
            new ConcurrentDictionary<Type, string>();

        private static readonly ImmutableSortedDictionary<string, IHandlerTypeDescriptor> KnownHandlers;

        static HandlerTypeHelper()
        {
            KnownHandlers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => {
                    try
                    {
                        return x.GetTypes();
                    }
                    catch
                    {
                        return Enumerable.Empty<Type>();
                    }
                })
                .Where(z => z.IsInterface && typeof(IJsonRpcHandler).IsAssignableFrom(z))
                .Where(z => z.GetCustomAttributes<MethodAttribute>().Any())
                .ToLookup(x => x.GetCustomAttribute<MethodAttribute>().Method)
                .Select(x => new HandlerTypeDescriptor(x.First()) as IHandlerTypeDescriptor)
                .ToImmutableSortedDictionary(x => x.Method, x => x, StringComparer.Ordinal);
        }

        public static IHandlerTypeDescriptor GetHandlerTypeDescriptor(string method)
        {
            return KnownHandlers.TryGetValue(method, out var descriptor) ? descriptor : null;
        }

        public static IHandlerTypeDescriptor GetHandlerTypeDescriptor<T>()
        {
            return KnownHandlers.Values.FirstOrDefault(x => x.InterfaceType == typeof(T)) ??
                   GetHandlerTypeDescriptor(GetMethodName(typeof(T)));
        }

        public static IHandlerTypeDescriptor GetHandlerTypeDescriptor(Type type)
        {
            return KnownHandlers.Values.FirstOrDefault(x => x.InterfaceType == type) ??
                   GetHandlerTypeDescriptor(GetMethodName(type));
        }

        public static string GetMethodName<T>()
            where T : IJsonRpcHandler
        {
            return GetMethodName(typeof(T));
        }

        public static bool IsMethodName(string name, params Type[] types)
        {
            return types.Any(z => GetMethodName(z).Equals(name));
        }

        public static string GetMethodName(Type type)
        {
            if (MethodNames.TryGetValue(type, out var method)) return method;

            // Custom method
            var attribute = type.GetTypeInfo().GetCustomAttribute<MethodAttribute>();
            if (attribute is null)
            {
                attribute = type.GetTypeInfo()
                    .ImplementedInterfaces
                    .Select(t => t.GetCustomAttribute<MethodAttribute>())
                    .FirstOrDefault(x => x != null);
            }

            // TODO: Log unknown method name
            if (attribute is null)
            {
                return null;
            }

            MethodNames.TryAdd(type, attribute.Method);
            return attribute.Method;
        }
    }
}
