using System.ComponentModel.DataAnnotations;

namespace ColdStoreManagement.BLL.Annotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true)]
    public class DateIsEarlierThanAttribute : ValidationAttribute 
    {
        readonly string datePropertyToCompare;
        public DateIsEarlierThanAttribute(string datePropertyToCompare)
        {
            this.datePropertyToCompare = datePropertyToCompare;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(datePropertyToCompare);
            if (property == null)
            {
                return new ValidationResult(
                    $"Unknown property: {datePropertyToCompare}",
                    new List<string> { validationContext.MemberName! });
            }

            var compareValue = property.GetValue(validationContext.ObjectInstance);

            if (value is DateTime dateValue && compareValue is DateTime compareDate)
            {
                if (dateValue > compareDate)
                {
                    return new ValidationResult(
                        ErrorMessage ?? $"The {validationContext.MemberName} value cannot be greater than the {datePropertyToCompare} value",
                        new List<string> { validationContext.MemberName!, datePropertyToCompare });
                }
            }
            return ValidationResult.Success!;
        }
    }
}
