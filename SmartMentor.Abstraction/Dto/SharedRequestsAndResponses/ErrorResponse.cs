namespace SmartMentor.Abstraction.Dto.SharedRequestsAndResponses
{
    public class ErrorResponse
    {
        public bool Success { get; set; }
        public required string Message { get; set; }
        public required string ErrorCode { get; set; }
        public List<ErrorDetail> Errors { get; set; } = new List<ErrorDetail>();
    }


}