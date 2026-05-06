using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using WebApp1.EF;
using WebApp1.Models;
namespace WebApp1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;
        SqlConnection conn;
        public ClientsController(DataContext context, IWebHostEnvironment env)
        {
            _context = context;
             _env=env;
            conn = new SqlConnection(_context.Database.GetConnectionString());
        }
        //Get All Projects in Database
        [Route("GetAllProjects")]
        [HttpGet]
        public async Task <IActionResult> GetAllProjects()
        {
            DataTable dt = new DataTable();
            string sqlg = "select ProjectCode,ProjectName from Projects";
            SqlDataAdapter da = new SqlDataAdapter(sqlg, conn);
            da.Fill(dt);
            return Ok(dt);

        }
        // Get Units By Project Name 
        [Route("GetUnitsByProject")]
        [HttpPost]
        public async Task<IActionResult> GetUnitsByProject(int projectid)
        {
            DataTable dt = new DataTable();
            string sqlg = @"select * from Project_Available_Units 
                            where ProjectCode=@ProjectCode AND ReservedStatus=0";
            await conn.OpenAsync();
            using(SqlCommand cmd=new SqlCommand(sqlg, conn))
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ProjectCode", projectid);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            await conn.CloseAsync();
            return Ok(dt);

        }

        // Get Price Of Unit 
        [Route("GetUnitPrice")]
        [HttpPost]
        public async Task<IActionResult> GetUnitPrice(int unitid)
        {
            DataTable dt = new DataTable();
            string sqlg = @"select * from Units where UnitID =@UnitID";
            await conn.OpenAsync();
            using (SqlCommand cmd=new SqlCommand(sqlg, conn))
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@UnitID", unitid);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            await conn.CloseAsync();
            return Ok(dt);

        }
        //Save Clients with their negotiation requests 
        [Route("UpsertClient")]
        [HttpPost]
        public async Task<IActionResult> UpsertClient([FromBody] Client cl)
        {
            bool nullData = false;
            bool updated = false;
            bool saved = false;
            int id = Convert.ToInt32(cl.ClientID);
            if (id == 0 && cl==null) 
            {
                return BadRequest(new { id = 0, nullData =true,error="client data is empty" });
            }
            await conn.OpenAsync();
            using(SqlTransaction transaction = (SqlTransaction)await conn.BeginTransactionAsync())
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
                       using (SqlCommand cmd = new SqlCommand(sqlupdate, conn,transaction))
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
                            using (SqlCommand cmd = new SqlCommand(sqld, conn,transaction))
                            {
                                cmd.Parameters.AddWithValue("@ClientID", id);
                                await cmd.ExecuteNonQueryAsync();
                            }
                            string insertDetails = @"insert into  Negotiations 
                                                    (serialCode,ClientID,ClientName,ProjectCode,ProjectName,
                                                    UnitID,unitName,OriginalPrice,NegotiationPrice, DiscountAmount
                                                   ,NegotiationStatus,NegotiationDate,checkedByAdmin,Requester,Reserved)
                                                    values(@serialCode,@ClientID,@ClientName,@ProjectCode,@ProjectName,@UnitID
                                                   ,@unitName,@OriginalPrice,@NegotiationPrice,@DiscountAmount
                                                   ,@NegotiationStatus,@NegotiationDate,@checkedByAdmin,@Requester,@Reserved)";        
                            using (SqlCommand cmd = new SqlCommand(insertDetails, conn,transaction))
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
                    var data = new { 
                        id = id, 
                        nullData = nullData, 
                        saved = saved, 
                        updated = updated 
                    };
                    return Ok(data);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(new { error = ex.Message, saved = false, updated = false });
                }
                finally
                {
                    await conn.CloseAsync();
                }
            }

        }
        //Delete Clients and their negotiations
        [Route("DeleteClient")]
        [HttpDelete]
        public async Task<IActionResult> DeleteClient(int id)
        {
            bool deleted = false;
            await conn.OpenAsync();
            using ( SqlTransaction transaction = (SqlTransaction) await conn.BeginTransactionAsync())
            {
                try
                {
                    if (id > 0)
                    {
                        
                            string deleteNegotiations = "delete Negotiations where ClientID=@ClientID";
                            using (SqlCommand cmd = new SqlCommand(deleteNegotiations, conn,transaction))
                            {
                                cmd.Parameters.AddWithValue("@ClientID", id);
                                await cmd.ExecuteNonQueryAsync();
                            }
                            string deleteClient = "delete Clients where ClientID=@ClientID";
                            using (SqlCommand cmd = new SqlCommand(deleteClient, conn,transaction))
                            {
                                cmd.Parameters.AddWithValue("@ClientID", id);
                                await cmd.ExecuteNonQueryAsync();
                            }
                    }
                    await transaction.CommitAsync();
                    return Ok(new { deleted = true });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(new { error = ex.Message, deleted = false });
                }
                finally
                {
                    await conn.CloseAsync();
                }
            }
       
        }
        // Search Clients 
        [Route("GetAllClients")]
        [HttpGet]
        public async Task<IActionResult> GetAllClients()
        {
            DataTable dt = new DataTable();
            string sqlg = "select * from Clients";
            using (SqlCommand cmd = new SqlCommand(sqlg, conn))
            {
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            return Ok(dt);
        }
        // Get negotiation of client 
        [Route("GetClientNegotiations")]
        [HttpPost]
        public async Task<IActionResult> GetClientNegotiations(int clientid)
        {
            DataTable dt = new DataTable();
            string sqlg = "select * from Negotiations where ClientID=@ClientID";
            await conn.OpenAsync();
            using (SqlCommand cmd = new SqlCommand(sqlg, conn))
            {
                cmd.Parameters.AddWithValue("@ClientID", clientid);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            await conn.CloseAsync();
            return Ok(dt);
        }
        // Get first client 
        [Route("GetFirstClient")]
        [HttpGet]
        public async Task<IActionResult> GetFirstClient()
        {
            DataTable dt = new DataTable();
            int first_clientId = 0;
            bool isnull = false;
            string sqlgetFirst = "select top(1)* from Clients order by ClientID ASC";
            using (SqlCommand cmd = new SqlCommand(sqlgetFirst, conn))
            {
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
         
            if (dt.Rows.Count > 0)
            {
                first_clientId = Convert.ToInt32(dt.Rows[0]["ClientID"]);
                isnull = false;
                
            }
            else {  isnull = true; } 
            var negotiations_f = await _context.Negotiations.Where(n => n.ClientID == first_clientId).ToListAsync();
            var data = new { dt = dt, negotiations_f = negotiations_f , isnull = isnull };
            return Ok(data);
        }
        //Get last Client
        [Route("GetLastClient")]
        [HttpGet]
        public async Task<IActionResult> GetLastClient()
        {
            DataTable dt = new DataTable();
            int last_clientid = 0;
            bool isnull = false;
            string sqlgetLast = "select top(1)* from Clients order by ClientID DESC";
            using (SqlCommand cmd = new SqlCommand(sqlgetLast, conn))
            {
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
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
            var negotiations_l =await _context.Negotiations.Where(n => n.ClientID == last_clientid).ToListAsync();
            var data = new { dt = dt, negotiations_l = negotiations_l, isnull= isnull };
            return Ok(data);
        }
        // Get Next Client 
        [Route("GetNextClientById")]
        [HttpPost]
        public async Task<IActionResult> GetNextClientById(int id)
        {
            bool empty_db = false;
            DataTable dt_all = new DataTable();
            DataTable dt = new DataTable();
            int next_clientid = 0;
            bool islast = false;
            string sqlall = "select * from Clients";
            await conn.OpenAsync();
            using (SqlCommand cmdd = new SqlCommand(sqlall, conn))
            {
                 using (var reader = await cmdd.ExecuteReaderAsync())
                 {
                      dt_all.Load(reader);
                 }
            }
            if (dt_all.Rows.Count > 0)
            {
                empty_db = false;
            }
            else
            {
                empty_db = true;
            }

            if (empty_db == false)
            {
                try
                {

                    string sqlnext = "select top(1)* from Clients where ClientID > @ClientID order by ClientID ASC";
                    using (SqlCommand cmd = new SqlCommand(sqlnext, conn))
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@ClientID", id);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            dt.Load(reader);
                        }
                    }
                   
                    if (dt.Rows.Count > 0)
                    {
                        next_clientid = Convert.ToInt32(dt.Rows[0]["ClientID"]);
                        islast = false;
                    }
                    else
                    {
                        string sqllast = "select * from Clients where ClientID = @ClientID";
                        using (SqlCommand cmdl = new SqlCommand(sqllast, conn))
                        {
                            cmdl.Parameters.Clear();
                            cmdl.Parameters.AddWithValue("@ClientID", id);
                             using (var reader = await cmdl.ExecuteReaderAsync())
                             {
                                dt.Load(reader);
                             }
                        }
                        if (dt.Rows.Count > 0)
                        {
                            islast = true;
                            next_clientid = Convert.ToInt32(dt.Rows[0]["ClientID"]);
                        }
                    }

                }
                catch (Exception ex)
                {
                    return BadRequest(new { error = ex.Message });
                }
            }
            await conn.CloseAsync();
            var negotiations_n =await  _context.Negotiations.Where(n => n.ClientID == next_clientid).ToListAsync();
            var data = new { dt = dt, negotiations_n = negotiations_n,islast=islast , empty_db = empty_db };
            return Ok(data);
        }
        // Get Previous Client 
        [Route("GetPreviousClientById")]
        [HttpPost]
        public async Task<IActionResult> GetPreviousClientById(int id)
        {
            DataTable dt = new DataTable();
            DataTable dt_all = new DataTable();
            int previous_clientid = 0;
            bool isfirst = false;
            bool empty_db = false;
            string sqlall = "select * from Clients";
            await conn.OpenAsync();
            using (SqlCommand cmdd = new SqlCommand(sqlall, conn))
            {
                using (var reader = await cmdd.ExecuteReaderAsync())
                {
                    dt_all.Load(reader);
                }
            }
            if (dt_all.Rows.Count > 0)
            {
                empty_db = false;
            }
            else
            {
                empty_db = true;
            }
            if (empty_db == false)
            {

                try
                {
                    string sqlnext = @"select top(1)* from Clients where ClientID < @ClientID order by ClientID DESC";
                    using (SqlCommand cmd = new SqlCommand(sqlnext, conn))
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@ClientID", id);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            dt.Load(reader);
                        }
                    }
                   
                    if (dt.Rows.Count > 0)
                    {
                        previous_clientid = Convert.ToInt32(dt.Rows[0]["ClientID"]);
                        isfirst = false;
                    }
                    else
                    {
                        string sqlfirst = @"select * from Clients where ClientID=@ClientID";
                        using (SqlCommand cmdf = new SqlCommand(sqlfirst, conn))
                        {
                            cmdf.Parameters.Clear();
                            cmdf.Parameters.AddWithValue("@ClientID", id);
                            using (var reader = await cmdf.ExecuteReaderAsync())
                            {
                                dt.Load(reader);
                            }
                        }
                        if (dt.Rows.Count > 0)
                        {
                            isfirst = true;
                            previous_clientid = Convert.ToInt32(dt.Rows[0]["ClientID"]);
                        }
                    }
                   
                }
                catch (Exception ex)
                {

                    return BadRequest(new { error = ex.Message });
                }

            }
            await conn.CloseAsync();
            var negotiations_p = await _context.Negotiations.Where(n => n.ClientID == previous_clientid).ToListAsync();
            var data = new { dt = dt, negotiations_p = negotiations_p, isfirst = isfirst, empty_db = empty_db };
            return Ok(data);


        }
       

    }
}
