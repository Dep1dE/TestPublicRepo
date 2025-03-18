using Lab5TestTask.Data;
using Lab5TestTask.Enums;
using Lab5TestTask.Models;
using Lab5TestTask.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Lab5TestTask.Services.Implementations;

/// <summary>
/// SessionService implementation.
/// Implement methods here.
/// </summary>
public class SessionService : ISessionService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<SessionService> _logger;

    public SessionService(ApplicationDbContext dbContext, ILogger<SessionService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Session> GetSessionAsync()
    {
        Session session = null;
        try
        {
            session = await _dbContext.Sessions
                .AsNoTracking() 
                .Where(s => s.DeviceType == DeviceType.Desktop)
                .OrderBy(s => s.StartedAtUTC)
                .FirstOrDefaultAsync();

            return session;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving the earliest desktop session.");
            return null;
        }
        finally
        {
            if (session != null)
            {
                _logger.LogDebug("Session found: ID = {SessionId}, Started at = {StartedAtUTC}", session.Id, session.StartedAtUTC);
            }
            else
            {
                _logger.LogWarning("No desktop sessions were found.");
            }
        }
    }

    public async Task<List<Session>> GetSessionsAsync()
    {
        try
        {
            var sessions = await _dbContext.Sessions
                .AsNoTracking()
                .Where(s => s.User.Status == UserStatus.Active && s.EndedAtUTC < new DateTime(2025, 1, 1))
                .ToListAsync();

            if (sessions.Count == 0)
            {
                _logger.LogWarning("No active user sessions ended before 2025 were found.");
            }
            else
            {
                _logger.LogInformation("Retrieved {SessionCount} active user sessions that ended before 2025.", sessions.Count);
            }

            return sessions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving sessions of active users ended before 2025.");
            return new List<Session>();
        }
    }
}
