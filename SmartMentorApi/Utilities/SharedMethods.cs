using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SmartMentor.Abstraction.Dto.SharedRequestsAndResponses;

namespace SmartMentorApi.Utilities
{
    public abstract class SharedMethods : ControllerBase
    {

        protected bool TryGetCurrentUserId(out Guid userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out userId);
        }

        protected ErrorResponse BuildUnauthorizedResponse()
        {
            return new ErrorResponse
            {
                Success = false,
                Message = "Authentication failed. User ID not found in token.",
                ErrorCode = "AUTH_001",
                Errors = new List<ErrorDetail>()
            };
        }

        protected ErrorResponse BuildValidationResponse()
        {
            var validationErrors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors.Select(e => new ErrorDetail
                {
                    Field = x.Key,
                    Message = e.ErrorMessage
                }))
                .ToList();

            return new ErrorResponse
            {
                Success = false,
                Message = "Validation failed. Please check the provided data and try again.",
                ErrorCode = "VALIDATION_001",
                Errors = validationErrors
            };
        }

        protected ErrorResponse BuildNotFoundResponse(string field, string message, string errorCode)
        {
            return new ErrorResponse
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode,
                Errors = new List<ErrorDetail>
                {
                    new ErrorDetail
                    {
                        Field = field,
                        Message = message
                    }
                }
            };
        }

        protected ErrorResponse BuildServerErrorResponse(string message)
        {
            return new ErrorResponse
            {
                Success = false,
                Message = message,
                ErrorCode = "COMMUNITY_500",
                Errors = new List<ErrorDetail>()
            };
        }
    }
}
