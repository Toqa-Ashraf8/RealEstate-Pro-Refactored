using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using WebApp1.Core.DTO;
using WebApp1.Core.Interfaces;
using WebApp1.Core.Models;

namespace WebApp1.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly DbConnection _db;
        private readonly IWebHostEnvironment _env;
        public BookingRepository(DbConnection db, IWebHostEnvironment env)
        {
            _env = env;
            _db = db;
        }
        public async Task<IEnumerable<Negotiation>> GetBookingClientData(BookingClient cl)
        {
            string sql = @"select * from Negotiations where ClientID=@ClientID 
                         AND ProjectCode=@ProjectCode AND UnitID=@UnitID";

            var negotiations = await _db.QueryAsync<Negotiation>(sql,
               new
               {
                   ClientID = cl.ClientID,
                   ProjectCode = cl.ProjectCode,
                   UnitID = cl.UnitID
               });
            return (negotiations);
        }

        public async Task<string> UploadBookingImages(IFormFile file, string folderName)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return ("No file uploaded.");
                string fileName = file.FileName;
                var physicalPath = Path.Combine(_env.ContentRootPath, "Photos", folderName, fileName);

                var directory = Path.GetDirectoryName(physicalPath);
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return fileName;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public List<InstallmentViewModel> GenerateInstallments(InstallmentDetails request)
        {
            int initial_payment_status = 0;
            var installments = new List<InstallmentViewModel>();

            decimal remainingAmount = request.TotalAmount - request.DownPayment;
            int totalMonths = request.InstallmentYears * 12;
            decimal rawMonthlyPrice = remainingAmount / (decimal)totalMonths;
            decimal roundedMonthlyPrice = Math.Round(rawMonthlyPrice / 100) * 100;

            for (int i = 1; i <= totalMonths; i++)
            {

                decimal currentAmount = roundedMonthlyPrice;

                if (i == totalMonths)
                {

                    currentAmount = remainingAmount - (roundedMonthlyPrice * (totalMonths - 1));
                }

                installments.Add(new InstallmentViewModel
                {
                    InstallmentNumber = i,
                    DueDate = request.FirstInstallmentDate.AddMonths(i - 1),
                    Months = totalMonths,
                    MonthlyAmount = currentAmount,
                    Paid = initial_payment_status
                });
            }
            return installments;
        }

        public async Task<(int id, bool savedBooking, bool updatedBooking)> ConfirmFullBooking(FullBookingRequest request)
        {
            bool savedBooking = false;
            bool updatedBooking = false;
            int id = Convert.ToInt32(request.UnitBooking.BookingID);

            await _db.OpenAsync();
            using var transaction = await _db.BeginTransactionAsync();

            try
            {
                string checkClientSql = "SELECT COUNT(*) FROM ClientExtraDetails WHERE ClientID=@ClientID";
                int clientExist = await _db.ExecuteScalarAsync<int>(checkClientSql, new { request.ClientExtraDetails.ClientID }, transaction);

                if (clientExist == 0)
                {
                    string sqlInsertClient = @"INSERT INTO ClientExtraDetails (NationalID, NationalIdImagePath, SecondaryPhone, Address, Job, ClientID, ClientName) 
                                       VALUES (@NationalID, @NationalIdImagePath, @SecondaryPhone, @Address, @Job, @ClientID, @ClientName)";
                    await _db.ExecuteAsync(sqlInsertClient, request.ClientExtraDetails, transaction);
                }
                else
                {
                    string sqUpdateClient = @"UPDATE ClientExtraDetails SET NationalID=@NationalID, NationalIdImagePath=@NationalIdImagePath, 
                                      SecondaryPhone=@SecondaryPhone, Address=@Address, Job=@Job, ClientName=@ClientName 
                                      WHERE ClientID=@ClientID";
                    await _db.ExecuteAsync(sqUpdateClient, request.ClientExtraDetails, transaction);
                }

                if (id == 0)
                {
                    string sqlBooking = @"INSERT INTO UnitBooking (ReservationAmount, PaymentMethod, CheckImagePath, DownPayment, FirstInstallmentDate, InstallmentYears, BookingDate, ClientID, ProjectCode, UnitID, Reserved) 
                                 VALUES (@ReservationAmount, @PaymentMethod, @CheckImagePath, @DownPayment, @FirstInstallmentDate, @InstallmentYears, @BookingDate, @ClientID, @ProjectCode, @UnitID, @Reserved);
                                 SELECT CAST(SCOPE_IDENTITY() as int)";

                    id = await _db.ExecuteScalarAsync<int>(sqlBooking, request.UnitBooking, transaction);

                    string sqlUnit = "UPDATE Units SET ReservedStatus=1 WHERE ProjectCode=@ProjectCode AND UnitID=@UnitID";
                    await _db.ExecuteAsync(sqlUnit, new { request.UnitBooking.ProjectCode, request.UnitBooking.UnitID }, transaction);

                    savedBooking = true;
                }
                else
                {
                    string sqlUpdateBooking = @"UPDATE UnitBooking SET ReservationAmount=@ReservationAmount, PaymentMethod=@PaymentMethod, CheckImagePath=@CheckImagePath, 
                                        DownPayment=@DownPayment, FirstInstallmentDate=@FirstInstallmentDate, InstallmentYears=@InstallmentYears, 
                                        BookingDate=@BookingDate, ClientID=@ClientID, ProjectCode=@ProjectCode, UnitID=@UnitID, Reserved=@Reserved 
                                        WHERE BookingID=@BookingID";

                    await _db.ExecuteAsync(sqlUpdateBooking, request.UnitBooking, transaction);
                    updatedBooking = true;
                }

                await _db.ExecuteAsync("DELETE FROM Installments WHERE BookingID=@BID", new { BID = id }, transaction);

                if (request.UnitBooking.installments != null && request.UnitBooking.installments.Any())
                {
                    string sqlInstallment = @"INSERT INTO Installments (InstallmentNumber, DueDate, Months, MonthlyAmount, Paid, PaymentType, CheckImage, BookingID) 
                                      VALUES (@InstallmentNumber, @DueDate, @Months, @MonthlyAmount, @Paid, @PaymentType, @CheckImage, @BookingID)";

                    request.UnitBooking.installments.ForEach(i => i.BookingID = id);
                    await _db.ExecuteAsync(sqlInstallment, request.UnitBooking.installments, transaction);
                }

                await transaction.CommitAsync();
                return (id, savedBooking, updatedBooking);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<bool> ConfirmReservation(NegotiationViewModel neg)
        {
            string sqlupdate = @"Update Negotiations set Reserved=1 
                               where ClientID=@ClientID AND 
                               ProjectCode=@ProjectCode AND
                               UnitID=@UnitID";
            await _db.ExecuteAsync(sqlupdate,
            new
            {
                ClientID = neg.ClientID,
                ProjectCode = neg.ProjectCode,
                UnitID = neg.UnitID
            });
            return true;
        }

        public async Task<IEnumerable<BookingClient>> GetAllReservedClients()
        {
            return await _db.QueryAsync<BookingClient>("Select * from vw_Booked_Clients where Reserved=1");
        }
        public async Task<ReservedClientDto> GetReservedClientById(int bookingId)
        {
            var sql = @"SELECT * FROM vw_Booked_Clients WHERE BookingID = @id;
                        SELECT * FROM vw_ClientExtraDetails WHERE BookingID = @id;
                        SELECT * FROM UnitBooking WHERE BookingID = @id;      
                        SELECT * FROM Installments WHERE BookingID = @id;";

            using var multi = await _db.QueryMultipleAsync(sql, new { id = bookingId });
            var dto = new ReservedClientDto();

            dto.InitialClientData = await multi.ReadFirstOrDefaultAsync<Negotiation>();
            dto.ClientExDetails = await multi.ReadFirstOrDefaultAsync<ClientDetails>();
            dto.BookingDetails = await multi.ReadFirstOrDefaultAsync<UnitBooking>();
            dto.installments = (await multi.ReadAsync<Installment>()).ToList();

            return dto;

        }

        public async Task<bool> DeleteBookingData(UnitBooking client)
        {
           
            await _db.OpenAsync();
            using var transaction =  _db.BeginTransaction();
            try
            {
                var parm = new { BookingID = client.BookingID };
                await _db.ExecuteAsync("delete Installments where BookingID=@BookingID", parm, transaction);
                await _db.ExecuteAsync("delete UnitBooking where BookingID=@BookingID", parm, transaction);
                await _db.ExecuteAsync("Update Units set ReservedStatus=0 where UnitID=@UnitID", new { UnitID = client.UnitID }, transaction);
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
           
        }

        public async Task<IEnumerable<dynamic>> SearchGeneric(string tableName, Search term)
        {
        
            var conditions = term.Fields.Select(field => $"{field} LIKE @searchterm");
            string whereClause = string.Join(" OR ", conditions);
          
            string sql = $"SELECT * FROM {tableName} WHERE {whereClause}";

            return await _db.QueryAsync<dynamic>(sql, new { searchterm = $"%{term.Term}%" });
        }



    }
}
