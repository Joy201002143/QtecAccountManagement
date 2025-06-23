namespace QtecAccountManagement.Services
{
    public class IAuthService
    {
        Task<UserModel> Authenticate(string username, string password);
        Task<bool> HasAccess(int userId, string requiredRole);
    }
}
