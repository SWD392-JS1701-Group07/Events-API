using Events.Business.Services;
using Events.Business.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Events.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/subjects")]
    [ApiVersionNeutral]
    public class SubjectController : Controller
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }
        [HttpGet]
        public async Task<IActionResult> ViewAllSubjects()
        {
            var subjects = await _subjectService.GetAllSubjects();
            return StatusCode(subjects.StatusCode, subjects);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ViewSubjectById(int id)
        {
            var subjects = await _subjectService.GetSubjectById(id);
            return StatusCode(subjects.StatusCode, subjects);
        }
    }
}
