using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server;
using OmniSharp.Extensions.LanguageServer.Shared;
using Xunit;

namespace Lsp.Tests
{
    public class FoundationTests
    {
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

        [Theory(DisplayName = "Handler interfaces should have a abstract class")]
        [ClassData(typeof(HandlersShouldAbstractClassData))]
        public void HandlersShouldAbstractClass(IHandlerTypeDescriptor descriptor)
        {
            // This test requires a refactor, the delegating handlers have been removed and replaced by shared implementations
            // TODO:
            // * Check for extension methods
            // * Check for IPartialItem(s)<> extension methods
            // * Check that the extension method calls `AddHandler` using the correct eventname
            // * check extension method name
            // * Also update events to have a nicer fire and forget abstract class
            // * Ensure all notifications have an action and task returning function
            var abstractHandler = descriptor.HandlerType.Assembly.ExportedTypes
                .FirstOrDefault(z => z.IsAbstract && z.IsClass && descriptor.HandlerType.IsAssignableFrom(z));
            abstractHandler.Should().NotBeNull($"{descriptor.HandlerType.Name} is missing abstract base class");

            var extensionClassName = new Regex(@"(\w+)$").Replace(descriptor.HandlerType.FullName,
                descriptor.HandlerType.Name.TrimStart('I').Replace("Resolve", "") + "Extensions");
            var methodName =
                "On" + descriptor.HandlerType.Name.Substring(1, descriptor.HandlerType.Name.IndexOf("Handler") - 1).Replace("Resolve", "");

            var extensionClass = descriptor.HandlerType.Assembly.GetExportedTypes()
                .FirstOrDefault(z => z.IsClass && z.FullName == extensionClassName);
            extensionClass.Should().NotBeNull($"{descriptor.HandlerType.Name} is missing extension method class {extensionClassName}");
            extensionClass.GetMethods().Any(z => z.Name == methodName).Should()
                .BeTrue($"{descriptor.HandlerType.Name} is missing extension methods named {methodName}");

            var registries = extensionClass.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(z => z.Name == methodName)
                .Select(z => z.GetParameters()[0].ParameterType)
                .Distinct()
                .ToHashSet();

            void MatchMethod(Type parameterType)
            {
                var missingRegistry = new List<string>();
                foreach (var registry in registries)
                {
                    var method = extensionClass.GetMethods(BindingFlags.Static | BindingFlags.Public)
                        .FirstOrDefault(z => z.GetParameters()[1].ParameterType == parameterType);
                    if (method == null)
                    {
                        missingRegistry.Add(registry.Name);
                    }

                    if (descriptor.HasRegistration)
                    {
                        method.GetParameters().Length.Should().Be(3,
                            $"{descriptor.HandlerType.Name} {parameterType.FullName} is missing registration type");
                        method.GetParameters()[2].ParameterType.Should().Be(descriptor.RegistrationType,
                            $"{descriptor.HandlerType.Name} {parameterType.FullName} is has incorrect registration type {method.GetParameters()[2].ParameterType.Name}");
                        method.GetParameters()[2].IsOptional.Should()
                            .BeFalse($"{descriptor.HandlerType.Name} {parameterType.FullName} Registration types should not be optional");
                    }
                }

                missingRegistry.Should()
                    .BeEmpty(
                        $"{descriptor.HandlerType.Name} missing extension with parameter type {parameterType.FullName} method for {string.Join(", ", missingRegistry)}");
            }

            var delegatingHandler =
                descriptor.HandlerType.Assembly.DefinedTypes.FirstOrDefault(z =>
                    abstractHandler.IsAssignableFrom(z) && abstractHandler != z);
            if (delegatingHandler != null)
            {
                delegatingHandler.DeclaringType.Should().NotBeNull();
                delegatingHandler.DeclaringType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Any(z => z.Name.StartsWith("On"))
                    .Should().BeTrue($"{descriptor.HandlerType.Name} is missing delegating extension method");
                return;
            }

            if (typeof(ICanBeResolved).IsAssignableFrom(descriptor.ParamsType)) return;

            if (descriptor.IsRequest)
            {
                var returnType = descriptor.HasResponseType
                    ? typeof(Task<>).MakeGenericType(descriptor.ResponseType)
                    : typeof(Task);
                if (descriptor.HasParamsType)
                {
                    MatchMethod(typeof(Func<,>).MakeGenericType(descriptor.ParamsType, returnType));
                    MatchMethod(typeof(Func<,,>).MakeGenericType(descriptor.ParamsType, typeof(CancellationToken),
                        returnType));
                    MatchMethod(typeof(Func<,,>).MakeGenericType(descriptor.ParamsType, returnType));
                    MatchMethod(typeof(Func<,,,>).MakeGenericType(descriptor.ParamsType, typeof(CancellationToken),
                        returnType));
                    if (descriptor.HasCapability)
                    {
                        MatchMethod(typeof(Func<,,,>).MakeGenericType(descriptor.ParamsType, descriptor.CapabilityType,
                            typeof(CancellationToken), returnType));
                    }
                }
                else
                {
                    MatchMethod(typeof(Func<>).MakeGenericType(returnType));
                    MatchMethod(typeof(Func<,,>).MakeGenericType(typeof(CancellationToken), returnType));
                    MatchMethod(typeof(Func<>).MakeGenericType(returnType));
                    MatchMethod(typeof(Func<,>).MakeGenericType(typeof(CancellationToken), returnType));
                    if (descriptor.HasCapability)
                    {
                        MatchMethod(typeof(Func<,,>).MakeGenericType(descriptor.CapabilityType,
                            typeof(CancellationToken), returnType));
                    }
                }
            }

            if (descriptor.IsNotification)
            {
                if (descriptor.HasParamsType)
                {
                    MatchMethod(typeof(Action<>).MakeGenericType(descriptor.ParamsType));
                    MatchMethod(typeof(Action<,>).MakeGenericType(descriptor.ParamsType, typeof(CancellationToken)));
                    MatchMethod(typeof(Func<,>).MakeGenericType(descriptor.ParamsType, typeof(Task)));
                    MatchMethod(typeof(Func<,,>).MakeGenericType(descriptor.ParamsType, typeof(CancellationToken),
                        typeof(Task)));
                    if (descriptor.HasCapability)
                    {
                        MatchMethod(typeof(Action<,,>).MakeGenericType(descriptor.ParamsType, descriptor.CapabilityType,
                            typeof(CancellationToken)));
                        MatchMethod(typeof(Func<,,,>).MakeGenericType(descriptor.ParamsType, descriptor.CapabilityType,
                            typeof(CancellationToken), typeof(Task)));
                    }
                }
                else
                {
                    MatchMethod(typeof(Action));
                    MatchMethod(typeof(Action<>).MakeGenericType(typeof(CancellationToken)));
                    MatchMethod(typeof(Func<>).MakeGenericType(typeof(Task)));
                    MatchMethod(typeof(Func<,>).MakeGenericType(typeof(CancellationToken), typeof(Task)));
                    if (descriptor.HasCapability)
                    {
                        MatchMethod(typeof(Action<,>).MakeGenericType(descriptor.CapabilityType,
                            typeof(CancellationToken)));
                        MatchMethod(typeof(Func<,,>).MakeGenericType(descriptor.CapabilityType,
                            typeof(CancellationToken), typeof(Task)));
                    }
                }
            }
        }

        public class ParamsShouldHaveMethodAttributeData : TheoryData<Type>
        {
            public ParamsShouldHaveMethodAttributeData()
            {
                foreach (var type in typeof(CompletionParams).Assembly.ExportedTypes
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
                foreach (var type in typeof(CompletionParams).Assembly.ExportedTypes
                    .Where(z => z.IsInterface && typeof(IJsonRpcHandler).IsAssignableFrom(z) && !z.IsGenericType)
                    .Except(new[] {typeof(ITextDocumentSyncHandler)}))
                {
                    Add(type);
                }
            }
        }

        public class HandlersShouldAbstractClassData : TheoryData<IHandlerTypeDescriptor>
        {
            public HandlersShouldAbstractClassData()
            {
                foreach (var type in typeof(CompletionParams).Assembly.ExportedTypes
                    .Where(z => z.IsInterface && typeof(IJsonRpcHandler).IsAssignableFrom(z) && !z.IsGenericType)
                    .Except(new[] {typeof(ITextDocumentSyncHandler)}))
                {
                    Add(HandlerTypeHelper.GetHandlerTypeDescriptor(type));
                }
            }
        }
    }
}
