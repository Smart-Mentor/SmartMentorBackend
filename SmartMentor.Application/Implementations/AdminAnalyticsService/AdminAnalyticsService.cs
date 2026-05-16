using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartMentor.Abstraction.Dto.Responses.AdminAnalyticsResponse;
using SmartMentor.Abstraction.Services.AdminAnalyticsService;
using SmartMentor.Persistence.Identity;
using System.Globalization;

namespace SmartMentor.Application.Implementations.AdminAnalyticsService
{
    public class AdminAnalyticsService : IAdminAnalyticsService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdminAnalyticsService> _logger;

        public AdminAnalyticsService(
            UserManager<ApplicationUser> userManager,
            ILogger<AdminAnalyticsService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<AdminAnalyticsOverviewResponse> GetOverviewAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var last30Days = now.AddDays(-30);

            var users = await _userManager.Users
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var totalUsers = users.Count;
            var verifiedUsers = users.Count(u => u.EmailConfirmed);
            var profileCompletedUsers = users.Count(u => u.IsProfileCompleted);
            var usersWithCareerGoal = users.Count(u => u.CareerGoalId.HasValue);
            var newUsersLast30Days = users.Count(u => u.CreatedAt >= last30Days);
            var activeUsersLast30Days = users.Count(u => u.LastLoginAt.HasValue && u.LastLoginAt.Value >= last30Days);

            return new AdminAnalyticsOverviewResponse
            {
                TotalUsers = totalUsers,
                NewUsersLast30Days = newUsersLast30Days,
                VerifiedUsers = verifiedUsers,
                ProfileCompletedUsers = profileCompletedUsers,
                UsersWithCareerGoal = usersWithCareerGoal,
                ActiveUsersLast30Days = activeUsersLast30Days,
                VerificationRate = CalculateRate(verifiedUsers, totalUsers),
                ProfileCompletionRate = CalculateRate(profileCompletedUsers, totalUsers),
                CareerGoalSelectionRate = CalculateRate(usersWithCareerGoal, totalUsers),
                ActiveUserRateLast30Days = CalculateRate(activeUsersLast30Days, totalUsers)
            };
        }

        public async Task<UserGrowthTrendResponse> GetUserGrowthAsync(
            string groupBy,
            DateTime? startDate,
            DateTime? endDate,
            CancellationToken cancellationToken = default)
        {
            var normalizedGroupBy = NormalizeGroupBy(groupBy);
            var rangeEnd = (endDate ?? DateTime.UtcNow).Date.AddDays(1).AddTicks(-1);
            var rangeStart = (startDate ?? rangeEnd.AddDays(-29)).Date;

            if (rangeStart > rangeEnd)
            {
                throw new ArgumentException("startDate cannot be later than endDate.");
            }

            var users = await _userManager.Users
                .AsNoTracking()
                .Where(u =>
                    u.CreatedAt <= rangeEnd ||
                    (u.EmailVerifiedAt.HasValue && u.EmailVerifiedAt.Value <= rangeEnd) ||
                    (u.ProfileCompletedAt.HasValue && u.ProfileCompletedAt.Value <= rangeEnd) ||
                    (u.LastLoginAt.HasValue && u.LastLoginAt.Value <= rangeEnd))
                .ToListAsync(cancellationToken);

            var points = BuildPeriods(rangeStart, rangeEnd, normalizedGroupBy)
                .Select(period => new UserGrowthTrendPointResponse
                {
                    PeriodLabel = period.Label,
                    PeriodStart = period.Start,
                    PeriodEnd = period.End,
                    Registrations = users.Count(u => u.CreatedAt >= period.Start && u.CreatedAt <= period.End),
                    VerifiedUsers = users.Count(u => u.EmailVerifiedAt.HasValue && u.EmailVerifiedAt.Value >= period.Start && u.EmailVerifiedAt.Value <= period.End),
                    ProfileCompletedUsers = users.Count(u => u.ProfileCompletedAt.HasValue && u.ProfileCompletedAt.Value >= period.Start && u.ProfileCompletedAt.Value <= period.End),
                    ActiveUsers = users.Count(u => u.LastLoginAt.HasValue && u.LastLoginAt.Value >= period.Start && u.LastLoginAt.Value <= period.End)
                })
                .ToList();

            return new UserGrowthTrendResponse
            {
                GroupBy = normalizedGroupBy,
                StartDate = rangeStart,
                EndDate = rangeEnd,
                Points = points
            };
        }

        public async Task<OnboardingFunnelResponse> GetOnboardingFunnelAsync(CancellationToken cancellationToken = default)
        {
            var users = await _userManager.Users
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var totalRegistered = users.Count;
            var verified = users.Count(u => u.EmailConfirmed);
            var profileCompleted = users.Count(u => u.IsProfileCompleted);
            var selectedCareerGoal = users.Count(u => u.CareerGoalId.HasValue);

            return new OnboardingFunnelResponse
            {
                TotalRegisteredUsers = totalRegistered,
                Stages = new List<OnboardingFunnelStageResponse>
                {
                    BuildStage("Registered", totalRegistered, totalRegistered),
                    BuildStage("Email Verified", verified, totalRegistered),
                    BuildStage("Profile Completed", profileCompleted, totalRegistered),
                    BuildStage("Career Goal Selected", selectedCareerGoal, totalRegistered)
                }
            };
        }

        private static OnboardingFunnelStageResponse BuildStage(string stageName, int count, int totalRegistered)
        {
            return new OnboardingFunnelStageResponse
            {
                StageName = stageName,
                Count = count,
                PercentageFromRegistrations = CalculateRate(count, totalRegistered)
            };
        }

        private static decimal CalculateRate(int value, int total)
        {
            if (total == 0)
            {
                return 0;
            }

            return Math.Round((decimal)value / total * 100, 2);
        }

        private static string NormalizeGroupBy(string groupBy)
        {
            var normalized = string.IsNullOrWhiteSpace(groupBy) ? "day" : groupBy.Trim().ToLowerInvariant();

            return normalized switch
            {
                "day" => normalized,
                "week" => normalized,
                "month" => normalized,
                _ => throw new ArgumentException("groupBy must be one of: day, week, month.")
            };
        }

        private static List<(DateTime Start, DateTime End, string Label)> BuildPeriods(DateTime startDate, DateTime endDate, string groupBy)
        {
            var periods = new List<(DateTime Start, DateTime End, string Label)>();
            var cursor = startDate;

            while (cursor <= endDate)
            {
                DateTime periodStart;
                DateTime periodEnd;
                string label;

                switch (groupBy)
                {
                    case "week":
                        var weekStart = cursor.AddDays(-(int)cursor.DayOfWeek + (cursor.DayOfWeek == DayOfWeek.Sunday ? -6 : 1));
                        if (weekStart < startDate)
                        {
                            weekStart = startDate;
                        }

                        periodStart = weekStart.Date;
                        periodEnd = periodStart.AddDays(6).Date.AddDays(1).AddTicks(-1);
                        if (periodEnd > endDate)
                        {
                            periodEnd = endDate;
                        }

                        label = $"{periodStart:yyyy-MM-dd} - {periodEnd:yyyy-MM-dd}";
                        cursor = periodEnd.Date.AddDays(1);
                        break;
                    case "month":
                        periodStart = new DateTime(cursor.Year, cursor.Month, 1);
                        if (periodStart < startDate)
                        {
                            periodStart = startDate;
                        }

                        periodEnd = new DateTime(cursor.Year, cursor.Month, DateTime.DaysInMonth(cursor.Year, cursor.Month))
                            .Date
                            .AddDays(1)
                            .AddTicks(-1);
                        if (periodEnd > endDate)
                        {
                            periodEnd = endDate;
                        }

                        label = periodStart.ToString("yyyy-MM", CultureInfo.InvariantCulture);
                        cursor = new DateTime(cursor.Year, cursor.Month, 1).AddMonths(1);
                        break;
                    default:
                        periodStart = cursor.Date;
                        periodEnd = cursor.Date.AddDays(1).AddTicks(-1);
                        if (periodEnd > endDate)
                        {
                            periodEnd = endDate;
                        }

                        label = periodStart.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                        cursor = cursor.Date.AddDays(1);
                        break;
                }

                periods.Add((periodStart, periodEnd, label));
            }

            return periods;
        }
    }
}
