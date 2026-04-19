namespace SmartMentor.Abstraction.Dto.Requests.AuthRequests
{
    public class ResetPasswordRequest
    {
        public string Email { get; set; }
        public Guid ResetToken { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string Code { get; set; }
    }
}