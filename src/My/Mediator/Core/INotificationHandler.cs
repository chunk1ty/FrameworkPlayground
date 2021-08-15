using System.Threading.Tasks;

namespace Mediator.Core
{
    public interface INotificationHandler<TNotification>
        where TNotification : INotification
    {
        Task Handle(TNotification notification);
    }
}
