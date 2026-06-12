using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.ValueObjects
{
    public sealed record EncryptedValue
    {
        public string CipherText { get; init; }
        public int KeyVersion { get; init; }

        public EncryptedValue(string cipherText, int keyVersion = 1)
        {
            CipherText = cipherText;
            KeyVersion = keyVersion;
        }

        public override string ToString() => "[REDACTED]"; // Prevents accidental log leakage
    }
}
