namespace SmartMentor.Abstraction.Dto.Requests.AuthRequests
{
     public class MeResponse
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool? PhoneNumberConfirmed { get; set; }
        public List<string> Roles { get; set; }
    }
}