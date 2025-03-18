using Lab5TestTask.Data;
using Lab5TestTask.Enums;
using Lab5TestTask.Models;
using Lab5TestTask.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;


namespace Lab5TestTask.Services.Implementations;

/// <summary>
/// UserService implementation.
/// Implement methods here.
/// </summary>
public class UserService : IUserService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<SessionService> _logger;


    public UserService(ApplicationDbContext dbContext, ILogger<SessionService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<User> GetUserAsync()
    {
        try
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .OrderByDescending(u => u.Sessions.Count)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                _logger.LogWarning("No users found in the database.");
            }
            else
            {
                _logger.LogInformation("Retrieved user {UserId} with the most sessions.", user.Id);
            }

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving the user with the most sessions.");
            return null;
        }
    }


    public async Task<List<User>> GetUsersAsync()
    {
        try
        {
            var users = await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.Sessions.Any(s => s.DeviceType == DeviceType.Mobile))
                .ToListAsync();

            if (users.Count == 0)
            {
                _logger.LogWarning("No users with at least one mobile session were found.");
            }
            else
            {
                _logger.LogInformation("Retrieved {UserCount} users with at least one mobile session.", users.Count);
            }

            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving users with at least one mobile session.");
            return new List<User>();
        }
    }

}
