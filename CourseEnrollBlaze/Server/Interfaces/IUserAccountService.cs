using CourseEnrollBlaze.Shared.Entities;

namespace CourseEnrollBlaze.Server.Interfaces
{
    public interface IUserAccountService
    {
        Task RegisterAsync(UserAccount user);
        Task<UserAccount?> LoginAsync(string email, string password);
        Task<List<UserAccount>> GetAllUsersAsync();
        Task<UserAccount?> GetUserByIdAsync(Guid id);
        Task<bool> UpdateUserAsync(UserAccount user);
        Task<bool> DeleteUserAsync(Guid id); 
    }
}
