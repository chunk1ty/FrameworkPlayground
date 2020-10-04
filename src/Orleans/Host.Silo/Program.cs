using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using Orleans.Hosting;

namespace Host.Silo
{
    class Program
    {
        public static Task Main(string[] args)
        {
            return new HostBuilder().UseOrleans(siloBuilder =>
                                    {
                                        siloBuilder.UseLocalhostClustering()
                                                   .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback);

                                    })
                                    .ConfigureServices(services =>
                                    {
                                        services.Configure<ConsoleLifetimeOptions>(options =>
                                        {
                                            options.SuppressStatusMessages = true;
                                        });
                                    })
                                    .ConfigureLogging(builder => builder.AddConsole())
                                    .RunConsoleAsync();
        }
    }
}
