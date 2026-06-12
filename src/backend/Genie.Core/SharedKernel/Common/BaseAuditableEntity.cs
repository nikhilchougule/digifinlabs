using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Common
{
    public abstract class BaseAuditableEntity : BaseEntity
    {
        /// <summary>UserId who created the record. Null for system-generated records (background jobs, AA sync).</summary>
        public Guid? CreatedBy { get; set; }
        public Guid? LastModifiedBy { get; set; }
    }
}
