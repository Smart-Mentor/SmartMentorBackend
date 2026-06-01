namespace SmartMentor.Abstraction.Dto.Responses.AdminAnalyticsResponse
{
    public class UserGrowthTrendResponse
    {
        public string GroupBy { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<UserGrowthTrendPointResponse> Points { get; set; } = new();
    }

    public class UserGrowthTrendPointResponse
    {
        public string PeriodLabel { get; set; } = string.Empty;
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public int Registrations { get; set; }
        public int VerifiedUsers { get; set; }
        public int ProfileCompletedUsers { get; set; }
        public int ActiveUsers { get; set; }
    }
}
