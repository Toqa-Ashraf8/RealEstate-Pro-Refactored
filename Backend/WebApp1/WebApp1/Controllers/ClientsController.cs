using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using WebApp1.EF;
using WebApp1.Interfaces;
using WebApp1.Models;
namespace WebApp1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _repo;
        private readonly IWebHostEnvironment _env;
        public ClientsController(IClientRepository repo, IWebHostEnvironment env)
        {
             _repo = repo;
             _env=env;
          
        }
        //Get All Projects in Database
        [Route("GetAllProjects")]
        [HttpGet]
        public async Task <IActionResult> GetAllProjects()
        {
            try
            {
                var dt = await _repo.GetAllProjects();
                return Ok(dt);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        // Get Units By Project Name 
        [Route("GetUnitsByProject")]
        [HttpPost]
        public async Task<IActionResult> GetUnitsByProject(int projectid)
        {
            try
            {
                var dt = await _repo.GetUnitsByProject(projectid);
                return Ok(dt);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // Get Price Of Unit 
        [Route("GetUnitPrice")]
        [HttpPost]
        public async Task<IActionResult> GetUnitPrice(int unitid)
        {
            try
            {
                var dt = await _repo.GetUnitPrice(unitid);
                return Ok(dt);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        //Save Clients with their negotiation requests 
        [Route("UpsertClient")]
        [HttpPost]
        public async Task<IActionResult> UpsertClient([FromBody] Client cl)
        {
            bool nullData = false;
            if (cl == null) return BadRequest(new {error="Client data is empty" , nullData = true });
            try
            {
               var (id, saved, updated) = await _repo.UpsertClient(cl);
                return Ok(new
                {
                    id = id,
                    saved = saved,
                    updated = updated,
                    nullData = false
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        //Delete Clients and their negotiations
        [Route("DeleteClient")]
        [HttpDelete]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var result = await _repo.DeleteClient(id);
            return Ok(new { deleted = result });
        }
        // Search Clients 
        [Route("GetAllClients")]
        [HttpGet]
        public async Task<IActionResult> GetAllClients()
        {
            try
            {
                var dt= await _repo.GetAllClients();
                return Ok(dt);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
           
        }
        // Get negotiation of client 
        [Route("GetClientNegotiations")]
        [HttpPost]
        public async Task<IActionResult> GetClientNegotiations(int clientid)
        {
            try
            {
                var dt = await _repo.GetClientNegotiations(clientid);
                return Ok(dt);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        // Get first client 
        [Route("GetFirstClient")]
        [HttpGet]
        public async Task<IActionResult> GetFirstClient()
        {
            try
            {
                var (client, negotiations, isnull) = await _repo.GetFirstClient();

                if (isnull)
                {
                    return NotFound(new { message = "No clients found", isnull = true });
                }
                return Ok(new
                {
                    clientData = client, 
                    negotiations = negotiations,
                    isnull = isnull
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        //Get last Client
        [Route("GetLastClient")]
        [HttpGet]
        public async Task<IActionResult> GetLastClient()
        {

            try
            {
                var (client, negotiations_l, isnull) = await _repo.GetLastClient();

                if (isnull)
                {
                    return NotFound(new { message = "No clients found", isnull = true });
                }
                return Ok(new
                {
                    dt = client,
                    negotiations_l = negotiations_l,
                    isnull = isnull
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
           
        }
        // Get Next Client 
        [Route("GetNextClient")]
        [HttpPost]
        public async Task<IActionResult> GetNextClient(int id)
        {
            try
            {
                var (client, negotiations, isLast, isEmpty) = await _repo.GetNextClient(id);
                return Ok(new { client, negotiations = negotiations, isLast, isEmpty });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }

        }
        // Get Previous Client 
        [Route("GetPreviousClient")]
        [HttpPost]
        public async Task<IActionResult> GetPreviousClient(int id)
        {
            try
            {
                var (dt, negotiations, isFirst, isEmpty) = await _repo.GetPreviousClient(id);
                return Ok(new
                {
                    dt,
                    negotiations,
                    isFirst,
                    isEmpty
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }


        }
       

    }
}
