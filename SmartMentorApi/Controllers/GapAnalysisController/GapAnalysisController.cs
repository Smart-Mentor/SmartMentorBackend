using System.Security.Claims;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartMentor.Abstraction.Dto.Responses.GapAnalysisResponse;
using SmartMentor.Abstraction.Services.GapAnalysisService;

namespace SmartMentorApi.Controllers.GapAnalysisController
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GapAnalysisController : ControllerBase
    {
        private readonly ILogger<GapAnalysisController> _logger;
        private readonly IGapAnalysisService _gapAnalysisService;

        public GapAnalysisController(
            ILogger<GapAnalysisController>logger,
            IGapAnalysisService gapAnalysisService
            )
        {
            _logger = logger;
            _gapAnalysisService = gapAnalysisService;
        }
        [HttpGet("gap-analysis")]
        public async Task<GapAnalysisResponse> Analysis()
        {
            var userid=HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userid))
            {
                _logger.LogWarning("User ID not found in claims.");
                throw new UnauthorizedAccessException("User ID not found in claims.");
            }
            var result= _gapAnalysisService.AnalyzeGapAsync(Guid.Parse(userid),cancellationToken: default);
            if (result == null)
            {
                _logger.LogWarning("Gap analysis result is null for user {UserId}", userid);
                throw new Exception("Gap analysis failed. Please try again later.");
            }
            return await result;
        }
    }
}
