using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Interfaces
{
    public interface ISoftDeletable
    {
        DateTime? DeletedAt { get; set; } //The timestamp of when the entity was soft deleted, null if the entity is not deleted. This allows us to track when an entity was deleted and to query for deleted entities if needed.
        bool IsDeleted { get; } //A computed property that returns true if DeletedAt is not null (i.e. the entity is soft deleted), and false if DeletedAt is null (i.e. the entity is active). This provides a convenient way to check if an entity is deleted without needing to check the DeletedAt property directly.

    }
}
