using System.Data;
using System.Text;

namespace QtecAccountManagement.Services
{
    public class AuthService : IAuthService
    {
        private readonly IDbConnection _db;

        public AuthService(IDbConnection db)
        {
            _db = db;
        }

        public async Task<UserModel> Authenticate(string username, string password)
        {
            using var connection = _db;
            var query = "SELECT u.UserId, u.Username, u.Email, u.FullName FROM Users u WHERE u.Username = @Username AND u.Password = @Password AND u.IsActive = 1";

            var user = await connection.QueryFirstOrDefaultAsync<UserModel>(query, new { Username = username, Password = HashPassword(password) });

            if (user != null)
            {
                var rolesQuery = "SELECT r.RoleName FROM UserRoles ur JOIN Roles r ON ur.RoleId = r.RoleId WHERE ur.UserId = @UserId";
                var roles = await connection.QueryAsync<string>(rolesQuery, new { UserId = user.UserId });
                user.Roles = roles.ToList();
            }

            return user;
        }

        public async Task<bool> HasAccess(int userId, string requiredRole)
        {
            using var connection = _db;
            var query = "SELECT COUNT(*) FROM UserRoles ur JOIN Roles r ON ur.RoleId = r.RoleId WHERE ur.UserId = @UserId AND r.RoleName = @RoleName";
            var count = await connection.ExecuteScalarAsync<int>(query, new { UserId = userId, RoleName = requiredRole });
            return count > 0;
        }

        private string HashPassword(string password)
        {
            // In a real application, use proper password hashing like BCrypt or ASP.NET Identity's PasswordHasher
            return Convert.ToBase64String(System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password)));
        }
    }
}
