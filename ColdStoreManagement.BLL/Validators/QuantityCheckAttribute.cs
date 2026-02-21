using ColdStoreManagement.BLL.Data;
using ColdStoreManagement.BLL.Models.Chamber;
using System.ComponentModel.DataAnnotations;

namespace ColdStoreManagement.BLL.Validators
{
    public class QuantityCheckAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var dbContext = validationContext.GetService(typeof(AppDbContext)) as AppDbContext;
            if (dbContext == null)
            {
                return new ValidationResult("Database context is not available.");
            }

            int editedQiantity = Convert.ToInt32(value);

            var model = (AddChamberVM)validationContext.ObjectInstance;
            int currentId = model.ChamberId; // 0 if adding, non-zero if editing

            var checkQuantityConsumed = dbContext.chamber
                .FirstOrDefault(c => c.chamberid == currentId)?.QuantityConsumed;

            if (checkQuantityConsumed > editedQiantity)
            {
                return new ValidationResult($"Chamber capacity cannot be less than quantity already consumed : {checkQuantityConsumed} ");
            }

#pragma warning disable CS8603 // Possible null reference return.
            return ValidationResult.Success;
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
