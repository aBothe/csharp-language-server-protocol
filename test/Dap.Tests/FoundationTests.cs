using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;
using Xunit;
using Xunit.Abstractions;

namespace Dap.Tests
{
    public class FoundationTests
    {
        private readonly ILogger _logger;

        public FoundationTests(ITestOutputHelper outputHelper)
        {
            this._logger = new TestLoggerFactory(outputHelper).CreateLogger(typeof(FoundationTests));
        }

        [Theory(DisplayName = "Params types should have a method attribute")]
        [ClassData(typeof(ParamsShouldHaveMethodAttributeData))]
        public void ParamsShouldHaveMethodAttribute(Type type)
        {
            type.GetCustomAttributes<MethodAttribute>().Any().Should()
                .Be(true, $"{type.Name} is missing a method attribute");
        }

        [Theory(DisplayName = "Handler interfaces should have a method attribute")]
        [ClassData(typeof(HandlersShouldHaveMethodAttributeData))]
        public void HandlersShouldHaveMethodAttribute(Type type)
        {
            type.GetCustomAttributes<MethodAttribute>().Any().Should()
                .Be(true, $"{type.Name} is missing a method attribute");
        }

        [Theory(DisplayName = "Handler method should match params method")]
        [ClassData(typeof(HandlersShouldHaveMethodAttributeData))]
        public void HandlersShouldMatchParamsMethodAttribute(Type type)
        {
            var paramsType = GetHandlerInterface(type).GetGenericArguments()[0];
            type.GetCustomAttribute<MethodAttribute>(true).Method.Should().Be(
                paramsType.GetCustomAttribute<MethodAttribute>().Method,
                $"{type.FullName} method does not match {paramsType.FullName}");
        }

        [Theory(DisplayName = "Handler interfaces should have a abstract class")]
        [ClassData(typeof(HandlersShouldAbstractClassData))]
        public void HandlersShouldAbstractClass(Type type)
        {
            _logger.LogInformation("Handler: {Type}", type);
            // This test requires a refactor, the delegating handlers have been removed and replaced by shared implementations
            // TODO:
            // * Check for extension methods
            // * Check for IPartialItem(s)<> extension methods
            // * Check that the extension method calls `AddHandler` using the correct eventname
            // * check extension method name
            // * Also update events to have a nicer fire and forget abstract class
            // * Ensure all notifications have an action and task returning function
            var abstractHandler = type.Assembly.ExportedTypes
                .FirstOrDefault(z => z.IsAbstract && z.IsClass && type.IsAssignableFrom(z));
            abstractHandler.Should().NotBeNull($"{type.FullName} is missing abstract base class");

            _logger.LogInformation("Abstract Handler: {Type}", abstractHandler);

            var extensionClassName = new Regex(@"(\w+)$").Replace(type.FullName,
                type.Name.Substring(1, type.Name.IndexOf("Handler") - 1) + "Extensions");
            var methodName =
                "On" + type.Name.Substring(1, type.Name.IndexOf("Handler") - 1);
            var rpcMethod = type.GetCustomAttribute<MethodAttribute>(true).Method;

            var extensionClass = type.Assembly.GetExportedTypes()
                .FirstOrDefault(z => z.IsClass && z.FullName == extensionClassName);

            _logger.LogInformation("Extension Class Name: {Name}", extensionClassName);
            _logger.LogInformation("Extension Class: {Type}", extensionClass);

            extensionClass.Should()
                .NotBeNull($"{type.FullName} is missing extension method class {extensionClassName}");
            extensionClass.GetMethods().Any(z => z.Name == methodName).Should()
                .BeTrue($"{type.FullName} is missing extension methods named {methodName}");

            var registries = extensionClass.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(z => z.Name == methodName)
                .Select(z => z.GetParameters()[0].ParameterType)
                .Distinct()
                .ToHashSet();

            void MatchMethod(Type parameterType)
            {
                foreach (var registry in registries)
                {
                    var method = extensionClass.GetMethods(BindingFlags.Static | BindingFlags.Public)
                        .FirstOrDefault(z => z.GetParameters()[1].ParameterType == parameterType);
                    method.Should()
                        .NotBeNull(
                            $"{type.FullName} missing extension with parameter type {parameterType.FullName} method for {registry.FullName}");

                    var registrySub = Substitute.For(new Type[] {method.GetParameters()[0].ParameterType},
                        Array.Empty<object>());
                    method.Invoke(
                        null,
                        new[] {
                            registrySub,
                            Substitute.For(new Type[] {method.GetParameters()[1].ParameterType},
                                Array.Empty<object>())
                        });

                    registrySub.Received().ReceivedCalls().Any(z =>
                            z.GetMethodInfo().Name == nameof(IJsonRpcHandlerRegistry.AddHandler) &&
                            z.GetArguments().Length == 2 && z.GetArguments()[0].Equals(rpcMethod))
                        .Should().BeTrue($"{type.Name} {parameterType.Name} should have the correct method.");
                }
            }

            var delegatingHandler =
                type.Assembly.DefinedTypes.FirstOrDefault(z =>
                    abstractHandler.IsAssignableFrom(z) && abstractHandler != z);
            if (delegatingHandler != null)
            {
                _logger.LogInformation("Delegating Handler: {Type}", delegatingHandler);
                delegatingHandler.DeclaringType.Should().NotBeNull();
                delegatingHandler.DeclaringType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Any(z => z.Name.StartsWith("On"))
                    .Should().BeTrue($"{type.FullName} is missing delegating extension method");
                return;
            }

            var isNotification = typeof(IJsonRpcNotificationHandler).IsAssignableFrom(type) || type
                .GetInterfaces().Any(z =>
                    z.IsGenericType && typeof(IJsonRpcNotificationHandler<>).IsAssignableFrom(z));
            var isRequest = !isNotification;
            var interfaceType = GetHandlerInterface(type);
            var paramsType = interfaceType.IsGenericType
                ? interfaceType.GetGenericArguments()[0]
                : typeof(EmptyRequest);
            var responseType = paramsType?
                .GetInterfaces()
                .FirstOrDefault(x =>
                    x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>))
                ?.GetGenericArguments()[0] ?? typeof(Unit);

            if (isRequest)
            {
                var returnType = responseType != typeof(Unit)
                    ? typeof(Task<>).MakeGenericType(responseType)
                    : typeof(Task);
                if (paramsType != typeof(EmptyRequest))
                {
                    MatchMethod(typeof(Func<,>).MakeGenericType(paramsType, returnType));
                    MatchMethod(typeof(Func<,,>).MakeGenericType(paramsType, typeof(CancellationToken),
                        returnType));
                }
                else
                {
                    MatchMethod(typeof(Func<>).MakeGenericType(returnType));
                    MatchMethod(typeof(Func<,>).MakeGenericType(typeof(CancellationToken), returnType));
                }
            }

            if (isNotification)
            {
                if (paramsType != typeof(EmptyRequest))
                {
                    MatchMethod(typeof(Action<>).MakeGenericType(paramsType));
                    MatchMethod(typeof(Action<,>).MakeGenericType(paramsType, typeof(CancellationToken)));
                    MatchMethod(typeof(Func<,>).MakeGenericType(paramsType, typeof(Task)));
                    MatchMethod(typeof(Func<,>).MakeGenericType(paramsType, typeof(CancellationToken),
                        typeof(Task)));
                }
                else
                {
                    MatchMethod(typeof(Action));
                    MatchMethod(typeof(Action<>).MakeGenericType(typeof(CancellationToken)));
                    MatchMethod(typeof(Func<>).MakeGenericType(typeof(Task)));
                    MatchMethod(typeof(Func<,>).MakeGenericType(typeof(CancellationToken), typeof(Task)));
                }
            }
        }

        public class ParamsShouldHaveMethodAttributeData : TheoryData<Type>
        {
            public ParamsShouldHaveMethodAttributeData()
            {
                foreach (var type in typeof(IDataBreakpointInfoHandler).Assembly.ExportedTypes
                    .Where(z => z.IsClass && !z.IsAbstract && z.GetInterfaces().Any(z =>
                        z.IsGenericType &&
                        typeof(IRequest<>).IsAssignableFrom(z.GetGenericTypeDefinition()))))
                {
                    Add(type);
                }
            }
        }

        public class HandlersShouldHaveMethodAttributeData : TheoryData<Type>
        {
            public HandlersShouldHaveMethodAttributeData()
            {
                foreach (var type in typeof(IDataBreakpointInfoHandler).Assembly.ExportedTypes
                    .Where(z => z.IsInterface && typeof(IJsonRpcHandler).IsAssignableFrom(z) && !z.IsGenericType))
                {
                    Add(type);
                }
            }
        }

        public class HandlersShouldAbstractClassData : TheoryData<Type>
        {
            public HandlersShouldAbstractClassData()
            {
                foreach (var type in typeof(IDataBreakpointInfoHandler).Assembly.ExportedTypes
                    .Where(z => z.IsInterface && typeof(IJsonRpcHandler).IsAssignableFrom(z) && !z.IsGenericType))
                {
                    Add(type);
                }
            }
        }


        private static readonly Type[] HandlerTypes = {
            typeof(IJsonRpcNotificationHandler), typeof(IJsonRpcNotificationHandler<>),
            typeof(IJsonRpcRequestHandler<>), typeof(IJsonRpcRequestHandler<,>),
        };

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
            if (IsValidInterface(type)) return type;
            return type?.GetTypeInfo()
                .ImplementedInterfaces
                .First(IsValidInterface);
        }
    }
}
