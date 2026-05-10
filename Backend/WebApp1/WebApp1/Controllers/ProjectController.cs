using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using WebApp1.Core.Interfaces;
using WebApp1.Core.Models;
namespace WebApp1.Controllers;


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
            var (id,saved,updated)= await _repo.UpsertProjectWithUnits(prj);
            return Ok(new { id = id,saved=saved,updated=updated });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message, errorOccured = true });
        }
    }

    //// Delete Projects (Master) With Units (Details) 
    [Route("DeleteProject")]
    [HttpDelete("DeleteProject/{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        try
        {
            var result = await _repo.DeleteProject(id);
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
        var dt = await _repo.GetAllProjects();
        return Ok(dt);
    }
    [HttpGet("GetUnitsByProject/{id}")]
    public async Task<IActionResult> GetUnitsByProject(int id)
    {
        try
        {
            var dt = await _repo.GetUnitsByProjectId(id);
            return Ok(dt);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
