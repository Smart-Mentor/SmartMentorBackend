   
   namespace SmartMentor.Abstraction.Dto.SharedRequestsAndResponses
   {
    public class SuccessResponse
    {
        public bool Success { get; set; }
        public required string Message { get; set; }
        public object? Data { get; set; }
    }
   }