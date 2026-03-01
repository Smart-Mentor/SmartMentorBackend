namespace SmartMentor.Abstraction.Dto.SharedRequestsAndResponses
{
    public class ErrorDetail
    {
        public required string Field { get; set; }
        public required string Message { get; set; }
        public string? Code { get; set; }
    }
}