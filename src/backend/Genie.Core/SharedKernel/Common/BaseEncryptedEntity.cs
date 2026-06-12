using SharedKernel.Interfaces;
using SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Common
{
    public abstract class BaseEncryptedEntity : BaseAuditableEntity, IEncryptable
    {
        public int EncryptionKeyVersion { get; set; } = 1; //Tracks encryption key version for key rotation — only re-encrypt records with an older version.

        /// <summary>
        /// Override in derived classes to enumerate all encrypted fields.
        /// Used by the encryption service for key-rotation sweeps.
        /// Default returns empty — opt-in per entity.
        /// </summary>
        public virtual IEnumerable<EncryptedValue> GetEncryptedFields()
        {
            yield break;
        }
    }
}
