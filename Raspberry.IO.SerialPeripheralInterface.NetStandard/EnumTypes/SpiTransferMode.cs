﻿using System;

namespace Raspberry.IO.SerialPeripheralInterface.NetStandard.EnumTypes
{
    /// <summary>
    /// Selects if data should be read, written or both.
    /// </summary>
    [Flags]
    public enum SpiTransferMode
    {
        /// <summary>
        /// Write data to the chip.
        /// </summary>
        Write = 1,

        /// <summary>
        /// Read data from the chip.
        /// </summary>
        Read = 2,

        /// <summary>
        /// Write and read data simultaneously.
        /// </summary>
        ReadWrite = 3
    }
}