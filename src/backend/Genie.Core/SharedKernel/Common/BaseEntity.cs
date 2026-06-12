using SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Common
{
    public abstract class BaseEntity : ISoftDeletable, IValidatable
    {
        public Guid Id { get; init; } = Guid.NewGuid(); //Set once on creation, never change this
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow; //Set once on creation, never change this
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; //Updated on every EF Core SaveChanges() call
        public DateTime? DeletedAt { get; set; } //Set when the entity is soft deleted. null=active, set=soft deleted
        public bool IsDeleted => DeletedAt.HasValue; //Computed property

        public virtual IEnumerable<string> Validate()
        {
            if (Id == Guid.Empty)
                yield return "Id must not be empty.";
        }

        public virtual string GetDisplayName()
        {
            return $"{GetType().Name}:{Id}";
        }

    }
}
