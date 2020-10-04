using System.Threading.Tasks;
using Orleans;

namespace Grains.Contracts
{
    public interface IDeviceGrain : IGrainWithIntegerKey
    {
        Task SetTemperature(double value);

        Task<double> GetAverageTemperature();

        Task Observe(IDeviceObserver observer);

        Task UnObserve(IDeviceObserver observer);
    }
}
