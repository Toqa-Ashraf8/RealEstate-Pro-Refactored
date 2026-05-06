using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using WebApp1.EF;
using WebApp1.Models;
namespace WebApp1.Controllers
  
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;
        SqlConnection conn;
        public ProjectController(DataContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
            conn = new SqlConnection(_context.Database.GetConnectionString());

        }
        // Save Images Of Projects
        [Route("UploadProjectImage")]
        [HttpPost]
        public async Task<IActionResult> UploadProjectImage([FromForm] upload_Projects_Images uploadimg)

        {
            var postedFile = uploadimg.file;
            string fileName = postedFile.FileName;
            var physicalPath = _env.ContentRootPath + "/Photos_projects/" + fileName;
            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await postedFile.CopyToAsync(stream);
            }
            return Ok(fileName);
        }
        // Save Images Of Units 
        [Route("UploadUnitImage")]
        [HttpPost]
        public async Task<IActionResult> UploadUnitImage([FromForm] upload_Unit_Images imgu)
        {
            var postedFile = imgu.fileu;
            string fileName = postedFile.FileName;
            var physicalPath = _env.ContentRootPath + "/Photos_Units/" + fileName;
            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await postedFile.CopyToAsync(stream);
            }
            return Ok(fileName);
        }

        // Save Projects (Master) With Units (Details)
        [Route("UpsertProjectWithUnits")]
        [HttpPost]
        public async Task<IActionResult> UpsertProjectWithUnits([FromBody] Project prj)
        {

            bool allFieldsAreEmpty = true;
            bool errorOccured = false;
            int id = Convert.ToInt32(prj.ProjectCode);
            if (!string.IsNullOrEmpty(prj.ProjectName)) allFieldsAreEmpty = false;
            if (allFieldsAreEmpty) return BadRequest(new { id = 0, errorOccured = true });
            if (conn.State != ConnectionState.Open) conn.Open();
            using (SqlTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    if (id == 0)
                    {
                        string sqlin = @"insert into Projects (ProjectName,ProjectType,
                                     Location,TotalUnits,ProjectStatus,ProjectImage)
                                     values(@ProjectName,@ProjectType,@Location,@TotalUnits,
                                     @ProjectStatus,@ProjectImage)
                                      SELECT SCOPE_IDENTITY()";

                        using (SqlCommand cmd = new SqlCommand(sqlin, conn, transaction))
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@ProjectName", prj.ProjectName);
                            cmd.Parameters.AddWithValue("@ProjectType", prj.ProjectType);
                            cmd.Parameters.AddWithValue("@Location", prj.Location);
                            cmd.Parameters.AddWithValue("@TotalUnits", prj.TotalUnits);
                            cmd.Parameters.AddWithValue("@ProjectStatus", prj.ProjectStatus);
                            cmd.Parameters.AddWithValue("@ProjectImage", prj.ProjectImage);
                            id = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                        }
                    }
                    else
                    {

                        string sqlupdate = @"update Projects set ProjectName=@ProjectName,
                                       ProjectType=@ProjectType,Location=@Location
                                      ,TotalUnits=@TotalUnits,ProjectStatus=@ProjectStatus,
                                       ProjectImage=@ProjectImage
                                       where ProjectCode=@ProjectCode";
                        using (SqlCommand cmd = new SqlCommand(sqlupdate, conn, transaction))
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@ProjectName", prj.ProjectName);
                            cmd.Parameters.AddWithValue("@ProjectType", prj.ProjectType);
                            cmd.Parameters.AddWithValue("@Location", prj.Location);
                            cmd.Parameters.AddWithValue("@TotalUnits", prj.TotalUnits);
                            cmd.Parameters.AddWithValue("@ProjectStatus", prj.ProjectStatus);
                            cmd.Parameters.AddWithValue("@ProjectImage", prj.ProjectImage);
                            cmd.Parameters.AddWithValue("@ProjectCode", id);
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                    string sqldelete = @"delete Units where ProjectCode =@ProjectCode";
                    using (SqlCommand cmd = new SqlCommand(sqldelete, conn, transaction))
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@ProjectCode", id);
                        await cmd.ExecuteNonQueryAsync();
                    }
                    if (prj.units != null && prj.units.Count > 0)
                    {
                        string sqlinsert = @"insert into Units (serial,unitName,Floor,TotalArea,
                                  MeterPrice,TotalPrice,unitImage,ProjectCode,ProjectName,ReservedStatus)
                                  values(@serial,@unitName,@Floor,@TotalArea,@MeterPrice,
                                  @TotalPrice,@unitImage,@ProjectCode,@ProjectName,@ReservedStatus)";

                        using (SqlCommand cmd = new SqlCommand(sqlinsert, conn, transaction))
                       
                            foreach (var unit in prj.units)
                            {
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("@serial", SqlDbType.Int).Value = unit.serial;
                                cmd.Parameters.Add("@unitName", SqlDbType.NVarChar).Value = unit.unitName;
                                cmd.Parameters.Add("@Floor", SqlDbType.NVarChar).Value = unit.Floor;
                                cmd.Parameters.Add("@TotalArea", SqlDbType.Decimal).Value = unit.TotalArea;
                                cmd.Parameters.Add("@MeterPrice", SqlDbType.Int).Value = unit.MeterPrice;
                                cmd.Parameters.Add("@TotalPrice", SqlDbType.Decimal).Value = unit.TotalPrice;
                                cmd.Parameters.Add("@unitImage", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(unit.unitImage) ? DBNull.Value : unit.unitImage;
                                cmd.Parameters.Add("@ProjectCode", SqlDbType.Int).Value = id;
                                cmd.Parameters.Add("@ProjectName", SqlDbType.NVarChar).Value = prj.ProjectName;
                                cmd.Parameters.Add("@ReservedStatus", SqlDbType.Bit).Value = Convert.ToBoolean(unit.ReservedStatus);
                                await cmd.ExecuteNonQueryAsync();
                            }
                    }
                    await transaction.CommitAsync();
                    return Ok(new { id = id, errorOccured = false });
                }
                catch(Exception ex)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(new { error = ex.Message });
                }
                finally
                {

                    if (conn.State == ConnectionState.Open) conn.Close();
                }

            }

        }

        // Delete Projects (Master) With Units (Details) 

        [Route("DeleteProject")]
        [HttpPost]
        public async Task<IActionResult> DeleteProject(int id)
        {
            
            bool delOk = false;
            if (conn.State != ConnectionState.Open) conn.Open();
            using (SqlTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    if (id > 0)
                    {
                        string deleteunits = @"delete Units where ProjectCode=@projectCode";
                        using (SqlCommand cmd = new SqlCommand(deleteunits, conn,transaction))
                        {
                            cmd.Parameters.AddWithValue("@projectCode", id);
                            await cmd.ExecuteNonQueryAsync();
                        }

                        string deleteproj = @"delete Projects where ProjectCode=@projectCode";
                        using (SqlCommand cmd = new SqlCommand(deleteproj, conn,transaction))
                        {
                            cmd.Parameters.AddWithValue("@projectCode", id);
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                    await transaction.CommitAsync();
                    return Ok(new { delOk = true });
                }
                catch(Exception ex)
                {
                    await  transaction.RollbackAsync();
                    return BadRequest(new { error=ex.Message,delOk = false });
                }
                finally
                {
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
                
            }
           
        }
        //Get Projects (Master) To Search 
        [Route("GetAllProjects")]
        [HttpGet]
        public async Task<IActionResult> GetAllProjects()
        {
            DataTable dt = new DataTable();
            string getdata = @"select * from Projects";
            SqlDataAdapter da = new SqlDataAdapter(getdata, conn);
            da.Fill(dt);
            return Ok(dt);

        }
        // Get Units (Display in cards)
        [Route("GetProjectUnits")]
        [HttpPost]
        public async Task<IActionResult> GetProjectUnits(int projectId)
         {
            DataTable dt = new DataTable();
            string getdata = @"select * from Units where ProjectCode=@ProjectCode";
            using (SqlCommand cmd = new SqlCommand(getdata, conn))
            {
                cmd.Parameters.AddWithValue("@ProjectCode", projectId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return Ok(dt);

        }
        //Get Units (details)
        [Route("GetUnitsByProject")]
        [HttpPost]
        public async Task<IActionResult> GetUnitsByProject(int Id)
        {
            DataTable dt = new DataTable();
            string getdetails = @"select * from Units where ProjectCode=@ProjectCode";
            using (SqlCommand cmd = new SqlCommand(getdetails, conn))
            {
                cmd.Parameters.AddWithValue("@ProjectCode", Id);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return Ok(dt);
        }

      

    }
}
