using Microsoft.AspNetCore.Mvc;
using SmartMentor.Abstraction.Services.AdminService;

namespace smartmentor.Api.Controllers.InterestsContrlloer
{
    [Route("api/[controller]")]
    [ApiController]
    public class InterestController : ControllerBase
    {
        private readonly IAdminService _interestService;
        private readonly ILogger<InterestController> _logger;

        public InterestController(IAdminService interestService, ILogger<InterestController> logger)
        {
            _interestService = interestService;
            _logger = logger;
        }

        [HttpGet("GetAllInterests")]
        public async Task<IActionResult> GetAllInterests()
        {
            var interests = await _interestService.GetAllInterestsAsync();
            return Ok(interests);
        }
        [HttpGet("GetInterestById/{id}")]
        public async Task<IActionResult> GetInterestById(int id)
        {
            var interest = await _interestService.GetInterestByIdAsync(id);
            if (interest == null)
            {
                return NotFound();
            }
            return Ok(interest);
        }
        
    }
}