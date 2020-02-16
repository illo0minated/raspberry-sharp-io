using System;

namespace Raspberry.IO.Interop.NetStandard.EnumTypes
{
    [Flags]
    public enum MemoryProtection
    {
        None = 0,
        Read = 1,
        Write = 2,
        ReadWrite = Read | Write
    }
}