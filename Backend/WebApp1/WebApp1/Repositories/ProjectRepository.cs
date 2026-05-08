using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using WebApp1.Interfaces;
using WebApp1.Models;

namespace WebApp1.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _env;
        public ProjectRepository(IConfiguration configuration, IWebHostEnvironment env)
        {
            _connectionString = configuration.GetConnectionString("connT");
            _env = env;
        }

        public async Task<DataTable> GetAllProjectsAsync()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Projects";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    await Task.Run(() => da.Fill(dt));
                }
            }
            return dt;
        }

        public async Task<int> UpsertProjectWithUnitsAsync(Project prj)
        {
            int id = Convert.ToInt32(prj.ProjectCode);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        if (id == 0)
                        {
                            string sqlInsert = @"INSERT INTO Projects (ProjectName, ProjectType, Location, TotalUnits, ProjectStatus, ProjectImage)
                                        VALUES (@ProjectName, @ProjectType, @Location, @TotalUnits, @ProjectStatus, @ProjectImage);
                                        SELECT SCOPE_IDENTITY();";

                            using (SqlCommand cmd = new SqlCommand(sqlInsert, conn, transaction))
                            {
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
                            string sqlUpdate = @"UPDATE Projects SET ProjectName=@ProjectName, ProjectType=@ProjectType, 
                                        Location=@Location, TotalUnits=@TotalUnits, ProjectStatus=@ProjectStatus, 
                                        ProjectImage=@ProjectImage WHERE ProjectCode=@ProjectCode";

                            using (SqlCommand cmd = new SqlCommand(sqlUpdate, conn, transaction))
                            {
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
                        string sqlDeleteUnits = "DELETE FROM Units WHERE ProjectCode = @ProjectCode";
                        using (SqlCommand cmd = new SqlCommand(sqlDeleteUnits, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@ProjectCode", id);
                            await cmd.ExecuteNonQueryAsync();
                        }

                        if (prj.units != null && prj.units.Count > 0)
                        {
                            string sqlInsertUnits = @"INSERT INTO Units (serial, unitName, Floor, TotalArea, MeterPrice, TotalPrice, unitImage, ProjectCode, ProjectName, ReservedStatus)
                                            VALUES (@serial, @unitName, @Floor, @TotalArea, @MeterPrice, @TotalPrice, @unitImage, @ProjectCode, @ProjectName, @ReservedStatus)";

                            foreach (var unit in prj.units)
                            {
                                using (SqlCommand cmd = new SqlCommand(sqlInsertUnits, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@serial", unit.serial);
                                    cmd.Parameters.AddWithValue("@unitName", unit.unitName);
                                    cmd.Parameters.AddWithValue("@Floor", unit.Floor);
                                    cmd.Parameters.AddWithValue("@TotalArea", unit.TotalArea);
                                    cmd.Parameters.AddWithValue("@MeterPrice", unit.MeterPrice);
                                    cmd.Parameters.AddWithValue("@TotalPrice", unit.TotalPrice);
                                    cmd.Parameters.AddWithValue("@unitImage", unit.unitImage);
                                    cmd.Parameters.AddWithValue("@ProjectCode", id);
                                    cmd.Parameters.AddWithValue("@ProjectName", prj.ProjectName);
                                    cmd.Parameters.AddWithValue("@ReservedStatus", unit.ReservedStatus);
                                    await cmd.ExecuteNonQueryAsync();
                                }
                            }
                        }

                        await transaction.CommitAsync();
                        return id;
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        public async Task<bool> DeleteProjectAsync(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string deleteUnits = "DELETE Units WHERE ProjectCode = @id";
                        using (SqlCommand cmd = new SqlCommand(deleteUnits, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            await cmd.ExecuteNonQueryAsync();
                        }

                        string deleteProj = "DELETE Projects WHERE ProjectCode = @id";
                        using (SqlCommand cmd = new SqlCommand(deleteProj, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            await cmd.ExecuteNonQueryAsync();
                        }

                        await transaction.CommitAsync();
                        return true;
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        public async Task<DataTable> GetUnitsByProjectIdAsync(int projectId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Units WHERE ProjectCode = @projectId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@projectId", projectId);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        await Task.Run(() => da.Fill(dt));
                    }
                }
            }
            return dt;
        }

        public async Task<string> UploadImage(IFormFile file, string folderName)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return ("No file uploaded.");
                string fileName = file.FileName;
                var physicalPath = Path.Combine(_env.ContentRootPath, folderName, fileName);

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
}   }