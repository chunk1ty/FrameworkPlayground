using System.Threading.Tasks;
using Orleans;

namespace Grains.Contracts
{
    // Stateless grain
    public interface IDecodeGrain : IGrainWithIntegerKey
    {
        Task Decode(string message);
    }
}
