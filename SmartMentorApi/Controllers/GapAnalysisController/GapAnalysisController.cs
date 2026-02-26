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
        public async Task<ActionResult<GapAnalysisResponse>> Analysis(CancellationToken cancellationToken)
        {
            var userid=HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userid))
            {
                _logger.LogWarning("User ID not found in claims.");
               return Unauthorized("User ID not found in claims.");
            }
            if(!Guid.TryParse(userid, out var userGuid))
            {
                _logger.LogWarning("Invalid User ID format: {UserId}", userid);
                return Unauthorized("Invalid User ID format.");
            }
            var result= await _gapAnalysisService.AnalyzeGapAsync(userGuid,cancellationToken);
            if (result == null)
            {
                _logger.LogWarning("Gap analysis result is null for user {UserId}", userid);
                throw new Exception("Gap analysis failed. Please try again later.");
            }
            return Ok(result);
        }
    }
}
