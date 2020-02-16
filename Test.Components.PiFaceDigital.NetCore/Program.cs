using System;
using System.Threading;
using System.Threading.Tasks;
using Raspberry.IO.Components.NetStandard.Devices.PiFaceDigital;

namespace Test.Components.PiFaceDigital.NetCore
{
    class Program
    {
        private static PiFaceDigitalDevice _piFace;
        private static bool _polling = true;
        private static Task _task;

        /// <summary>
        /// Demo App
        /// 
        /// Loops through each output pin 10 times pulsing them on for 1/2 second
        /// Inputs are polled and any change reported on the console
        /// </summary>
        static void Main()
        {

            _piFace = new PiFaceDigitalDevice();


            // setup events
            foreach (var ip in _piFace.InputPins)
            {
                ip.OnStateChanged += ip_OnStateChanged;
            }

            _task = Task.Factory.StartNew(PollInputs);

            for (int i = 0; i < 10; i++)
            {
                for (int pin = 0; pin < 8; pin++)
                {
                    _piFace.OutputPins[pin].State = true;
                    _piFace.UpdatePiFaceOutputPins();
                    Thread.Sleep(500);
                    _piFace.OutputPins[pin].State = false;
                    _piFace.UpdatePiFaceOutputPins();
                    Thread.Sleep(500);
                }
            }

            //stop polling loop
            _polling = false;
            _task.Wait();
        }




        /// <summary>
        /// Loop polling the inputs at 200ms intervals
        /// </summary>
        private static void PollInputs()
        {
            while (_polling)
            {
                _piFace.PollInputPins();
                Thread.Sleep(200);
            }
        }

        /// <summary>
        /// Log any input pin changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ip_OnStateChanged(object sender, InputPinChangedArgs e)
        {
            Console.WriteLine("Pin {0} became {1}", e.pin.Id, e.pin.State);
        }
    }
}
