using ColdStoreManagement.BLL.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ColdStoreManagement.BLL.Models.Auth
{
    public class LoginModel
    {
        [Required(ErrorMessage = "The Username value should be specified.")]
        public string GlobalUserName { get; set; } = null!;
        [Required(ErrorMessage = "The Password value should be specified.")]
        // [MinPasswordLength(6, "The Password must be at least 6 characters long.")]
        public string UserPassword { get; set; } = null!;
        [Required(ErrorMessage = "Please select a unit.")]
        public string GlobalUnitName { get; set; } = null!;
    }
    public class LoginResultModel
    {
        // --- Status Fields ---
        /// <summary>
        /// Expected values: "TRUE" or "FALSE"
        /// </summary>
        public string RetFlag { get; set; } = "FALSE";

        /// <summary>
        /// Error message or success notification from the Database/Stored Procedure
        /// </summary>
        public string? RetMessage { get; set; }

        // --- User Identity Fields ---
        public int GlobalUserId { get; set; }

        public string GlobalUserName { get; set; } = string.Empty;

        // --- Organizational Fields ---
        public int GlobalUnitId { get; set; }

        public string? GlobalUnitName { get; set; }

        /// <summary>
        /// User role or permission group (e.g., "Admin", "Operator")
        /// </summary>
        public string GlobalUserGroup { get; set; } = string.Empty;

        // --- Metadata (Optional but helpful) ---
        public DateTime? LastLoginDate { get; set; }
    }
}
