using System;

namespace Raspberry.IO.Interop.NetStandard.EnumTypes
{
    [Flags]
    public enum MemoryFlags
    {
        None = 0,
        Shared = 1
    }
}