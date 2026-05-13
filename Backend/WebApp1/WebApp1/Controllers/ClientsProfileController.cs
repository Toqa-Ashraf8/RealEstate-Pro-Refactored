
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp1.Core.DTO;
using WebApp1.Core.Interfaces;
using WebApp1.EF;
using WebApp1.Repositories;
namespace WebApp1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsProfileController : ControllerBase
    {
        private readonly IClientsProfileRepository _repo;
        
        public ClientsProfileController(IClientsProfileRepository repo)
        {
            _repo = repo;
        }
        [Route("GetAllClients")]
        [HttpGet]
        public async Task<IActionResult> GetAllClients() => Ok(await _repo.GetAllClients());
        [Route("GetClientDetails")]
        [HttpPost]
        public async Task<IActionResult> GetClientDetails([FromQuery] int clientid)
        {
            var result = await _repo.GetClientProfile(clientid);
            if (result.clientData == null) return NotFound("Client not found");
            return Ok(result);
        }


    }
}
