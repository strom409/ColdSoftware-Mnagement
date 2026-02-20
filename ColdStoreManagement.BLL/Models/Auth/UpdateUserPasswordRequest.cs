using ColdStoreManagement.BLL.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ColdStoreManagement.BLL.Models.Auth
{
    public sealed class UpdateUserPasswordRequest
    {
        [Required(ErrorMessage = "The UserId value should be specified.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "The NewPassword value should be specified.")]
        [MinPasswordLength(6, "The NewPassword must be at least 6 characters long.")]
        public string NewPassword { get; set; } = string.Empty;
       
        [Required(ErrorMessage = "The RetypePassword value must be specified.")]
        [Compare(nameof(NewPassword), ErrorMessage = "The password and confirmation password do not match.")]
        public string RetypePassword { get; set; } = string.Empty;

        public string GlobalUserName { get; set; } = string.Empty;

        public string? GlobalUnitName { get; set; }
    }
    public sealed class ChangePasswordRequest: IValidatableObject
    {
        [Required(ErrorMessage = "The Username value should be specified.")]      
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "The OldPassword value should be specified.")]
        public string OldPassword { get; set; } = string.Empty;
        [Required(ErrorMessage = "The NewPassword value should be specified.")]
        [MinPasswordLength(6, "The NewPassword must be at least 6 characters long.")]
        public string NewPassword { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (OldPassword == NewPassword)
            {
                yield return new ValidationResult(
                    "The NewPassword must be different from the OldPassword.",
                    new[] { nameof(NewPassword) });
            }
        }
    }


}
