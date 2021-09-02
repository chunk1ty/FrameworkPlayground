using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mediator.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mediator
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMyMediator(this IServiceCollection services, ServiceLifetime lifetime, params Type[] markers)
        {
            var handlerInfo = new Dictionary<Type, Type>();

            foreach (Type marker in markers)
            {
                var assembly = marker.Assembly;
                var requests = GetClassesImplementingInterface(assembly, typeof(IRequest<>));
                var handlers = GetClassesImplementingInterface(assembly, typeof(IRequestHandler<,>));

                requests.ForEach(x =>
                    handlerInfo[x] = handlers.SingleOrDefault(xx => x == xx.GetInterface("IRequestHandler`2")!.GetGenericArguments()[0])
                );

                var serviceDescriptor = handlers.Select(x => new ServiceDescriptor(x, x, lifetime));
                services.TryAdd(serviceDescriptor);
            }

            services.AddSingleton<IMediator>(x => new MyMediator(x.GetRequiredService, handlerInfo));

            return services;
        }

        private static List<Type> GetClassesImplementingInterface(Assembly assembly, Type typeToMatch)
        {
            return assembly.ExportedTypes
                .Where(type =>
                {
                    var genericInterfaceTypes = type.GetInterfaces().Where(x => x.IsGenericType).ToList();
                    var implementRequestType =
                        genericInterfaceTypes.Any(x => x.GetGenericTypeDefinition() == typeToMatch);

                    return !type.IsInterface && !type.IsAbstract && implementRequestType;

                }).ToList();
        }
    }
}
