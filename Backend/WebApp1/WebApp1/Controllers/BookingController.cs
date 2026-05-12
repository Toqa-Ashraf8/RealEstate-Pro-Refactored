
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

        //    [Route("DeleteBookingData")]
        //    [HttpPost]
        //    public JsonResult DeleteBookingData([FromBody] UnitBooking client)
        //    {
        //        bool isDeleted = false;
        //        try
        //        {
        //            string deleteInstallment = "delete Installments where BookingID=@BookingID";
        //            if (conn.State == ConnectionState.Closed) conn.Open();
        //            using (SqlCommand cmd = new SqlCommand(deleteInstallment, conn))
        //            {
        //                cmd.Parameters.Clear();
        //                cmd.Parameters.AddWithValue("@BookingID", client.BookingID);
        //                int rows = cmd.ExecuteNonQuery();
        //                isDeleted = true;

        //            }
        //            if (isDeleted)
        //            {
        //                try
        //                {
        //                    string deleteClient = "delete UnitBooking where BookingID=@BookingID";
        //                    using (SqlCommand cmd = new SqlCommand(deleteClient, conn))
        //                    {
        //                        cmd.Parameters.Clear();
        //                        cmd.Parameters.AddWithValue("@BookingID", client.BookingID);
        //                        cmd.ExecuteNonQuery();
        //                        isDeleted = true;
        //                    }
        //                    string sqlp = "Update Units set ReservedStatus=0 where UnitID=@UnitID";
        //                    using (SqlCommand cmd = new SqlCommand(sqlp, conn))
        //                    {
        //                        if (conn.State == ConnectionState.Closed) conn.Open();
        //                        cmd.Parameters.Clear();
        //                        cmd.Parameters.AddWithValue("@UnitID", client.UnitID);
        //                        cmd.ExecuteNonQuery();

        //                    }

        //                }
        //                catch (Exception)
        //                {

        //                    return new JsonResult(new { message = "حدث خطأ أثناء تغيير الحالة " });
        //                }

        //            }

        //        }
        //        catch (Exception)
        //        {

        //            return new JsonResult(new { message = "حدث خطأ أثناء مسح الحجز" });
        //        }
        //        finally
        //        {
        //            if (conn.State == ConnectionState.Open) conn.Close();
        //        }

        //        return new JsonResult(isDeleted);
        //    }

        //    [Route("SearchBookings")]
        //    [HttpPost]
        //    public JsonResult SearchBookings([FromBody]Search term)
        //    {
        //        DataTable dt = new DataTable();
        //        List<string> conditions = new List<string>();
        //        foreach (var field in term.Fields)
        //        {
        //            conditions.Add($"{field} LIKE @searchterm");
        //        }
        //        string whereClause = string.Join(" OR ", conditions);
        //        string search = @"select * from reserved_clients_details where " + whereClause;
        //        using (SqlCommand cmd = new SqlCommand(search, conn))
        //        {
        //            if (conn.State == ConnectionState.Closed) conn.Open();
        //            cmd.Parameters.Clear();
        //            cmd.Parameters.AddWithValue("@searchterm", "%" + term.Term + "%");
        //            SqlDataAdapter da = new SqlDataAdapter(cmd);
        //            da.Fill(dt);
        //            if (conn.State == ConnectionState.Open) conn.Close();

        //        } 
        //        if (dt.Rows.Count > 0)
        //        {

        //           return new JsonResult(dt);
        //        }
        //        else
        //        {
        //            return new JsonResult(new DataTable());
        //        }
        //    }

        //    [Route("SearchClients")]
        //    [HttpPost]
        //    public JsonResult SearchClients([FromBody] Search term)
        //    {
        //        DataTable dt = new DataTable();
        //        List<string> conditions = new List<string>();
        //        foreach (var field in term.Fields)
        //        {
        //            conditions.Add($"{field} LIKE @searchterm");
        //        }
        //        string whereClause = string.Join(" OR ", conditions);
        //        string search = @"select * from Clients where " + whereClause;
        //        using (SqlCommand cmd = new SqlCommand(search, conn))
        //        {
        //            if (conn.State == ConnectionState.Closed) conn.Open();
        //            cmd.Parameters.Clear();
        //            cmd.Parameters.AddWithValue("@searchterm", "%" + term.Term + "%");
        //            SqlDataAdapter da = new SqlDataAdapter(cmd);
        //            da.Fill(dt);
        //            if (conn.State == ConnectionState.Open) conn.Close();

        //        }
        //        if (dt.Rows.Count > 0)
        //        {

        //            return new JsonResult(dt);
        //        }
        //        else
        //        {
        //            return new JsonResult(new DataTable());
        //        }
        //    }

    }
}
