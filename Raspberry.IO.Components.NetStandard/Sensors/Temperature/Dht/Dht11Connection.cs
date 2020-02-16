
#region References

using System;
using Raspberry.IO.NetStandard;
using UnitsNet;

#endregion

namespace Raspberry.IO.Components.NetStandard.Sensors.Temperature.Dht
{
    /// <summary>
    /// Represents a connection to a DHT11 sensor.
    /// </summary>
    public class Dht11Connection : DhtConnection
    {
        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="DhtConnection" /> class.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="autoStart">if set to <c>true</c>, DHT is automatically started. Default value is <c>true</c>.</param>
        public Dht11Connection(IInputOutputBinaryPin pin, bool autoStart = true) : base(pin, autoStart) { }

        #endregion

        #region Protected Methods

        protected override TimeSpan DefaultSamplingInterval
        {
            get { return TimeSpan.FromSeconds(1); }
        }

        protected override TimeSpan WakeupInterval
        {
            get { return TimeSpan.FromMilliseconds(18); }
        }
        
        protected override TimeSpan HostReleaseBusInterval
        {
            get { throw new ArgumentException("DHT11 sensor does not have the host release time"); }
        }

        protected override bool HaveHostReleaseBus => false;

        protected override DhtData GetDhtData(int temperatureValue, int humidityValue)
        {
            return new DhtData
            {
                RelativeHumidity = Ratio.FromPercent(humidityValue/256d),
                Temperature = UnitsNet.Temperature.FromDegreesCelsius(temperatureValue/256d)
            };
        }

        #endregion
    }
}
