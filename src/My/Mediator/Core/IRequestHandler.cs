using System.Threading.Tasks;

namespace Mediator.Core
{
    public interface IRequestHandler<TRequest, TResponse> 
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(TRequest request);
    }
}
