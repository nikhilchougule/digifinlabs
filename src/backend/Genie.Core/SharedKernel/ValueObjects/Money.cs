using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.ValueObjects
{
    public sealed record Money
    {
        public long AmountInPaise { get; }
        public string CurrencyCode { get; }

        // Overload 1 — from paise (internal / DB use)
        public Money(long amountInPaise, string currencyCode = "INR")
        {
            AmountInPaise = amountInPaise;
            CurrencyCode = currencyCode;
        }

        // Overload 2 — from decimal rupees (API payloads / user input)
        public Money(decimal amountInRupees, string currencyCode = "INR")
            : this((long)(amountInRupees * 100), currencyCode) { }

        // Overload 3 — from string (CSV imports / AA framework data)
        public Money(string amountString, string currencyCode = "INR")
            : this(decimal.Parse(amountString), currencyCode) { }

        public static readonly Money Zero = new(0L);

        public Money Add(Money other) { EnsureSameCurrency(other); return new Money(AmountInPaise + other.AmountInPaise, CurrencyCode); }
        public Money Subtract(Money other) { EnsureSameCurrency(other); return new Money(AmountInPaise - other.AmountInPaise, CurrencyCode); }
        public Money Multiply(decimal factor) => new Money((long)(AmountInPaise * factor), CurrencyCode);

        /// <summary>For display only — never persist the result of this method.</summary>
        public decimal ToRupees() => AmountInPaise / 100m;

        public override string ToString() => $"{CurrencyCode} {ToRupees():N2}";

        private void EnsureSameCurrency(Money other)
        {
            if (CurrencyCode != other.CurrencyCode)
                throw new InvalidOperationException(
                    $"Cannot operate on different currencies: {CurrencyCode} vs {other.CurrencyCode}.");
        }
    }
}
