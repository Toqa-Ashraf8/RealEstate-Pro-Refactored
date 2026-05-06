using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using WebApp1.EF;
using WebApp1.Models;
using WebApp1.ViewModels;

namespace WebApp1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;
        SqlConnection conn;
        public BookingController(DataContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
            conn = new SqlConnection(_context.Database.GetConnectionString());
        }

        // Get Client Data Automatically to Complete Booking
        [Route("GetBookingClientData")]
        [HttpPost]
        public JsonResult GetBookingClientData([FromBody] BookingClient cl)
        {
            int id = Convert.ToInt32(cl.ClientID);
            DataTable dt = new DataTable();
            DataTable clientData = new DataTable();
            bool isExist = false;
            DataTable clientDetails = new DataTable();
            if (id > 0){

                try
                {
                    string sqlget = @"select * from Negotiations 
                                    where ClientID=@ClientID AND 
                                    ProjectCode=@ProjectCode AND
                                    UnitID=@UnitID";
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlget, conn))
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@ClientID", id);
                        cmd.Parameters.AddWithValue("@ProjectCode", cl.ProjectCode);
                        cmd.Parameters.AddWithValue("@UnitID", cl.UnitID);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
                   
                    string get = @"select * from ClientExtraDetails where ClientID=@ClientID";
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    using (SqlCommand cmd = new SqlCommand(get, conn))
                    {
                        DataTable clientInformationDT = new DataTable();
                        cmd.Parameters.AddWithValue("@ClientID", cl.ClientID);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(clientInformationDT);
                        if (clientInformationDT.Rows.Count > 0)
                        {
                            clientData = clientInformationDT;
                            isExist = true;
                        }
                        else
                        {
                            isExist = false;
                        }
                    }
                }
                catch  { return new JsonResult(new { message = "حدث خطأ" });}
               
                  
                finally {if (conn.State == ConnectionState.Open) conn.Close(); }

            }
            var data = new { dt = dt, clientData = clientData , isExist= isExist };
            return new JsonResult(data);

        }
        //Save NationalID Cards Images 
        [Route("NationalIdUploadRequest")]
        [HttpPost]
        public JsonResult NationalIdUploadRequest ([FromForm] upload_NationalID_Images cardimg)
        {
            var postedFile = cardimg.formFile;
            string fileName = postedFile.FileName;
            var physicalPath = _env.ContentRootPath + "/NationalIDCard_Images/" + fileName;
            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                postedFile.CopyTo(stream);
            }
            return new JsonResult(fileName);
        }
        //Save Client Checks Images
        [Route("CheckUploadRequest")]
        [HttpPost]
        public JsonResult CheckUploadRequest([FromForm] upload_Checks_Images checkimg)
        {
            var postedFile = checkimg.file_c;
            string fileName = postedFile.FileName;
            var physicalPath = _env.ContentRootPath + "/Checks_Images/" + fileName;
            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                postedFile.CopyTo(stream);
            }
            return new JsonResult(fileName);
        }
        //Save Installment Checks Images 
        [Route("InstallmentCheckUploadRequest")]
        [HttpPost]
        public JsonResult InstallmentCheckUploadRequest([FromForm] upload_Installment_Checks check)
        {
            var postedFile = check.checkfile;
            string fileName=postedFile.FileName;
            var physicalPath = _env.ContentRootPath + "/InstallmentChecks_Images/" + fileName;
            using(var stream=new FileStream(physicalPath, FileMode.Create))
            {
                postedFile.CopyTo(stream);
            }
            return new JsonResult(fileName);

        }
       
        [Route("ConfirmFullBooking")]
        [HttpPost]
        public JsonResult ConfirmFullBooking([FromBody] FullBookingRequest request)
        {
            bool savedBooking = false;
            bool updatedBooking = false;
            int booking_id = Convert.ToInt32(request.UnitBooking.BookingID);
            if (request == null || request.ClientExtraDetails == null || request.UnitBooking == null)
                return new JsonResult(new { success = false, message = "بيانات ناقصة" });

            if (conn.State == ConnectionState.Closed) conn.Open();
            SqlTransaction transaction = conn.BeginTransaction();

            try
            {
                if (booking_id == 0)
                {
                string checkSql = "SELECT COUNT(*) FROM ClientExtraDetails WHERE ClientID=@ClientID";
                int exists = 0;
                using (SqlCommand cmdCheck = new SqlCommand(checkSql, conn, transaction))
                {
                    cmdCheck.Parameters.AddWithValue("@ClientID", request.ClientExtraDetails.ClientID);
                    exists = (int)cmdCheck.ExecuteScalar();
                }

                    if (exists == 0)
                    {
                        string sqlInsertClient = @"INSERT INTO ClientExtraDetails (NationalID, NationalIdImagePath, 
                                                   SecondaryPhone, Address, Job, ClientID, ClientName) 
                                                   VALUES (@NationalID, @NationalIdImagePath, @SecondaryPhone, 
                                                   @Address, @Job, @ClientID, @ClientName)";
                        using (SqlCommand cmd = new SqlCommand(sqlInsertClient, conn, transaction))
                        {
                            FillClientParams(cmd, request.ClientExtraDetails);
                            cmd.ExecuteNonQuery();
                            savedBooking = false;
                        }
                    }
                    else
                    {
                        string sqUpdateClient = @"Update ClientExtraDetails set NationalID=@NationalID, 
                                                  NationalIdImagePath=@NationalIdImagePath, 
                                                  SecondaryPhone=@SecondaryPhone,Address=@Address, 
                                                  Job=@Job,ClientID=@ClientID,ClientName=@ClientName";
                        using (SqlCommand cmd = new SqlCommand(sqUpdateClient, conn, transaction))
                        {
                            FillClientParams(cmd, request.ClientExtraDetails);
                            cmd.ExecuteNonQuery();
                            savedBooking = false;
                        }
                    }

                    string sqlBooking = @"INSERT INTO UnitBooking (ReservationAmount, PaymentMethod, CheckImagePath, DownPayment, 
                     FirstInstallmentDate, InstallmentYears, BookingDate, ClientID, ProjectCode, UnitID, Reserved) 
                     VALUES (@ReservationAmount, @PaymentMethod, @CheckImagePath, @DownPayment, @FirstInstallmentDate, @InstallmentYears, 
                     @BookingDate, @ClientID, @ProjectCode, @UnitID, @Reserved);
                     SELECT SCOPE_IDENTITY()";

                    using (SqlCommand cmd = new SqlCommand(sqlBooking, conn, transaction))
                    {
                        FillBookingParams(cmd, request.UnitBooking);
                        booking_id = Convert.ToInt32(cmd.ExecuteScalar());
                        savedBooking = true;
                    }
                    if (savedBooking == true)
                    {
                        string sqlUnit = "UPDATE Units SET ReservedStatus=1 WHERE ProjectCode=@ProjectCode AND UnitID=@UnitID";
                        using (SqlCommand cmd = new SqlCommand(sqlUnit, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@ProjectCode", request.UnitBooking.ProjectCode);
                            cmd.Parameters.AddWithValue("@UnitID", request.UnitBooking.UnitID);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        return new JsonResult(new { message = "حدث خطأ أثناء تحديث حالة الوحدة " });
                    }
                }
                else
                {
                    try
                    {
                        string sqUpdateClient = @"Update ClientExtraDetails set NationalID=@NationalID, 
                                                  NationalIdImagePath=@NationalIdImagePath, 
                                                  SecondaryPhone=@SecondaryPhone,Address=@Address, 
                                                  Job=@Job,ClientID=@ClientID,ClientName=@ClientName";
                        using (SqlCommand cmd = new SqlCommand(sqUpdateClient, conn, transaction))
                        {
                            FillClientParams(cmd, request.ClientExtraDetails);
                            cmd.ExecuteNonQuery();
                            updatedBooking = true;
                        }

                        string sqlBooking = @" Update UnitBooking set ReservationAmount=@ReservationAmount, 
                                           PaymentMethod=@PaymentMethod, CheckImagePath=@CheckImagePath, 
                                           DownPayment=@DownPayment, FirstInstallmentDate=@FirstInstallmentDate,
                                           InstallmentYears=@InstallmentYears, BookingDate=@BookingDate, 
                                           ClientID=@ClientID,ProjectCode=@ProjectCode,UnitID=@UnitID, Reserved=@Reserved 
                                           where BookingID=@BookingID";

                        using (SqlCommand cmd = new SqlCommand(sqlBooking, conn, transaction))
                        {
                            FillBookingParams(cmd, request.UnitBooking);
                            cmd.ExecuteNonQuery();
                            updatedBooking = true;
                        }
                    }
                    catch {  return new JsonResult(new { message = "حدث خطأ أثناء التحديث" });}
                  
                }
     
                if (request.UnitBooking.installments != null && request.UnitBooking.installments.Count > 0)
                {
                    try
                    {
                        string sqldelete = @"delete Installments where BookingID=@BID";
                        if (conn.State == ConnectionState.Closed) conn.Open();
                        using(SqlCommand cmd=new SqlCommand(sqldelete, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@BID", booking_id);
                            cmd.ExecuteNonQuery();
                        }
                       
                    }
                    catch { return new JsonResult(new { message = "حدث خطأ أثناء مسح الأقساط" });} 
                    
                    string sqlInstallment = @"INSERT INTO Installments (InstallmentNumber, DueDate, 
                                            Months, MonthlyAmount, Paid, PaymentType, CheckImage, BookingID) 
                                            VALUES (@Num, @Date, @Months, @Amt, @Paid, @Type, @Img, @BID)";

                    foreach (var item in request.UnitBooking.installments)
                    {
                        using (SqlCommand cmd = new SqlCommand(sqlInstallment, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Num", item.InstallmentNumber);
                            cmd.Parameters.AddWithValue("@Date", item.DueDate);
                            cmd.Parameters.AddWithValue("@Months", item.Months);
                            cmd.Parameters.AddWithValue("@Amt", item.MonthlyAmount);
                            cmd.Parameters.AddWithValue("@Paid", item.Paid);
                            cmd.Parameters.AddWithValue("@Type", item.PaymentType);
                            cmd.Parameters.AddWithValue("@Img", item.CheckImage);
                            cmd.Parameters.AddWithValue("@BID", booking_id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                transaction.Commit();
                return new JsonResult(new { 
                    success = true, 
                    bookingId = booking_id,
                    savedBooking=savedBooking,
                    updatedBooking=updatedBooking});
            }
            catch (Exception ex)
            {
                transaction.Rollback(); 
                return new JsonResult(new { success = false, message = "حدث خطأ: " + ex.Message });
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }

       
        private void FillClientParams(SqlCommand cmd, ClientExtraDetails cl)
        {
            cmd.Parameters.AddWithValue("@NationalID", (object)cl.NationalID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NationalIdImagePath", (object)cl.NationalIdImagePath ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SecondaryPhone", (object)cl.SecondaryPhone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Address", (object)cl.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Job", (object)cl.Job ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ClientID", (object)cl.ClientID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ClientName", (object)cl.ClientName ?? DBNull.Value);
        }
        private void FillBookingParams(SqlCommand cmd, UnitBooking ub)
        {
            cmd.Parameters.AddWithValue("@BookingID", (object)ub.BookingID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ReservationAmount", (object)ub.ReservationAmount ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PaymentMethod", (object)ub.PaymentMethod ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CheckImagePath", (object)ub.CheckImagePath ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DownPayment", (object)ub.DownPayment ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FirstInstallmentDate", (object)ub.FirstInstallmentDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@InstallmentYears", (object)ub.InstallmentYears ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BookingDate", (object)ub.BookingDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ClientID", (object)ub.ClientID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ProjectCode", (object)ub.ProjectCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UnitID", (object)ub.UnitID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Reserved", (object)ub.Reserved ?? DBNull.Value);
        }
        //Generate installment Table 
        [Route("GenerateInstallments")]
        [HttpPost]
        public JsonResult GenerateInstallments([FromBody]InstallmentDetails request)
        {
            int initial_payment_status = 0;
            if (request == null || request.InstallmentYears <= 0)
                return new JsonResult("بيانات غير صالحة");
            var installments = new List<InstallmentViewModel>();
            //الحساب المتبقي من غير المقدم
            int remainingAmount = request.TotalAmount - request.DownPayment;
            //حساب عدد الشهور من عدد السنين
            int TotalMonths = request.InstallmentYears * 12;
            //قيمة القسط الشهري 
            decimal monthlyPrice = remainingAmount / TotalMonths;

            for (int i = 1; i <= TotalMonths; i++)
            {
                installments.Add(new InstallmentViewModel
                {
                    InstallmentNumber = i,
                    // إضافة شهر في كل لفة بناءً على تاريخ أول قسط
                    DueDate = request.FirstInstallmentDate.AddMonths(i - 1),
                    Months = TotalMonths,
                    MonthlyAmount = monthlyPrice,
                    Paid = initial_payment_status

                });

            }
            return new JsonResult(installments);

        }
        //Updated Negotiation Requests to Reserved 
        [Route("ConfirmReservation")]
        [HttpPost]
        public JsonResult ConfirmReservation([FromBody] NegotiationViewModel neg)
        {
            bool saved = false;
            string sqlup = @"Update Negotiations set Reserved=1 
                             where ClientID=@ClientID AND 
                             ProjectCode=@ProjectCode AND
                             UnitID=@UnitID";
            using (SqlCommand cmd = new SqlCommand(sqlup, conn))
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ClientID", neg.ClientID);
                cmd.Parameters.AddWithValue("@ProjectCode", neg.ProjectCode);
                cmd.Parameters.AddWithValue("@UnitID", neg.UnitID);
                cmd.ExecuteNonQuery();
                if (conn.State == ConnectionState.Open) conn.Close();
                saved = true;
            }
            return new JsonResult(saved);
        }
        //Get Reserved Data
        [Route("GetAllReservedClients")]
        [HttpGet]
        public JsonResult GetAllReservedClients()
        {
            DataTable dt = new DataTable();
            string sqls = "Select * from reserved_clients_details where Reserved=1";
            using (SqlCommand cmd = new SqlCommand(sqls, conn))
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                cmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            if (conn.State == ConnectionState.Open) conn.Close();
            return new JsonResult(dt);
        }
        //Get Rserved Clients with Installments to enable editing data 
        [Route("GetReservedClientById")]
        [HttpPost]
        public JsonResult GetReservedClientById(int id)
        {
            var clientdata = new List<BookingClient>();
            var reservationData = new List<InstallmentData>();
            DataTable clientdt = new DataTable();
            DataTable installmentdt = new DataTable();
            string sqls = "Select * from reserved_clients_details where BookingID=@BookingID"; 
            if (conn.State == ConnectionState.Closed) conn.Open();
            using (SqlCommand cmd = new SqlCommand(sqls, conn))
            {
               
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@BookingID", id);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(clientdt);
                if (clientdt.Rows.Count > 0)
                {
                    foreach (DataRow row in clientdt.Rows)
                    {
                        clientdata.Add(new BookingClient
                        {
                            ClientID = Convert.ToInt32(row["ClientID"]),
                            ClientName = row["ClientName"].ToString(),
                            ProjectCode = Convert.ToInt32(row["ProjectCode"]),
                            ProjectName = row["ProjectName"].ToString(),
                            UnitID = Convert.ToInt32(row["UnitID"]),
                            unitName = row["unitName"].ToString(),
                           

                        });
                        reservationData.Add(new InstallmentData
                        {
                            BookingID = Convert.ToInt32(row["BookingID"]),
                            ReservationAmount = Convert.ToInt32(row["ReservationAmount"]),
                            PaymentMethod = row["PaymentMethod"].ToString(),
                            DownPayment = Convert.ToInt32(row["DownPayment"]),
                            FirstInstallmentDate = Convert.ToDateTime(row["FirstInstallmentDate"]),
                            InstallmentYears = Convert.ToInt32(row["InstallmentYears"]),
                            CheckImagePath= row["CheckImagePath"].ToString(),
                            BookingDate = Convert.ToDateTime(row["BookingDate"]),
                        });
                    }

                }
             }
            string sqlg = "Select * from reserved_clients_installments where BookingID=@BookingID";
            using (SqlCommand cmd = new SqlCommand(sqlg, conn))
            {
         
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@BookingID", id);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(installmentdt);
            }
            if (conn.State == ConnectionState.Open) conn.Close();
            var data = new {  
                clientdt = clientdt, 
                installmentdt= installmentdt ,
                reservationData = reservationData,
                clientdata= clientdata
            }; 
            return new JsonResult(data);
        }

        [Route("DeleteBookingData")]
        [HttpPost]
        public JsonResult DeleteBookingData([FromBody] UnitBooking client)
        {
            bool isDeleted = false;
            try
            {
                string deleteInstallment = "delete Installments where BookingID=@BookingID";
                if (conn.State == ConnectionState.Closed) conn.Open();
                using (SqlCommand cmd = new SqlCommand(deleteInstallment, conn))
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@BookingID", client.BookingID);
                    int rows = cmd.ExecuteNonQuery();
                    isDeleted = true;

                }
                if (isDeleted)
                {
                    try
                    {
                        string deleteClient = "delete UnitBooking where BookingID=@BookingID";
                        using (SqlCommand cmd = new SqlCommand(deleteClient, conn))
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@BookingID", client.BookingID);
                            cmd.ExecuteNonQuery();
                            isDeleted = true;
                        }
                        string sqlp = "Update Units set ReservedStatus=0 where UnitID=@UnitID";
                        using (SqlCommand cmd = new SqlCommand(sqlp, conn))
                        {
                            if (conn.State == ConnectionState.Closed) conn.Open();
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@UnitID", client.UnitID);
                            cmd.ExecuteNonQuery();

                        }

                    }
                    catch (Exception)
                    {

                        return new JsonResult(new { message = "حدث خطأ أثناء تغيير الحالة " });
                    }
                   
                }

            }
            catch (Exception)
            {

                return new JsonResult(new { message = "حدث خطأ أثناء مسح الحجز" });
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }

            return new JsonResult(isDeleted);
        }

        [Route("SearchBookings")]
        [HttpPost]
        public JsonResult SearchBookings([FromBody]Search term)
        {
            DataTable dt = new DataTable();
            List<string> conditions = new List<string>();
            foreach (var field in term.Fields)
            {
                conditions.Add($"{field} LIKE @searchterm");
            }
            string whereClause = string.Join(" OR ", conditions);
            string search = @"select * from reserved_clients_details where " + whereClause;
            using (SqlCommand cmd = new SqlCommand(search, conn))
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@searchterm", "%" + term.Term + "%");
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (conn.State == ConnectionState.Open) conn.Close();
               
            } 
            if (dt.Rows.Count > 0)
            {
               
               return new JsonResult(dt);
            }
            else
            {
                return new JsonResult(new DataTable());
            }
        }

        [Route("SearchClients")]
        [HttpPost]
        public JsonResult SearchClients([FromBody] Search term)
        {
            DataTable dt = new DataTable();
            List<string> conditions = new List<string>();
            foreach (var field in term.Fields)
            {
                conditions.Add($"{field} LIKE @searchterm");
            }
            string whereClause = string.Join(" OR ", conditions);
            string search = @"select * from Clients where " + whereClause;
            using (SqlCommand cmd = new SqlCommand(search, conn))
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@searchterm", "%" + term.Term + "%");
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (conn.State == ConnectionState.Open) conn.Close();

            }
            if (dt.Rows.Count > 0)
            {

                return new JsonResult(dt);
            }
            else
            {
                return new JsonResult(new DataTable());
            }
        }

    }
}
