using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
using WebApp1.EF;
using WebApp1.Models;
namespace WebApp1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NegotiationController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;
        SqlConnection conn;
        public NegotiationController(DataContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
            conn = new SqlConnection(_context.Database.GetConnectionString());
        }
        [Route("GetPendingNegotiationsCount")]
        [HttpGet]
        public JsonResult GetPendingNegotiationsCount()
        {
            string query = "SELECT COUNT(*) FROM Negotiations where checkedByAdmin=0";


            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                int count = (int)cmd.ExecuteScalar();
                if (conn.State == ConnectionState.Open) conn.Close();
                return new JsonResult(count);
            }

        }
        // Get Count Number Of Unchecked Requests By Admin 
        [Route("GetPendingNegotiations")]
        [HttpGet]
        public JsonResult GetPendingNegotiations()
        {
          
            DataTable dt = new DataTable();
            string sqlg = "select * from Negotiations where checkedByAdmin=0";
            SqlDataAdapter da = new SqlDataAdapter(sqlg, conn);
            da.Fill(dt);
            return new JsonResult(dt);
        }
        //Approve Or Reject Negotiation Request By Admin 
        [Route("ProcessNegotiationReview")]
        [HttpPost]
        public JsonResult ProcessNegotiationReview([FromBody] Rejected_negotiations_phase ph)
        {
            bool saved = false;
            bool cond = Convert.ToBoolean(ph.NegotiationCondition);
            string approvedstatement = "مقبول";
            string rejectstatement = "مرفوض";
            if (ph.ClientID!=0)
            {

            try
            {
                string sqlin = @"insert into Rejected_negotiations_phases
                               (ClientID,ProjectCode,UnitID,
                                NegotiationCondition,SuggestedPrice,ReasonOfReject,CheckedDate) 
                                values(@ClientID,@ProjectCode,@UnitID,@NegotiationCondition,
                                 @SuggestedPrice,@ReasonOfReject,@CheckedDate)"; 
                using (SqlCommand cmd = new SqlCommand(sqlin, conn))
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@ClientID", ph.ClientID);
                    cmd.Parameters.AddWithValue("@ProjectCode", ph.ProjectCode);
                    cmd.Parameters.AddWithValue("@UnitID", ph.UnitID);
                    cmd.Parameters.AddWithValue("@NegotiationCondition", ph.NegotiationCondition);
                    cmd.Parameters.AddWithValue("@SuggestedPrice", ph.SuggestedPrice);
                    cmd.Parameters.AddWithValue("@ReasonOfReject", string.IsNullOrEmpty(ph.ReasonOfReject) ? DBNull.Value : ph.ReasonOfReject);
                    cmd.Parameters.AddWithValue("@CheckedDate",ph.CheckedDate);
                    cmd.ExecuteNonQuery();
                    if (conn.State == ConnectionState.Open) conn.Close();
                    saved = true;
                }

            }
            catch
            {

                saved = false;
            }
                    
            if (cond==true)
            {
                    
                try
                {
                    string sqlup = @"update Negotiations set NegotiationStatus='" + approvedstatement + "' , " +
                                    "checkedByAdmin=1 where ClientID=@ClientID AND " +
                                    "ProjectCode=@ProjectCode AND " +
                                    "UnitID=@UnitID";
                    using (SqlCommand cmd = new SqlCommand(sqlup, conn))
                    {
                            if (conn.State == ConnectionState.Closed) conn.Open();
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@ClientID", ph.ClientID);
                            cmd.Parameters.AddWithValue("@ProjectCode", ph.ProjectCode);
                            cmd.Parameters.AddWithValue("@UnitID", ph.UnitID);
                            cmd.ExecuteNonQuery();
                            if (conn.State == ConnectionState.Open) conn.Close();
                            saved = true;
                    }
                }
                catch 
                {

                    saved = false;
                }
               
            }
            else
            {
                try
                {
                    string sqlup = @"update Negotiations set NegotiationStatus='" + rejectstatement + "' , " +
                                     "checkedByAdmin=1 where ClientID=@ClientID AND " +
                                     "ProjectCode=@ProjectCode AND " +
                                     "UnitID=@UnitID";
                    using (SqlCommand cmd = new SqlCommand(sqlup, conn))
                    { 
                        if (conn.State == ConnectionState.Closed) conn.Open();
                            cmd.Parameters.Clear();
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@ClientID", ph.ClientID);
                            cmd.Parameters.AddWithValue("@ProjectCode", ph.ProjectCode);
                            cmd.Parameters.AddWithValue("@UnitID", ph.UnitID);
                            cmd.ExecuteNonQuery();
                        if (conn.State == ConnectionState.Open) conn.Close();
                        saved = true;
                    }
                }
                catch
                {

                    saved = false;
                }

            }
            }
            var data = new { saved = saved };
            return new JsonResult(data);
        }
        // Re_Approve Or Re_Reject Negotiation Request By Admin 
        [Route("UpdateNegotiationReview")]
        [HttpPost]
        public JsonResult UpdateNegotiationReview([FromBody] Rejected_negotiations_phase ph2)
        {
            bool cond = Convert.ToBoolean(ph2.NegotiationCondition);
            bool  Re_Approved = false;
            bool Re_Rejected = false;
            string rejectstatement = "مرفوض";
            string approvedstatement = "مقبول";
            try
            {
                string sqlMerge = @" MERGE INTO Rejected_negotiations_phases AS Target
                                     USING (SELECT @ClientID AS CID, @ProjectCode AS PC, @UnitID AS U) AS Source
                                     ON (Target.ClientID = Source.CID AND Target.ProjectCode = Source.PC AND 
                                     Target.UnitID = Source.U)
                                     WHEN MATCHED THEN
                                     UPDATE SET 
                                     NegotiationCondition = @NegotiationCondition,
                                     SuggestedPrice = @SuggestedPrice,
                                     ReasonOfReject = @ReasonOfReject,
                                     CheckedDate = @CheckedDate
                                     WHEN NOT MATCHED THEN
                                     INSERT (ClientID, ProjectCode, UnitID, NegotiationCondition, 
                                     SuggestedPrice, ReasonOfReject, CheckedDate)
                                     VALUES (@ClientID, @ProjectCode, @UnitID, @NegotiationCondition, 
                                     @SuggestedPrice, @ReasonOfReject, @CheckedDate);";

                using (SqlCommand cmd = new SqlCommand(sqlMerge, conn))
                {
                    cmd.Parameters.AddWithValue("@ClientID", ph2.ClientID);
                    cmd.Parameters.AddWithValue("@ProjectCode", ph2.ProjectCode);
                    cmd.Parameters.AddWithValue("@UnitID", ph2.UnitID);
                    cmd.Parameters.AddWithValue("@NegotiationCondition", ph2.NegotiationCondition);
                    cmd.Parameters.AddWithValue("@SuggestedPrice", ph2.SuggestedPrice);
                    cmd.Parameters.AddWithValue("@ReasonOfReject", (object)ph2.ReasonOfReject ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CheckedDate", ph2.CheckedDate);
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    cmd.ExecuteNonQuery();
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
                if (cond == false)
                {

                    try
                    {
                        string sqlup = @"update Negotiations set NegotiationStatus='" + rejectstatement + "' " +
                                        "where ClientID=@ClientID AND ProjectCode=@ProjectCode AND UnitID=@UnitID";
                        using (SqlCommand cmd = new SqlCommand(sqlup, conn))
                        {
                            if (conn.State == ConnectionState.Closed) conn.Open();
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@ClientID", ph2.ClientID);
                            cmd.Parameters.AddWithValue("@ProjectCode", ph2.ProjectCode);
                            cmd.Parameters.AddWithValue("@UnitID", ph2.UnitID);
                            cmd.ExecuteNonQuery();
                            if (conn.State == ConnectionState.Open) conn.Close();
                            Re_Rejected = true;
                        }
                    }


                    catch (Exception ex)
                    {
                        return new JsonResult(new { error = ex.Message });
                    }
                }
                if (cond == true)
                {

                    try
                    {
                        string sqlup = @"update Negotiations set NegotiationStatus='" + approvedstatement + "' " +
                                        "where ClientID=@ClientID AND ProjectCode=@ProjectCode AND UnitID=@UnitID";
                        using (SqlCommand cmd = new SqlCommand(sqlup, conn))
                        {
                            if (conn.State == ConnectionState.Closed) conn.Open();
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@ClientID", ph2.ClientID);
                            cmd.Parameters.AddWithValue("@ProjectCode", ph2.ProjectCode);
                            cmd.Parameters.AddWithValue("@UnitID", ph2.UnitID);
                            cmd.ExecuteNonQuery();
                            if (conn.State == ConnectionState.Open) conn.Close();
                            Re_Approved = true;
                        }
                    }


                    catch (Exception ex)
                    {
                        return new JsonResult(new { error = ex.Message });
                    }
                }

            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = ex.Message });
            }
            var data = new { Re_Approved = Re_Approved, Re_Rejected = Re_Rejected };
            return new JsonResult(data);
        }
        // Get Rejected Requests And Their Count Number
        [Route("GetRejectedNegotiations")]
        [HttpGet]
        public JsonResult GetRejectedNegotiations()
        {
            int count = 0;
            DataTable dt = new DataTable();
            string sqld = "select * from Negotiations_2 where NegotiationCondition=0 AND " +
                          "checkedByAdmin=1 AND " +
                          "Reserved=0";
            SqlDataAdapter da = new SqlDataAdapter(sqld, conn);
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                count= dt.Rows.Count;
            }
            var data = new { count = count, dt = dt };
            return new JsonResult(data);
        }
        // Get Accepted Requests And Their Count Number 
        [Route("GetApprovedNegotiations")]
        [HttpGet]
        public JsonResult GetApprovedNegotiations()
        {
            int count_a = 0;
            DataTable dt = new DataTable();
            string sqld = "select * from Negotiations_2 where" +
                            " NegotiationCondition=1 AND " +
                            "checkedByAdmin=1 " +
                            "AND Reserved=0";
            SqlDataAdapter da = new SqlDataAdapter(sqld, conn);
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                count_a = dt.Rows.Count;
            }
            var data = new { count_a = count_a, dt = dt };
            return new JsonResult(data);

        }
       
       

    }
}
