using System.ComponentModel.DataAnnotations;

namespace App.Model.Entities
{
    public class Product : IValidatableObject
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public decimal Price { get; set; }
        public string Description { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Name == null || Name.Length < 3)
            {
                yield return new ValidationResult("Name must be at least 3 characters long.");
            }
            if (Description == null || Description.Length < 10)
            {
                yield return new ValidationResult("Description must be at least 10 characters long.");
            }
            if (Price < 0)
            {
                yield return new ValidationResult("Price cannot be negative.");
            }
        }
    }
}
