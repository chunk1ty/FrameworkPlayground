using System.Threading.Tasks;
using Grains.Contracts;
using Orleans;

namespace Client.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using (var client = ClientConfigurations())
            {
                await client.Connect();

                var deviceObserver = new DeviceObserver();
                var deviceObserverReference = await client.CreateObjectReference<IDeviceObserver>(deviceObserver);

                var grain = client.GetGrain<IDecodeGrain>(0);

                while (true)
                {
                    // <grain identity>, <temperature value>
                    var temperatureInput = System.Console.ReadLine();
                    await grain.Decode(temperatureInput);

                    // subscribe 
                    var deviceGrain = client.GetGrain<IDeviceGrain>(int.Parse(temperatureInput.Split(',')[0]));
                    await deviceGrain.Observe(deviceObserverReference);
                }
            }
        }

        private static IClusterClient ClientConfigurations()
        {
            return new ClientBuilder().UseLocalhostClustering()
                                      .Build();
        }
    }
}
