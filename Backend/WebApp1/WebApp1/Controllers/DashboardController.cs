using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using WebApp1.EF;
using System.Globalization;
using WebApp1.Core.Interfaces;

namespace WebApp1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardRepository _repo;
        public DashboardController(IDashboardRepository repo)
        {
            _repo=repo;
        }
        [Route("GetProjectsUnitsStats")]
        [HttpGet]
        public async Task<IActionResult> GetProjectsUnitsStats() => Ok(await _repo.GetProjectsUnitsStats());

        [Route("GetDailyStats")]
        [HttpGet]
        public async Task<IActionResult> GetDailyStats() => Ok(await _repo.GetMonthlyBookingStats());

        [Route("GetProjectsCount")]
        [HttpGet]
        public async Task<IActionResult> GetProjectsCount() =>
        Ok(await _repo.GetCount("SELECT COUNT(*) FROM Projects"));

        [Route("GetClientsCount")]
        [HttpGet]
        public async Task<IActionResult> GetClientsCount() =>
        Ok(await _repo.GetCount("SELECT COUNT(*) FROM Clients"));

        [Route("GetNegotiationsCount")]
        [HttpGet]
        public async Task<IActionResult> GetNegotiationsCount() =>
        Ok(await _repo.GetCount("SELECT COUNT(*) FROM Negotiations WHERE checkedByAdmin=0"));

        [Route("GetReservedUnits")]
        [HttpGet]
        public async Task<IActionResult> GetReservedUnits() =>
        Ok(await _repo.GetCount("SELECT COUNT(*) FROM Units WHERE ReservedStatus=1"));

        [Route("SetAvailableUnits")]
        [HttpGet]
        public async Task<IActionResult> SetAvailableUnits() =>
        Ok(await _repo.GetCount("SELECT COUNT(*) FROM Units WHERE ReservedStatus=0"));
    
    }
}
