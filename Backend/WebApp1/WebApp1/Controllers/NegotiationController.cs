using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
using WebApp1.Core.Interfaces;
using WebApp1.Core.Models;
using WebApp1.EF;
namespace WebApp1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NegotiationController : ControllerBase
    {
        private readonly INegotiationRepository _repo;
        private readonly IWebHostEnvironment _env;
        public NegotiationController(INegotiationRepository repo,IWebHostEnvironment env)
        {
            _env = env;
            _repo = repo;
        }
        [Route("GetPendingNegotiationsCount")]
        [HttpGet]
        public async Task<IActionResult> GetPendingNegotiationsCount()
        {
            var count=await _repo.GetPendingNegotiationsCount();
            return Ok(new { count = count });
        }
        // Get Count Number Of Unchecked Requests By Admin 
        [Route("GetPendingNegotiations")]
        [HttpGet]
        public async Task<IActionResult> GetPendingNegotiations()
        {
            var dt= await _repo.GetPendingNegotiations();
            return Ok(new { dt = dt });
        }
        //Approve Or Reject Negotiation Request By Admin 
        [Route("ProcessNegotiationReview")]
        [HttpPost]
        public async Task<IActionResult> ProcessNegotiationReview([FromBody] Rejected_negotiations_phase phase)
        {
            var result = await _repo.ProcessNegotiationReview(phase);
            return Ok(new {saved=result});
        }
        // Re_Approve Or Re_Reject Negotiation Request By Admin 
        [Route("UpdateNegotiationReview")]
        [HttpPost]
        public async Task<IActionResult> UpdateNegotiationReview([FromBody] Rejected_negotiations_phase phase)
        {
            var (Re_Approved, Re_Rejected) =await _repo.UpdateNegotiationReview(phase);
            return Ok(new { Re_Approved = Re_Approved, Re_Rejected = Re_Rejected });
        }
        // Get Rejected Requests And Their Count Number
        [Route("GetRejectedNegotiations")]
        [HttpGet]
        public async Task<IActionResult> GetRejectedNegotiations()
        {
            var (count ,negotiations) = await _repo.GetRejectedNegotiations();
            return Ok(new { count = count, dt = negotiations });
        }
        // Get Accepted Requests And Their Count Number 
        [Route("GetApprovedNegotiations")]
        [HttpGet]
        public async Task<IActionResult> GetApprovedNegotiations()
        {
            var (count, negotiations) = await _repo.GetApprovedNegotiations();
            return Ok(new { count_a = count, dt = negotiations });

        }
       
       

    }
}
