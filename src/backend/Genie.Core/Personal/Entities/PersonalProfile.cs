using Personal.Enums;
using SharedKernel.Common;
using SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;

namespace Personal.Entities
{
    /// <summary>
    /// Profile of an individual (salaried / employed) user on the DigifinLabs Personal vertical.
    ///
    /// DESIGN RULES:
    ///   - 1:1 with User — UserId mirrors User.ProfileRefId.
    ///   - MonthlyIncome is PII — stored as AES-256 EncryptedValue.
    ///     Decrypt only within a trusted service boundary; never in query projections or logs.
    ///   - Age validation enforced at domain level (18+) in addition to UI validation.
    /// </summary>
    public sealed class PersonalProfile : BaseEncryptedEntity
    {
        public required Guid UserId { get; init; }

        public required string FullName { get; set; }

        public required DateOnly DateOfBirth { get; set; }

        /// <summary>'M' = Male | 'F' = Female | 'O' = Other / Prefer not to say.</summary>
        public required char Gender { get; set; }

        public required EmploymentType EmploymentType { get; set; }

        public string? EmployerName { get; set; }

        public string? Designation { get; set; }

        /// <summary>
        /// Gross monthly income in paise, AES-256 encrypted.
        /// Use Money value object for arithmetic after decryption.
        /// Never log, cache, or expose plaintext.
        /// </summary>
        public EncryptedValue? MonthlyIncome { get; set; }

        /// <summary>Current residential address. Stored as an EF Core owned entity.</summary>
        public Address? Address { get; set; }

        /// <summary>
        /// Nominee details as JSONB.
        /// Shape: { name, relation, dob, contactNumber }
        /// </summary>
        public Dictionary<string, object> NomineeDetails { get; set; } = new();

        public override IEnumerable<string> Validate()
        {
            foreach (var err in base.Validate())
                yield return err;

            if (UserId == Guid.Empty)
                yield return "UserId must not be empty.";

            if (string.IsNullOrWhiteSpace(FullName))
                yield return "FullName is required.";

            if (DateOfBirth >= DateOnly.FromDateTime(DateTime.UtcNow))
                yield return "DateOfBirth must be in the past.";

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - DateOfBirth.Year;
            if (today < DateOfBirth.AddYears(age)) age--;
            if (age < 18)
                yield return "Applicant must be at least 18 years old.";

            if (Gender is not ('M' or 'F' or 'O'))
                yield return "Gender must be 'M' (Male), 'F' (Female), or 'O' (Other).";

            if (EmploymentType == EmploymentType.Salaried && string.IsNullOrWhiteSpace(EmployerName))
                yield return "EmployerName is required for salaried individuals.";
        }

        public override string GetDisplayName() => $"Personal[{FullName}]:{Id}";
    }
}
