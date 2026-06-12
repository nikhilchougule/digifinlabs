using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Interfaces
{
    public interface IAuditable
    {
        Guid Id { get; }
        DateTime Timestamp { get; }
    }
}
