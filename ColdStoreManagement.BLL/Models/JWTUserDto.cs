
namespace ColdStoreManagement.BLL.Models
{
    public class JWTUserDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string UnitName { get; set; } = null!;        
        public string Jti { get; set; } = null!;
    }
}
