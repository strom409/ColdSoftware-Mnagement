using ColdStoreManagement.BLL.Data;
using ColdStoreManagement.BLL.Models.Chamber;
using System.ComponentModel.DataAnnotations;

namespace ColdStoreManagement.BLL.Validators
{
    public class UniqueChamberNameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return new ValidationResult("Chamber name required.");
            }
            var dbContext = validationContext.GetService(typeof(AppDbContext)) as AppDbContext;
            if (dbContext is null)
            {
                return new ValidationResult("Database context not available.");
            }
            var chamberName = (string)value;

            var model = (AddChamberVM)validationContext.ObjectInstance;
            int currentId = model.ChamberId; // 0 if adding, non-zero if editing

            var exists = dbContext.chamber
                .Any(c => c.chambername == chamberName && c.chamberid != currentId);

            if (exists)
            {
                return new ValidationResult("Chamber name already exists.");
            }

#pragma warning disable CS8603 // Possible null reference return.
            return ValidationResult.Success;
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
