using System;

namespace Raspberry.IO.Components.NetStandard.Devices.PiFaceDigital
{
    public class InputPinChangedArgs : EventArgs
    {
        public PiFaceInputPin pin;
    }
}
