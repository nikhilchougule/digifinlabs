using Business.Entities;
using Microsoft.EntityFrameworkCore;
using Personal.Entities;
using SharedKernel.Entities;
using Los.Application.Entities;
using Los.Lending.Entities;
using Los.Servicing.Entities;
using Los.Underwriting.Entities;
using System.Text.RegularExpressions;

// Alias to avoid ambiguity with the Los.Application namespace component
using LosApplication = Los.Application.Entities.Application;

namespace Genie.Persistence
{
    public class GenieDbContext : DbContext
    {
        public GenieDbContext(DbContextOptions<GenieDbContext> options) : base(options) { }

        // ── identity schema ──────────────────────────────────────────────────────
        public DbSet<Tenant> Tenants => Set<Tenant>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Credential> Credentials => Set<Credential>();
        public DbSet<KycProfile> KycProfiles => Set<KycProfile>();
        public DbSet<OtpChallenge> OtpChallenges => Set<OtpChallenge>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        // ── shared schema ─────────────────────────────────────────────────────────
        public DbSet<ConsentLog> ConsentLogs => Set<ConsentLog>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<DocumentVault> DocumentVaults => Set<DocumentVault>();
        public DbSet<AccessGrant> AccessGrants => Set<AccessGrant>();

        // ── personal schema ───────────────────────────────────────────────────────
        public DbSet<PersonalProfile> PersonalProfiles => Set<PersonalProfile>();
        public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
        public DbSet<BankTransaction> BankTransactions => Set<BankTransaction>();
        public DbSet<CreditProfile> CreditProfiles => Set<CreditProfile>();
        public DbSet<LoanOffer> LoanOffers => Set<LoanOffer>();
        public DbSet<FinanceDashboard> FinanceDashboards => Set<FinanceDashboard>();

        // ── business schema ───────────────────────────────────────────────────────
        public DbSet<BusinessProfile> BusinessProfiles => Set<BusinessProfile>();
        public DbSet<Promoter> Promoters => Set<Promoter>();
        public DbSet<UdyamProfile> UdyamProfiles => Set<UdyamProfile>();
        public DbSet<GstProfile> GstProfiles => Set<GstProfile>();
        public DbSet<GstFiling> GstFilings => Set<GstFiling>();
        public DbSet<BusinessBankAccount> BusinessBankAccounts => Set<BusinessBankAccount>();
        public DbSet<BusinessCreditScore> BusinessCreditScores => Set<BusinessCreditScore>();
        public DbSet<FinancialStatement> FinancialStatements => Set<FinancialStatement>();

        // ── los schema ────────────────────────────────────────────────────────────
        public DbSet<LosApplication> Applications => Set<LosApplication>();
        public DbSet<ApplicationProduct> ApplicationProducts => Set<ApplicationProduct>();
        public DbSet<ApplicationDocument> ApplicationDocuments => Set<ApplicationDocument>();
        public DbSet<ApplicationStageLog> ApplicationStageLogs => Set<ApplicationStageLog>();
        public DbSet<ApplicationAssignment> ApplicationAssignments => Set<ApplicationAssignment>();
        public DbSet<TatTracker> TatTrackers => Set<TatTracker>();
        public DbSet<Lender> Lenders => Set<Lender>();
        public DbSet<LenderProduct> LenderProducts => Set<LenderProduct>();
        public DbSet<Disbursement> Disbursements => Set<Disbursement>();
        public DbSet<RepaymentSchedule> RepaymentSchedules => Set<RepaymentSchedule>();
        public DbSet<Repayment> Repayments => Set<Repayment>();
        public DbSet<UnderwritingAssessment> UnderwritingAssessments => Set<UnderwritingAssessment>();
        public DbSet<BankingAnalysis> BankingAnalyses => Set<BankingAnalysis>();
        public DbSet<IncomeAssessment> IncomeAssessments => Set<IncomeAssessment>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // All IEntityTypeConfiguration<T> classes in this assembly are picked up automatically
            builder.ApplyConfigurationsFromAssembly(typeof(GenieDbContext).Assembly);
            ApplySnakeCaseConvention(builder);
        }

        /// <summary>
        /// Converts any table/column names not already set explicitly in configurations
        /// to snake_case. Replaces EFCore.NamingConventions which has no stable EF10 release.
        /// </summary>
        private static void ApplySnakeCaseConvention(ModelBuilder builder)
        {
            foreach (var entity in builder.Model.GetEntityTypes())
            {
                // Table name (only if not already set by ToTable())
                if (entity.GetTableName() is { } table)
                    entity.SetTableName(ToSnakeCase(table));

                foreach (var prop in entity.GetProperties())
                {
                    var col = prop.GetColumnName();
                    if (col is not null)
                        prop.SetColumnName(ToSnakeCase(col));
                }

                foreach (var key in entity.GetKeys())
                    key.SetName(ToSnakeCase(key.GetName() ?? string.Empty));

                foreach (var idx in entity.GetIndexes())
                    idx.SetDatabaseName(ToSnakeCase(idx.GetDatabaseName() ?? string.Empty));
            }
        }

        private static string ToSnakeCase(string name)
            => Regex.Replace(name, "([a-z0-9])([A-Z])", "$1_$2").ToLowerInvariant();
    }
}
