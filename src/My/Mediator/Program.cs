using System;
using System.Threading.Tasks;
using Mediator.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator
{
    class Program
    {
        static async Task Main()
        {
            // 1. Manual registration

            //var requestHandlersMappings = new Dictionary<Type, Type>
            //{
            //    {typeof(MyRequest), typeof(MyRequestRequestHandler)},
            //    {typeof(MyAgeRequest), typeof(MyAgeRequestRequestHandler)}
            //};

            //var notificationHandlersMappings = new Dictionary<Type, List<Type>>
            //{
            //    {typeof(MyNotification), new List<Type>(2) {typeof(MyNotificationHandler1), typeof(MyNotificationHandler2) }},
            //};

            //var registrations = new Dictionary<Type, Func<object>>
            //{
            //    {typeof(MyRequestRequestHandler), () => new MyRequestRequestHandler()},
            //    {typeof(MyAgeRequestRequestHandler), () => new MyAgeRequestRequestHandler()},
            //    {typeof(MyNotificationHandler1), () => new MyNotificationHandler1()},
            //    {typeof(MyNotificationHandler2), () => new MyNotificationHandler2()},
            //};

            //IMediator mediator = new MyMediator(requestHandlersMappings, notificationHandlersMappings, registrations);

            //await mediator.Send(new MyRequest("Hello World!"));
            //await mediator.Send(new MyAgeRequest(28));

            //await mediator.Publish(new MyNotification("notification"));

            // 2. Dynamic registration
            var serviceProvider = new ServiceCollection().AddMediator(ServiceLifetime.Scoped, typeof(Program))
                                                         .BuildServiceProvider();

            var mediator = serviceProvider.GetRequiredService<IMediator>();

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

    public class MyNotification : INotification
    {
        public MyNotification(string text)
        {
            Text = text;
        }

        public string Text { get; }
    }

    public class MyNotificationHandler1 : INotificationHandler<MyNotification>
    {
        public Task Handle(MyNotification notification)
        {
            Console.WriteLine($"[{notification.Text}] Handler type: [{this.GetType()}]");

            return Task.FromResult(true);
        }
    }

    public class MyNotificationHandler2 : INotificationHandler<MyNotification>
    {
        public Task Handle(MyNotification notification)
        {
            Console.WriteLine($"[{notification.Text}] Handler type: [{this.GetType()}]");

            return Task.FromResult(true);
        }
    }
}
