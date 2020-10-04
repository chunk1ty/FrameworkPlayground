﻿using Orleans;

 namespace Grains.Contracts
{
    // Pub/Sub semantic
    public interface IDeviceObserver : IGrainObserver
    {
        void HighTemperature(long grainId, double temperature);
    }
}
