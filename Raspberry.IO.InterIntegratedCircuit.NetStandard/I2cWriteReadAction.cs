namespace Raspberry.IO.InterIntegratedCircuit.NetStandard
{
    /// <summary>
    /// Defines an I2C write and read  action, to be used when repeated start is needed.
    /// </summary>
    public class I2cWriteReadAction : I2cAction
    {
        public byte[] WriteBuffer { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="I2cWriteReadAction"/> class.
        /// </summary>
        /// <param name="buffer">The buffer which should be used to store the received data.</param>
        public I2cWriteReadAction(params byte[] buffer)
            : base(buffer)
        {

        }
    }
}
