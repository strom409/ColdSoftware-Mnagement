using ColdStoreManagement.BLL.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ColdStoreManagement.BLL.Data.UserData
{
    public class UserDataBase 
    {
        [Required(ErrorMessage = "The Username value should be specified.")]
        public string Username { get; set; } = null!;
        [Required(ErrorMessage = "The Password value should be specified.")]
        [MinPasswordLength(6, "The Password must be at least 6 characters long.")]
        public string Password { get; set; } = null!;
    }

    public class UserData: UserDataBase 
    {
        public static DateTime BirthDateNullValue { get; set; } = new DateTime(1970, 1, 1);

        [Required(ErrorMessage = "The Email value should be specified.")]
        [Email(ErrorMessage = "The Email value is invalid.")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "The Phone value should be specified.")]
        public string Phone { get; set; } = null!;

        [DateInPast(YearsAgo = 18,  ErrorMessage = "Users should be at least 18 years old to register.")]
        public DateTime BirthDate { get; set; } = BirthDateNullValue;
        public string Occupation { get; set; }
        public string Notes { get; set; }
    }
}
