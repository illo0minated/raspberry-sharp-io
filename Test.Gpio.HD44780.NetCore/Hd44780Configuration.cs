using System;
using Raspberry.IO.Components.NetStandard.Displays.Hd44780;

namespace Test.Gpio.HD44780.NetCore
{
    internal class Hd44780Configuration : IDisposable
    {
        private readonly IDisposable driver;

        public Hd44780Configuration(IDisposable driver = null)
        {
            this.driver = driver;
        }

        public void Dispose()
        {
            if (driver != null)
                driver.Dispose();
        }

        public Hd44780Pins Pins;
    }
}