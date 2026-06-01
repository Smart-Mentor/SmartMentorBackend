using SmartMentor.Abstraction.Dto.Responses.AdminAnalyticsResponse;

namespace SmartMentor.Abstraction.Services.AdminAnalyticsService
{
    public interface IAdminAnalyticsService
    {
        Task<AdminAnalyticsOverviewResponse> GetOverviewAsync(CancellationToken cancellationToken = default);
        Task<UserGrowthTrendResponse> GetUserGrowthAsync(
            string groupBy,
            DateTime? startDate,
            DateTime? endDate,
            CancellationToken cancellationToken = default);
        Task<OnboardingFunnelResponse> GetOnboardingFunnelAsync(CancellationToken cancellationToken = default);
    }
}
