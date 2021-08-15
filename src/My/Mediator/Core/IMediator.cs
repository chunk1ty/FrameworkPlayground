using System.Threading.Tasks;

namespace Mediator.Core
{
    public interface IMediator
    {
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request);
    }
}
