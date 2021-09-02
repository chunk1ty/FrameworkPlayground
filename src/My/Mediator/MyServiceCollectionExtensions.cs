using DiContainer.Core.MyAdvancedDiContainer;
using Mediator.Core;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;

namespace Mediator
{
    public static class MyServiceCollectionExtensions
    {
        public static MyServiceCollection AddMyMediator(this MyServiceCollection services, MyServiceLifetime lifetime, params Type[] markers)
        {
            var handlersInfo = new Dictionary<Type, Type>();

            foreach (Type marker in markers)
            {
                var assembly = marker.Assembly;
                var requests = GetClassesImplementingInterface(assembly, typeof(IRequest<>));
                var handlers = GetClassesImplementingInterface(assembly, typeof(IRequestHandler<,>));

                foreach (Type request in requests)
                {
                    Type requestHandler = handlers.SingleOrDefault(h => request == h.GetInterfaces().First().GetGenericArguments().First());
                    handlersInfo.Add(request, requestHandler);
                }

                var serviceDescriptor = handlers.Select(x => new MyServiceDescriptor(x, x, lifetime));


                services.AddServiceDescriptor(serviceDescriptor);
            }

            services.RegisterSingleton<IMediator>(x => new MyMediator(x.GetService, handlersInfo));

            return services;
        }

        private static List<Type> GetClassesImplementingInterface(Assembly assembly, Type typeToMatch)
        {
            return assembly.ExportedTypes
                .Where(type =>
                {
                    return type.GetInterfaces().Any(t => t.IsGenericType &&
                                                       t.GetGenericTypeDefinition() == typeToMatch);
                }).ToList();
        }
    }
}
