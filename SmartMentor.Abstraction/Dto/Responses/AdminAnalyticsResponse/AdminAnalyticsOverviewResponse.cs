namespace SmartMentor.Abstraction.Dto.Responses.AdminAnalyticsResponse
{
    public class AdminAnalyticsOverviewResponse
    {
        public int TotalUsers { get; set; }
        public int NewUsersLast30Days { get; set; }
        public int VerifiedUsers { get; set; }
        public int ProfileCompletedUsers { get; set; }
        public int UsersWithCareerGoal { get; set; }
        public int ActiveUsersLast30Days { get; set; }
        public decimal VerificationRate { get; set; }
        public decimal ProfileCompletionRate { get; set; }
        public decimal CareerGoalSelectionRate { get; set; }
        public decimal ActiveUserRateLast30Days { get; set; }
    }
}
