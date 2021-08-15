using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mediator.Core;

namespace Mediator
{
    class Program
    {
        static async Task Main()
        {
            var mappings = new Dictionary<Type, Type>
            {
                {typeof(MyRequest), typeof(MyRequestRequestHandler)},
                {typeof(MyAgeRequest), typeof(MyAgeRequestRequestHandler)}
            };

            var registrations = new Dictionary<Type, Func<object>>
            {
                {typeof(MyRequestRequestHandler), () => new MyRequestRequestHandler()},
                {typeof(MyAgeRequestRequestHandler), () => new MyAgeRequestRequestHandler()},
            };

            IMediator mediator = new MyMediator(mappings, registrations);
            
            await mediator.Send(new MyRequest("Hello World!"));

            await mediator.Send(new MyAgeRequest(28));
        }

        private static Func<Type, object> DiRegistrations()
        {
            return (T) => new MyRequestRequestHandler();
        }
    }

    public class MyRequest : IRequest<bool>
    {
        public MyRequest(string text)
        {
            Text = text;
        }

        public string Text { get; }
    }

    public class MyRequestRequestHandler : IRequestHandler<MyRequest, bool>
    {
        public Task<bool> Handle(MyRequest request)
        {
            Console.WriteLine($"[{request.Text}] Handler type: [{this.GetType()}]");

            return Task.FromResult(true);
        }
    }

    public class MyAgeRequest : IRequest<int>
    {
        public MyAgeRequest(int age)
        {
            Age = age;
        }

        public int Age { get; }
    }

    public class MyAgeRequestRequestHandler : IRequestHandler<MyAgeRequest, int>
    {
        public Task<int> Handle(MyAgeRequest request)
        {
            Console.WriteLine($"My age is: [{request.Age}] Handler type: [{this.GetType()}]");

            return Task.FromResult(42);
        }
    }
}
