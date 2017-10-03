namespace Raspberry.IO.InterIntegratedCircuit
{
    using System;

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
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            Buffer = buffer;
        }

        /// <summary>
        /// Action Buffer
        /// </summary>
        public byte[] Buffer { get; private set; }

    }
}
