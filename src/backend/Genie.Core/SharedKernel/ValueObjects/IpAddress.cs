using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.ValueObjects
{
    public sealed record IpAddress
    {
        public string Value { get; }

        public IpAddress(string raw)
        {
            if (!System.Net.IPAddress.TryParse(raw, out _))
                throw new ArgumentException($"'{raw}' is not a valid IP address.", nameof(raw));
            Value = raw;
        }

        public override string ToString() => Value;
    }
}
