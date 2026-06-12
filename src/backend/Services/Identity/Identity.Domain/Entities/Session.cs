using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using SharedKernel.Common;
using SharedKernel.Enums;
using SharedKernel.ValueObjects;
using SharedKernel.Entities;

namespace Identity.Domain.Entities
{
    internal sealed class Session : BaseEntity
    {
        public required Guid UserId { get; set; }
        public required string RefreshTokenHash { get; init; }
        public IPAddress? IpAddress { get;init; }
        public string? UserAgent { get; init; }
        public required DateTime ExpiresAt { get; init; }
        public DateTime? RevokedAt { get; set; }

        public bool IsValid=> RevokedAt is null && ExpiresAt > DateTime.UtcNow;

        //Navigation properties
        public SharedKernel.Entities.User? User { get; set; }
    }
}
