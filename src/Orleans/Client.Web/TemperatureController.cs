using System.Threading.Tasks;
using Grains.Contracts;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace Client.Web
{
    [ApiController]
    public class TemperatureController : ControllerBase
    {
        private readonly IClusterClient _client;

        public TemperatureController(IClusterClient client)
        {
            _client = client;
        }

        [HttpGet("averagetemperature/{key}")]
        public async Task<double> GetAverageTemperature(long key)
        {
            var deviceGrain = _client.GetGrain<IDeviceGrain>(key);

            return await deviceGrain.GetAverageTemperature();
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody]string temperature)
        {
            var decodeGrain = _client.GetGrain<IDecodeGrain>(0);
            await decodeGrain.Decode(temperature);

            return Ok("Ok");
        }
    }
}
