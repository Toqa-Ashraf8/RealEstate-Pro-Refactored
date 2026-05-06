using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using WebApp1.EF;
using System.Globalization;

namespace WebApp1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly DataContext _context;
        SqlConnection conn;
        public DashboardController(DataContext context)
        {
            _context = context;
            conn=new SqlConnection(_context.Database.GetConnectionString());
        }
        [Route("GetProjectsUnitsStats")]
        [HttpGet]
        public JsonResult GetProjectsUnitsStats()
        {

            DataTable dt = new DataTable();
            string sql= @"SELECT p.ProjectName, COUNT(u.UnitID) as TotalUnitsCount
                         FROM Projects p
                         LEFT JOIN Units u ON p.ProjectCode = u.ProjectCode
                         GROUP BY p.ProjectName";
            if (conn.State == ConnectionState.Closed) conn.Open();
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            da.Fill(dt);
            if (conn.State == ConnectionState.Open) conn.Close();
            return new JsonResult(dt);

        }

        [Route("GetDailyStats")]
        [HttpGet]
        public JsonResult GetDailyStats()
        {
          
            DataTable dt = new DataTable();
            string sqls = @"SELECT 
                            MONTH(BookingDate) AS MonthNumber, 
                            COUNT(*) AS BookingCount
                            FROM reserved_clients_details
                            WHERE YEAR(BookingDate) = YEAR(GETDATE())
                            GROUP BY MONTH(BookingDate)";

     
            if (conn.State == ConnectionState.Closed) conn.Open();
            SqlDataAdapter da = new SqlDataAdapter(sqls, conn);
            da.Fill(dt);
            if (conn.State == ConnectionState.Open) conn.Close();
            var finalResult = Enumerable.Range(1, 12).Select(i => {
                var row = dt.AsEnumerable().FirstOrDefault(r => Convert.ToInt32(r["MonthNumber"]) == i);

                return new
                {
                    MonthName = CultureInfo.GetCultureInfo("ar-EG").DateTimeFormat.GetMonthName(i),
                    
                    BookingCount = row != null ? Convert.ToInt32(row["BookingCount"]) : 0,
                    MonthNumber = i
                };
            }).ToList();
            return new JsonResult(finalResult);
        
         }

        [Route("GetProjectsCount")]
        [HttpGet]
        public JsonResult GetProjectsCount()
        {
            DataTable dt = new DataTable();
            int projectCount;
            string sqlg = "select * from Projects";
            SqlDataAdapter da = new SqlDataAdapter(sqlg, conn);
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                projectCount = dt.Rows.Count;
            }
            else
            {
                projectCount = 0;
            }
            return new JsonResult(projectCount);
           
        }

        [Route("GetClientsCount")]
        [HttpGet]
        public JsonResult GetClientsCount()
        {
            DataTable dt = new DataTable();
            int count;
            string sqlg = "select * from Clients";
            SqlDataAdapter da = new SqlDataAdapter(sqlg, conn);
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                count = dt.Rows.Count;
            }
            else
            {
                count = 0;
            }
            return new JsonResult(count);

        }

        [Route("GetNegotiationsCount")]
        [HttpGet]
        public JsonResult GetNegotiationsCount()
        {
            DataTable dt = new DataTable();
            int negotiationCount;
            string sqlg = "select * from Negotiations where checkedByAdmin=0";
            SqlDataAdapter da = new SqlDataAdapter(sqlg, conn);
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                negotiationCount = dt.Rows.Count;
            }
            else
            {
                negotiationCount = 0;
            }
            return new JsonResult(negotiationCount);

        }

        [Route("GetReservedUnits")]
        [HttpGet]
        public JsonResult GetReservedUnits()
        {
            DataTable dt = new DataTable();
            int unitsCount;
            string sqlg = "select * from Units where ReservedStatus=1";
            SqlDataAdapter da = new SqlDataAdapter(sqlg, conn);
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                unitsCount = dt.Rows.Count;
            }
            else
            {
                unitsCount = 0;
            }
            return new JsonResult(unitsCount);
        }

        [Route("SetAvailableUnits")]
        [HttpGet]
        public JsonResult SetAvailableUnits()
        {
            DataTable dt = new DataTable();
            int availableUnits;
            string sqlg = "select * from Units where ReservedStatus=0";
            SqlDataAdapter da = new SqlDataAdapter(sqlg, conn);
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
               availableUnits = dt.Rows.Count;
            }
            else
            {
                availableUnits = 0;
            }
            return new JsonResult(availableUnits);
        }
    }
}
