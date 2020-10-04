using System.Threading.Tasks;
using Grains.Contracts;
using Orleans;
using Orleans.Concurrency;

namespace Grains
{
    [StatelessWorker]
    public class DecodeGrain : Grain, IDecodeGrain
    {
        public Task Decode(string message)
        {
            var parts = message.Split(',');
            
            var deviceGrain = GrainFactory.GetGrain<IDeviceGrain>(int.Parse(parts[0]));
            
            return deviceGrain.SetTemperature(double.Parse(parts[1]));
        }
    }
}
