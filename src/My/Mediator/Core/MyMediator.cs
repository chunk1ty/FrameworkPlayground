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
        private readonly Dictionary<Type, Type> _handlerMappings;

        // DI handlers registrations
        private readonly Dictionary<Type, Func<object>> _registrations;

        public MyMediator(Dictionary<Type, Type> handlerMappings, Dictionary<Type, Func<object>> registrations)
        {
            _handlerMappings = handlerMappings;
            _registrations = registrations;
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            Type requestType = request.GetType();
            if (!_handlerMappings.ContainsKey(requestType))
            {
                throw new ArgumentException($"Cannot find handler for request of type: [{requestType}]");
            }

            Type requestHandlerType = _handlerMappings[requestType];
            object handler = _registrations[requestHandlerType]();

            return await (Task<TResponse>)handler.GetType().GetMethod("Handle").Invoke(handler, new[] {request});
        }
    }
}
