using System;
using System.Collections.Generic;
using System.Text;
using SharedKernel.ValueObjects;

namespace SharedKernel.Interfaces
{
    public interface IEncryptable
    {
        int EncryptionKeyVersion { get; set; } //Used to track which version of the encryption key was used to encrypt this entity, so we can re-encrypt it with the latest key if needed.
        IEnumerable<EncryptedValue> GetEncryptedFields(); //Returns a list of EncryptedValue objects representing the fields that need to be encrypted for this entity, including the field name, value, and any additional metadata needed for encryption. This allows us to have a consistent way of identifying which fields need to be encrypted across different entities, and to handle the encryption logic in a centralized way.
    }
}
