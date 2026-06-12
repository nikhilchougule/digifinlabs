using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.ValueObjects
{
    public sealed record Address(
    string Line1,
    string? Line2,
    string City,
    string State,
    string Pincode,
    string Country = "India",
    GeoCoordinate? Coordinates = null)
    {
        public override string ToString() =>
            string.Join(", ",
                new[] { Line1, Line2, City, State, Pincode, Country }
                .Where(s => !string.IsNullOrWhiteSpace(s)));
    }

}
