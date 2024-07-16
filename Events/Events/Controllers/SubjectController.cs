using Events.Business.Services;
using Events.Business.Services.Interfaces;
using Events.Models.DTOs.Request;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPost]
        [Authorize(Roles = "1, 4")]
        public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectDTO createSubject)
        {
            var subjects = await _subjectService.CreateSubject(createSubject);
            return StatusCode(subjects.StatusCode, subjects);
        }

        [HttpPut("id")]
        [Authorize(Roles = "1, 4")]
        public async Task<IActionResult> UpdateSubject(int id, [FromBody] CreateSubjectDTO createSubject)
        {
            var subjects = await _subjectService.UpdateSubject(id, createSubject);
            return StatusCode(subjects.StatusCode, subjects);   
        }
    }
}
