using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;
using WebApp1.EF;
using WebApp1.Interfaces;
using WebApp1.Models;


namespace WebApp1.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly string _connectionString;
        private readonly DataContext _context;
        public ClientRepository(IConfiguration configuration,DataContext context)
        {
            _connectionString = configuration.GetConnectionString("connT");
            _context= context;
        }

        public async Task<DataTable> GetAllProjects()
        {
            DataTable dt=new DataTable();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "select ProjectCode,ProjectName from Projects";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    await Task.Run(() => da.Fill(dt));
                }
            }
            return dt;

        }

        public async Task<DataTable> GetUnitsByProject(int projectid)
        {
            DataTable dt = new DataTable();
            string query = @"select * from vw_Project_Available_Units 
                            where ProjectCode=@ProjectCode AND ReservedStatus=0";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@ProjectCode", projectid);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    await Task.Run(() => da.Fill(dt));
                }
                await conn.CloseAsync();
            }
            return dt;
        }

        public async Task<DataTable> GetUnitPrice(int unitid)
        {
            DataTable dt = new DataTable();
            string query = @"select * from Units where UnitID =@UnitID";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@UnitID", unitid);
                    SqlDataAdapter da =new SqlDataAdapter(cmd);
                    await Task.Run(() => da.Fill(dt));
                }
                await conn.CloseAsync();
            } 
            return dt;
        }

        public async Task<(int id, bool saved, bool updated)> UpsertClient(Client cl)
        {
            bool updated = false;
            bool saved = false;
            int id = Convert.ToInt32(cl.ClientID);
           
            using(SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlTransaction transaction =  conn.BeginTransaction())
                {
                    try
                    {
                        if (id == 0)
                        {

                            string sqlin = @"insert into Clients (ClientName,PhoneNumber, ClientStatus,Notes) 
                                           values(@ClientName,@PhoneNumber,@ClientStatus,@Notes)select SCOPE_IDENTITY()";
                            using (SqlCommand cmd = new SqlCommand(sqlin, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ClientName", cl.ClientName);
                                cmd.Parameters.AddWithValue("@PhoneNumber", cl.PhoneNumber);
                                cmd.Parameters.AddWithValue("@ClientStatus", cl.ClientStatus);
                                cmd.Parameters.AddWithValue("@Notes", cl.Notes);
                                id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                                saved = true;
                            }
                        }
                        else
                        {
                            string sqlupdate = @"update Clients set ClientName=@ClientName,PhoneNumber=@PhoneNumber,
                                               ClientStatus=@ClientStatus,Notes=@Notes where ClientID=@ClientID";
                            using (SqlCommand cmd = new SqlCommand(sqlupdate, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ClientName", cl.ClientName);
                                cmd.Parameters.AddWithValue("@PhoneNumber", cl.PhoneNumber);
                                cmd.Parameters.AddWithValue("@ClientStatus", cl.ClientStatus);
                                cmd.Parameters.AddWithValue("@Notes", cl.Notes);
                                cmd.Parameters.AddWithValue("@ClientID", id);
                                await cmd.ExecuteNonQueryAsync();
                                updated = true;
                            }
                        }
                        if (cl.negotiations.Count > 0)
                        {
                            string sqld = @"delete Negotiations where ClientID=@ClientID";
                            using (SqlCommand cmd = new SqlCommand(sqld, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ClientID", id);
                                await cmd.ExecuteNonQueryAsync();
                            }
                            string insertDetails = @"insert into Negotiations 
                                                    (serialCode,ClientID,ClientName,ProjectCode,ProjectName,
                                                    UnitID,unitName,OriginalPrice,NegotiationPrice, DiscountAmount
                                                   ,NegotiationStatus,NegotiationDate,checkedByAdmin,Requester,Reserved)
                                                    values(@serialCode,@ClientID,@ClientName,@ProjectCode,@ProjectName,@UnitID
                                                   ,@unitName,@OriginalPrice,@NegotiationPrice,@DiscountAmount
                                                   ,@NegotiationStatus,@NegotiationDate,@checkedByAdmin,@Requester,@Reserved)";
                            using (SqlCommand cmd = new SqlCommand(insertDetails, conn, transaction))
                            {
                                foreach (var neg in cl.negotiations)
                                {
                                    cmd.Parameters.Clear();
                                    cmd.Parameters.AddWithValue("@serialCode", neg.serialCode);
                                    cmd.Parameters.AddWithValue("@ClientID", id);
                                    cmd.Parameters.AddWithValue("@ClientName", cl.ClientName);
                                    cmd.Parameters.AddWithValue("@ProjectCode", neg.ProjectCode);
                                    cmd.Parameters.AddWithValue("@ProjectName", neg.ProjectName);
                                    cmd.Parameters.AddWithValue("@UnitID", neg.UnitID);
                                    cmd.Parameters.AddWithValue("@unitName", neg.unitName);
                                    cmd.Parameters.AddWithValue("@OriginalPrice", neg.OriginalPrice);
                                    cmd.Parameters.AddWithValue("@NegotiationPrice", neg.NegotiationPrice);
                                    cmd.Parameters.AddWithValue("@DiscountAmount", neg.DiscountAmount);
                                    cmd.Parameters.AddWithValue("@NegotiationStatus", neg.NegotiationStatus);
                                    cmd.Parameters.AddWithValue("@NegotiationDate", neg.NegotiationDate);
                                    cmd.Parameters.AddWithValue("@checkedByAdmin", neg.checkedByAdmin);
                                    cmd.Parameters.AddWithValue("@Requester", neg.Requester);
                                    cmd.Parameters.AddWithValue("@Reserved", neg.Reserved);
                                    await cmd.ExecuteScalarAsync();
                                }

                            }

                        }
                        await transaction.CommitAsync();
                        return (id, saved, updated);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw new Exception("Database Error: " + ex.Message);
                    }
                }
            }
        }

        public async Task<bool> DeleteClient(int id)
        {
            using(SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        if (id > 0)
                        {

                            string deleteNegotiations = "delete Negotiations where ClientID=@ClientID";
                            using (SqlCommand cmd = new SqlCommand(deleteNegotiations, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ClientID", id);
                                await cmd.ExecuteNonQueryAsync();
                            }
                            string deleteClient = "delete Clients where ClientID=@ClientID";
                            using (SqlCommand cmd = new SqlCommand(deleteClient, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ClientID", id);
                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                        await transaction.CommitAsync();
                        return true ;
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                    
                }
            }
            
        }

        public async Task<DataTable> GetAllClients()
        {
            DataTable dt = new DataTable();
            string query = "select * from Clients";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        await Task.Run(() => da.Fill(dt));
                    }
                }
            }
            return dt;
        }

        public async Task<DataTable> GetClientNegotiations(int clientid)
        {
            DataTable dt = new DataTable();
            string query = "select * from Negotiations where ClientID=@ClientID";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ClientID", clientid);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        await Task.Run(() => da.Fill(dt));
                    }
                }
            }
            return dt;
        }

        public async Task<(DataTable dt, List<Negotiation> negotiations, bool isnull)> GetFirstClient()
        {
            DataTable dt = new DataTable();
            int firstClientId = 0;
            List<Negotiation> negotiations = new List<Negotiation>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sqlgetFirst = "SELECT TOP(1) * FROM Clients ORDER BY ClientID ASC";
                using (SqlCommand cmd = new SqlCommand(sqlgetFirst, conn))
                {
                    await conn.OpenAsync();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                       await Task.Run(() => da.Fill(dt));
                    }
                }
            }
            if (dt.Rows.Count > 0)
            {
                firstClientId = Convert.ToInt32(dt.Rows[0]["ClientID"]);
                negotiations = await _context.Negotiations
                                             .Where(n => n.ClientID == firstClientId)
                                             .ToListAsync();
                return (dt, negotiations, false);
            }

            return (dt, negotiations, true);
        }

        public async Task<(DataTable dt, List<Negotiation> negotiations, bool isnull)> GetLastClient()
        {
            DataTable dt = new DataTable();
            int last_clientid = 0;
            List<Negotiation> negotiations_l = new List<Negotiation>();
             bool isnull = false;
            using(SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string sqlgetLast = "select top(1)* from Clients order by ClientID DESC";
                using (SqlCommand cmd = new SqlCommand(sqlgetLast, conn))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        await Task.Run(() => da.Fill(dt));
                    }
                }
            }
           
            if (dt.Rows.Count > 0)
            {
                last_clientid = Convert.ToInt32(dt.Rows[0]["ClientID"]);
                isnull = false;
            }
            else
            {
                isnull = true;
            }
            negotiations_l = await _context.Negotiations.Where(n => n.ClientID == last_clientid).ToListAsync();
            return (dt,negotiations_l,isnull);
        }

        public async Task<(DataTable dt, List<Negotiation> negotiations, bool isLast, bool isEmpty)> GetNextClient(int currentId)
        {
            DataTable dt = new DataTable();
            int nextId = 0;
            bool isLast = false;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string sqlCheck = "SELECT COUNT(1) FROM Clients";
                using (SqlCommand cmdCheck = new SqlCommand(sqlCheck, conn))
                {
                    int count = (int)await cmdCheck.ExecuteScalarAsync();
                    if (count == 0) return (dt, new List<Negotiation>(), false, true);
                }
                string sqlNext = "SELECT TOP(1) * FROM Clients WHERE ClientID > @ID ORDER BY ClientID ASC";
                using (SqlCommand cmd = new SqlCommand(sqlNext, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", currentId);
                    using (var reader = await cmd.ExecuteReaderAsync()) { dt.Load(reader); }
                }

                if (dt.Rows.Count == 0)
                {
                    isLast = true;
                    string sqlCurrent = "SELECT * FROM Clients WHERE ClientID = @ID";
                    using (SqlCommand cmd = new SqlCommand(sqlCurrent, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", currentId);
                        using (var reader = await cmd.ExecuteReaderAsync()) { dt.Load(reader); }
                    }
                }

                if (dt.Rows.Count > 0) nextId = Convert.ToInt32(dt.Rows[0]["ClientID"]);
            }
            var negotiations = await _context.Negotiations.Where(n => n.ClientID == nextId).ToListAsync();
            return (dt, negotiations, isLast, false);
        }

        public async Task<(DataTable dt, List<Negotiation> negotiations, bool isFirst, bool isEmpty)> GetPreviousClient(int currentId)
        {
            DataTable dt = new DataTable();
            int previousId = 0;
            bool isFirst = false;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string sqlCheck = "SELECT COUNT(1) FROM Clients";
                using (SqlCommand cmdCheck = new SqlCommand(sqlCheck, conn))
                {
                    int count = (int)await cmdCheck.ExecuteScalarAsync();
                    if (count == 0) return (dt, new List<Negotiation>(), false, true);
                }

                string sqlPrev = "SELECT TOP(1) * FROM Clients WHERE ClientID < @ID ORDER BY ClientID DESC";
                using (SqlCommand cmd = new SqlCommand(sqlPrev, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", currentId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        await Task.Run(() => da.Fill(dt));
                    }
                }

                if (dt.Rows.Count == 0)
                {
                    isFirst = true;
                    string sqlCurrent = "SELECT * FROM Clients WHERE ClientID = @ID";
                    using (SqlCommand cmd = new SqlCommand(sqlCurrent, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", currentId);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                          await Task.Run(() => da.Fill(dt));
                        }
                            
                    }
                }

                if (dt.Rows.Count > 0) previousId = Convert.ToInt32(dt.Rows[0]["ClientID"]);
            }

            var negotiations = await _context.Negotiations
                                             .Where(n => n.ClientID == previousId)
                                             .ToListAsync();

            return (dt, negotiations, isFirst, false);
        }



    }
}
