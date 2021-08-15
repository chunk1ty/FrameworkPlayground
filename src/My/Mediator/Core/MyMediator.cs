using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mediator.Core
{
    public class MyMediator : IMediator
    {
        // keeps mapping between request and related handler
        // key => request
        // value => request handler
        private readonly Dictionary<Type, Type> _requestHandlersMappings;

        // keeps mapping between notification and related handlers
        // key => request
        // value => request handler
        private readonly Dictionary<Type, List<Type>> _notificationHandlersMappings;

        // DI handlers registrations
        private readonly Dictionary<Type, Func<object>> _registrations;

        public MyMediator(Dictionary<Type, Type> requestHandlersMappings,
                          Dictionary<Type, List<Type>> notificationHandlersMappings,
                          Dictionary<Type, Func<object>> registrations)
        {
            _requestHandlersMappings = requestHandlersMappings;
            _registrations = registrations;
            _notificationHandlersMappings = notificationHandlersMappings;
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            Type requestType = request.GetType();
            if (!_requestHandlersMappings.ContainsKey(requestType))
            {
                throw new ArgumentException($"Cannot find handler for request of type: [{requestType}]");
            }

            Type requestHandlerType = _requestHandlersMappings[requestType];
            object handler = _registrations[requestHandlerType]();

            return await (Task<TResponse>)handler.GetType().GetMethod("Handle").Invoke(handler, new[] { request });
        }

        public async Task Publish<TNotification>(TNotification notification)
            where TNotification : INotification
        {
            if (notification is null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            Type notificationType = notification.GetType();
            if (!_notificationHandlersMappings.ContainsKey(notificationType))
            {
                throw new ArgumentException($"Cannot find handler for notification of type: [{notificationType}]");
            }

            List<Type> notificationHandlerTypes = _notificationHandlersMappings[notificationType];

            foreach (Type notificationHandlerType in notificationHandlerTypes)
            {
                object handler = _registrations[notificationHandlerType]();

                await (Task)handler.GetType().GetMethod("Handle").Invoke(handler, new[] { (object)notification });
            }
        }
    }
}
