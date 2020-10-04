﻿using Grains.Contracts;

 namespace Client.Console
{
    public class DeviceObserver : IDeviceObserver
    {
        public void HighTemperature(long grainId, double temperature)
        {
            System.Console.WriteLine($"[{grainId}] Average temperature - {temperature}.");
        }
    }
}
