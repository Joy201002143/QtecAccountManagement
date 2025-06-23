using System.ComponentModel.DataAnnotations;

namespace QtecAccountManagement.Models
{
    public class AuthModels
    {
        public class LoginModel
        {
            [Required]
            public string Username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public class UserModel
        {
            public int UserId { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string FullName { get; set; }
            public List<string> Roles { get; set; } = new List<string>();
        }
    }
}
