using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grains.Contracts;
using Microsoft.Extensions.Logging;
using Orleans;

namespace Grains
{
    public class DeviceGrain : Grain, IDeviceGrain
    {
        private double _lastValue;
        private readonly List<double> _temperatures = new List<double>();
        private readonly HashSet<IDeviceObserver> _observers = new HashSet<IDeviceObserver>();

        private readonly ILogger _logger;

        public DeviceGrain(ILogger<DeviceGrain> logger)
        {
            _logger = logger;
        }

        public override Task OnActivateAsync()
        {
            RegisterTimer(Callback, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10));

            return base.OnActivateAsync();
        }

        private Task Callback(object param)
        {
            if (!_temperatures.Any())
            {
                return Task.CompletedTask;
            }

            double averageTemperature = _temperatures.Average();
            if (averageTemperature > 100)
            {
                _logger.LogInformation($"[{this.GetGrainIdentity().PrimaryKeyLong}] Average temperature is {averageTemperature}");

                // Notify observers for average temperature.
                foreach (IDeviceObserver observer in _observers)
                {
                    observer.HighTemperature(this.GetGrainIdentity().PrimaryKeyLong, averageTemperature);
                }
            }

            return Task.CompletedTask;
        }

        public Task SetTemperature(double value)
        {
            _logger.LogInformation($"[{this.GetGrainIdentity().PrimaryKeyLong}] DeviceGrain with value '{value}' {Thread.CurrentThread.ManagedThreadId}");

            if (_lastValue < 100 && value >= 100)
            {
                _logger.LogInformation($"[{this.GetGrainIdentity().PrimaryKeyLong}] High temperature! '{value}'");
            }

            _lastValue = value;

            _temperatures.Add(value);

            return Task.CompletedTask;
        }

        public Task<double> GetAverageTemperature()
        {
            return Task.FromResult(_temperatures.Average());
        }

        public Task Observe(IDeviceObserver observer)
        {
            _observers.Add(observer);

            return Task.CompletedTask;
        }

        public Task UnObserve(IDeviceObserver observer)
        {
            _observers.Remove(observer);

            return Task.CompletedTask;
        }
    }
}
