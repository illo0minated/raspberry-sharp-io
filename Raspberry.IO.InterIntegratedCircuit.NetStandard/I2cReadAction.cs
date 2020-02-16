namespace Raspberry.IO.InterIntegratedCircuit.NetStandard
{
    /// <summary>
    /// Defines an I2C read action.
    /// </summary>
    public class I2CReadAction : I2cAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="I2CReadAction"/> class.
        /// </summary>
        /// <param name="buffer">The buffer which should be used to store the received data.</param>
        public I2CReadAction(params byte[] buffer)
            : base(buffer)
        {
        }
    }
}
