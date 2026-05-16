using Microsoft.AspNetCore.Mvc;
using SmartMentor.Abstraction.Services.AdminService;

namespace smartmentor.Api.Controllers.SkillsController
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillController : ControllerBase
    {
        private readonly IAdminService _skillService;
        private readonly ILogger<SkillController> _logger;

        public SkillController(IAdminService skillService, ILogger<SkillController> logger)
        {
            _skillService = skillService;
            _logger = logger;
        }

        [HttpGet("GetAllSkills")]
        public async Task<IActionResult> GetAllSkills()
        {
            var skills = await _skillService.GetAllSkillsAsync();
            return Ok(skills);
        }
        [HttpGet("GetSkillById/{id}")]
        public async Task<IActionResult> GetSkillById(int id)
        {
            var skill = await _skillService.GetSkillByIdAsync(id);
            if (skill == null)
            {
                return NotFound();
            }
            return Ok(skill);
        }
        
    }
}