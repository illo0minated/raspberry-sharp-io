using System;
using System.Threading;
using Raspberry.IO.Components.NetStandard.Sensors.Humidity.Htu21df;
using Raspberry.IO.GeneralPurpose.NetStandard;
using Raspberry.IO.InterIntegratedCircuit.NetStandard;

namespace Test.Gpio.Htu21df.NetCore
{
    class Program
    {
        static void Main()
        {
            const ConnectorPin sdaPin = ConnectorPin.P1Pin03;
            const ConnectorPin sclPin = ConnectorPin.P1Pin05;

            Console.WriteLine( "HTU21DF Sample: Read humidity and temperature" );
            Console.WriteLine();
            Console.WriteLine( "\tSDA: {0}", sdaPin );
            Console.WriteLine( "\tSCL: {0}", sclPin );
            Console.WriteLine();

            using ( var driver = new I2cDriver( sdaPin.ToProcessor(), sclPin.ToProcessor() ) )
            {
                var deviceConnection = new Htu21dfConnection( driver.Connect( 0x40 ) );
                deviceConnection.Begin();

                while ( !Console.KeyAvailable )
                {
                    var temp = deviceConnection.ReadTemperature();
                    var humidity = deviceConnection.ReadHumidity();
                    Console.WriteLine( $"Temp is: {temp:F1}C. RH is {humidity:F1}%" );
                    Thread.Sleep( 2000 );
                }
            }
        }
    }
}
