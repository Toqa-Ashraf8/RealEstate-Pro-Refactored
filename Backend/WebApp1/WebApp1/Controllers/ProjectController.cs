using Microsoft.AspNetCore.Mvc;
using WebApp1.Interfaces;
using WebApp1.Models;
namespace WebApp1.Controllers
  
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository _repo;
        public ProjectController(IProjectRepository repo)
        {
            _repo = repo;
        }
       
        [Route("UploadImage")]
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file, string folderName)
        {
            var filename=await _repo.UploadImage(file, folderName);
            return Ok(filename);
        }

        // Save Projects (Master) With Units (Details)
        [Route("UpsertProjectWithUnits")]
        [HttpPost]
        public async Task<IActionResult> UpsertProjectWithUnits([FromBody] Project prj)
        {
            try
            {
                if (prj == null) return BadRequest("Project data is null");

                int projectId = await _repo.UpsertProjectWithUnitsAsync(prj);
                return Ok(new { id = projectId, message = "Saved Successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message, errorOccured = true });
            }
        }

        //// Delete Projects (Master) With Units (Details) 
        [HttpDelete("DeleteProject/{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            try
            {
                var result = await _repo.DeleteProjectAsync(id);
                return Ok(new { delOk = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message, delOk = false });
            }
        }
        //Get Projects (Master) To Search 
        [Route("GetAllProjects")]
        [HttpGet]
        public async Task<IActionResult> GetAllProjects()
        {
            var dt = await _repo.GetAllProjectsAsync();
            return Ok(dt);

        }
        [HttpGet("GetUnitsByProject/{id}")]
        public async Task<IActionResult> GetUnitsByProject(int id)
        {
            try
            {
                var dt = await _repo.GetUnitsByProjectIdAsync(id);
                return Ok(dt);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
