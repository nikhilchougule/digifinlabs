using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.DTOs
{
    public sealed record TokenPair(
        string AccessToken,
        string RefreshToken,
        DateTime AccessTokenExpiresAt,
        DateTime RefreshTokenExpiresAt
    );
}
