using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Interfaces
{
    public interface IValidatable
    {
        IEnumerable<string> Validate(); //Returns a list of validation error messages if the entity is not valid, or an empty list if the entity is valid. This allows us to have a consistent way of validating entities across different types, and to handle the validation logic in a centralized way.
    }
}
