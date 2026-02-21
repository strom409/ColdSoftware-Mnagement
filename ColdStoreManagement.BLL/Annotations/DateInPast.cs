using System.ComponentModel.DataAnnotations;

namespace ColdStoreManagement.BLL.Annotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class DateInPastAttribute : ValidationAttribute
    {
        public int YearsAgo { get; set; } = 0;

        public override bool IsValid(object? value)
        {
            if (value is null)
            {
                return true; // ValidationAttribute convention: null is valid, use [Required] for null checks
            }

            return value is DateTime date && date <= DateTime.Now.AddYears(-YearsAgo);
        }
    }
}
