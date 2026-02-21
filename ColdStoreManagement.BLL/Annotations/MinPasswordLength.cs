using System.ComponentModel.DataAnnotations;

namespace ColdStoreManagement.BLL.Annotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class MinPasswordLengthAttribute : ValidationAttribute
    {
        int MinLength { get; }
        public MinPasswordLengthAttribute(int minLength, string errorMsg) : base(errorMsg) 
        {
            MinLength = minLength;
        }

        public override bool IsValid(object? value)
        {
            if (value is null)
            {
                return false; // ValidationAttribute convention: null is valid, use [Required] for null checks
            }
            return value is string str && str.Length >= MinLength;
        }
    }
}
