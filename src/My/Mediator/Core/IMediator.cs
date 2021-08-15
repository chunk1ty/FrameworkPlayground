using System.Threading.Tasks;

namespace Mediator.Core
{
    public interface IMediator
    {
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request);

        Task Publish<TNotification>(TNotification notification)
            where TNotification : INotification;
    }
}
