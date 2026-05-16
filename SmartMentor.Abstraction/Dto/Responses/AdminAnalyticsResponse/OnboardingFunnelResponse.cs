namespace SmartMentor.Abstraction.Dto.Responses.AdminAnalyticsResponse
{
    public class OnboardingFunnelResponse
    {
        public int TotalRegisteredUsers { get; set; }
        public List<OnboardingFunnelStageResponse> Stages { get; set; } = new();
    }

    public class OnboardingFunnelStageResponse
    {
        public string StageName { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal PercentageFromRegistrations { get; set; }
    }
}
