using CourseEnrollBlaze.Server.Context;
using CourseEnrollBlaze.Server.Interfaces;
using CourseEnrollBlaze.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseEnrollBlaze.Server.Services
{
    public class UserAccountService : IUserAccountService
    {
        private readonly DbContextOptions<CEBDbContext> _dbContextOptions;
        public UserAccountService(DbContextOptions<CEBDbContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
            InitializeDummyDataAsync().Wait();
        }

        private async Task InitializeDummyDataAsync()
        {
            using (var context = new CEBDbContext(_dbContextOptions))
            {

                if (!context.UserAccounts.Any())
                {
                    context.UserAccounts.AddRange(
                        new UserAccount
                        {
                            Id = Guid.NewGuid(),
                            FirstName = "John",
                            LastName = "Doe",
                            Email = "john.doe@example.com",
                            Password = "$2y$10$Ni0P3vbDlSS5RnGBjt69weBvtGnF4kOWFtbjhrge0bPsxW55KhXNK",
                            StudentNumber = await GenerateStudentNumberAsync(),
                            IsEmailConfirmed = true,
                            Role = "Student"
                        },
                        new UserAccount
                        {
                            Id = Guid.NewGuid(),
                            FirstName = "Jane",
                            LastName = "Doe",
                            Email = "jane.doe@example.com",
                            Password = "$2y$10$qpsjhtISPx3ReeuqKSFt6e.Rt.ZbKf5atERPZ6BYaxnL.gKsMZr5y",
                            StudentNumber = await GenerateStudentNumberAsync(),
                            IsEmailConfirmed = true,
                            Role = "Instructor"
                        }
                    );

                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task RegisterAsync(UserAccount user)
        {
            using (var context = new CEBDbContext(_dbContextOptions))
            {
                user.StudentNumber = await GenerateStudentNumberAsync();

                var existingUser = await context.UserAccounts.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    throw new ArgumentException("Email is already taken.");
                }
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password, BCrypt.Net.BCrypt.GenerateSalt()); ;
                user.CreatedAt = DateTime.Now;

                context.UserAccounts.Add(user);
                await context.SaveChangesAsync();
            }
        }

        public async Task<UserAccount?> LoginAsync(string email, string password)
        {
            using (var context = new CEBDbContext(_dbContextOptions))
            {
                var user = await context.UserAccounts.FirstOrDefaultAsync(u => u.Email == email);

                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    return null;
                }
                return user;
            }
        }

        public async Task<List<UserAccount>> GetAllUsersAsync()
        {
            using (var context = new CEBDbContext(_dbContextOptions))
            {
                return await context.UserAccounts.ToListAsync();
            }
        }

        public async Task<UserAccount?> GetUserByIdAsync(Guid id)
        {
            using (var context = new CEBDbContext(_dbContextOptions))
            {
                var user = await context.UserAccounts.FirstOrDefaultAsync(u => u.Id == id);
                return user;
            }
        }

        public async Task<bool> UpdateUserAsync(UserAccount user)
        {
            using (var context = new CEBDbContext(_dbContextOptions))
            {
                var existingUser = await context.UserAccounts.FirstOrDefaultAsync(u => u.Id == user.Id);
                if (existingUser != null)
                {
                    existingUser.Password = user.Password;
                    existingUser.FirstName = user.FirstName;
                    existingUser.LastName = user.LastName;
                    existingUser.StudentNumber = existingUser.StudentNumber;
                    existingUser.IsEmailConfirmed = user.IsEmailConfirmed;
                    existingUser.Role = user.Role;
                    await context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            using (var context = new CEBDbContext(_dbContextOptions))
            {
                // Simulate asynchronous operation
                await Task.Delay(100); // Example delay

                var userToRemove = await context.UserAccounts.FirstOrDefaultAsync(u => u.Id == id);
                if (userToRemove != null)
                {
                    context.UserAccounts.Remove(userToRemove);
                    await context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
        }

        public async Task<string> GenerateStudentNumberAsync()
        {
            using (var context = new CEBDbContext(_dbContextOptions))
            {

                var currentYear = DateTime.UtcNow.Year.ToString();
                var lastStudentNumber = await context.UserAccounts
                    .Where(u => u.StudentNumber.StartsWith(currentYear))
                    .OrderByDescending(u => u.StudentNumber)
                    .Select(u => u.StudentNumber)
                    .FirstOrDefaultAsync();

                int number;
                if (lastStudentNumber != null && int.TryParse(lastStudentNumber.Substring(currentYear.Length), out number))
                {
                    return $"{currentYear}{number + 1:D5}";
                }
                else
                {
                    return $"{currentYear}00001";
                }
            }
        }
    }
}
