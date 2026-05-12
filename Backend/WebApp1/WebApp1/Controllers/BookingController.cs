
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using WebApp1.Core.DTO;
using WebApp1.Core.Interfaces;
using WebApp1.Core.Models;
using WebApp1.EF;

namespace WebApp1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _repo;
        public BookingController(IBookingRepository repo)
        {
            _repo = repo;
        }

        // Get Client Data Automatically to Complete Booking
        [Route("GetBookingClientData")]
        [HttpPost]
        public async Task<IActionResult> GetBookingClientData([FromBody] BookingClient cl)
        {
            bool isExist = false;
            var negotiations=await _repo.GetBookingClientData(cl);
            return Ok(new { isExist ,dt=negotiations });

        }
        //Save NationalID Cards - Check Image - Installment Check Image Images 
        [Route("UploadImages")]
        [HttpPost]
        public async Task<IActionResult> UploadImages(IFormFile file,string folderName)
        {
            try
            {
                var filename = await _repo.UploadBookingImages(file, folderName);
                return Ok(filename);
            }
            catch (Exception ex )
            {

                return BadRequest(ex.Message);
            }
            
        }

        [Route("GenerateInstallments")]
        [HttpPost]
        public IActionResult GenerateInstallments([FromBody] InstallmentDetails request)
        {
            try
            {
                if (request == null || request.InstallmentYears <= 0)
                    return BadRequest("بيانات غير صالحة");

                var installments = _repo.GenerateInstallments(request);
                return Ok(installments);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        //Save Client Checks Images
        [Route("ConfirmFullBooking")]
        [HttpPost]
        public async Task<IActionResult> ConfirmFullBooking([FromBody] FullBookingRequest request)
        {

            if (request == null || request.ClientExtraDetails == null || request.UnitBooking == null)
                return BadRequest(new { message = "بيانات ناقصة" });
            var (id,savedBooking, updatedBooking)=await _repo.ConfirmFullBooking(request);
            return Ok(new {id=id,savedBooking=savedBooking, updatedBooking=updatedBooking});

        }


        ////Updated Negotiation Requests to Reserved 
        [Route("ConfirmReservation")]
        [HttpPost]
        public async Task<IActionResult> ConfirmReservation([FromBody] NegotiationViewModel neg)
        {
            var result = await _repo.ConfirmReservation(neg);
            return Ok(result);

        }
        ////Get Reserved Data
        [Route("GetAllReservedClients")]
        [HttpGet]
        public async Task<IActionResult> GetAllReservedClients()
        {
            var data = await _repo.GetAllReservedClients();
            return Ok(data);
        }
        ////Get Rserved Clients with Installments to enable editing data 
        [Route("GetReservedClientById")]
        [HttpPost]
        public async Task<IActionResult> GetReservedClientById(int id)
        {
           var dto=await _repo.GetReservedClientById(id);
            return Ok(dto);
        }

        [Route("DeleteBookingData")]
        [HttpPost]
        public async Task<IActionResult> DeleteBookingData([FromBody] UnitBooking client)
        {
            var result=await _repo.DeleteBookingData(client);
            return Ok(result);
        }

        [Route("SearchBookings")]
        [HttpPost]
        public async Task<IActionResult> SearchBookings([FromBody] Search term)
        {
            var results = await _repo.SearchGeneric("vw_Booked_Clients", term);
            return Ok(results);
        }

        [Route("SearchClients")]
        [HttpPost]
        public async Task<IActionResult> SearchClients([FromBody] Search term)
        {
            var results = await _repo.SearchGeneric("Clients", term);
            return Ok(results);
        }

    }
}
