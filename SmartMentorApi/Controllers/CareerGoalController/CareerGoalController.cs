using Microsoft.AspNetCore.Mvc;
using SmartMentor.Abstraction.Services.AdminService;

namespace smartmentor.Api.Controllers.CareerGoalController
{
    [Route("api/[controller]")]
    [ApiController]
    public class CareerGoalController : ControllerBase
    {
        private readonly IAdminService _careerGoalService;
        private readonly ILogger<CareerGoalController> _logger;

        public CareerGoalController(IAdminService careerGoalService, ILogger<CareerGoalController> logger)
        {
            _careerGoalService = careerGoalService;
            _logger = logger;
        }

        [HttpGet("GetAllCareerGoals")]
        public async Task<IActionResult> GetAllCareerGoals()
        {
            var careerGoals = await _careerGoalService.GetAllCareerGoalsAsync();
            return Ok(careerGoals);
        }
        [HttpGet("GetCareerGoalById/{id}")]
        public async Task<IActionResult> GetCareerGoalById(int id)
        {
            var careerGoal = await _careerGoalService.GetCareerGoalByIdAsync(id);
            if (careerGoal == null)
            {
                return NotFound();
            }
            return Ok(careerGoal);
        }
        
    }
}