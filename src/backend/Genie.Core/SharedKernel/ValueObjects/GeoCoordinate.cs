using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.ValueObjects
{
    public sealed record GeoCoordinate(double Latitude, double Longitude)
    {
        public override string ToString() => $"{Latitude:F6}, {Longitude:F6}";
    }
}
