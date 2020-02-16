using System;

namespace Raspberry.IO.InterIntegratedCircuit.NetStandard
{
    /// <summary>
    /// Abstract class for i2c Actions 
    /// </summary>
    public abstract class I2cAction
    {
        /// <summary>
        /// Must be inherited by class used during transactions
        /// </summary>
        /// <param name="buffer">the buffer.</param>
        protected I2cAction(byte[] buffer)
        {
            Buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
        }

        /// <summary>
        /// Action Buffer
        /// </summary>
        public byte[] Buffer { get; private set; }

    }
}
