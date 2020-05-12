using System;
using System.Linq;
using System.Reflection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageProtocolShared
{
    public static class HandlerTypeHelpers
    {
        private static readonly Type[] HandlerTypes = { typeof(IJsonRpcNotificationHandler), typeof(IJsonRpcNotificationHandler<>), typeof(IJsonRpcRequestHandler<>), typeof(IJsonRpcRequestHandler<,>), };

        private static bool IsValidInterface(Type type)
        {
            if (type.GetTypeInfo().IsGenericType)
            {
                return HandlerTypes.Contains(type.GetGenericTypeDefinition());
            }
            return HandlerTypes.Contains(type);
        }

        public static Type GetHandlerInterface(Type type)
        {
            return type?.GetTypeInfo()
                .ImplementedInterfaces
                .First(IsValidInterface);
        }

        public static Type UnwrapGenericType(Type genericType, Type type)
        {
            return type?.GetTypeInfo()
                .ImplementedInterfaces
                .FirstOrDefault(x => x.GetTypeInfo().IsGenericType && x.GetTypeInfo().GetGenericTypeDefinition() == genericType)
                ?.GetTypeInfo()
                ?.GetGenericArguments()[0];
        }
    }
}
