using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using WebApp1.Interfaces;
using WebApp1.Models;

namespace WebApp1.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly DbConnection _db;
        private readonly IWebHostEnvironment _env;
        public ProjectRepository(DbConnection db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task <IEnumerable<Project>> GetAllProjects()
        {
             string query = "SELECT * FROM Projects";
             return await _db.QueryAsync<Project>(query);
        }

        public async Task<int> UpsertProjectWithUnits(Project prj)
        {
            int id = Convert.ToInt32(prj.ProjectCode);
            await _db.OpenAsync();
            using var transaction = _db.BeginTransaction();

            try
            {
                if (id == 0)
                {
                    string sqlInsert = @"INSERT INTO Projects (ProjectName, ProjectType, Location, TotalUnits, ProjectStatus, ProjectImage)
                                        VALUES (@ProjectName, @ProjectType, @Location, @TotalUnits, @ProjectStatus, @ProjectImage);
                                        SELECT CAST(SCOPE_IDENTITY() as int);";
                     
                    id = await _db.ExecuteScalarAsync<int>(sqlInsert, prj, transaction);
                }
                else
                {
                    string sqlUpdate = @"UPDATE Projects SET ProjectName=@ProjectName, ProjectType=@ProjectType, 
                                        Location=@Location, TotalUnits=@TotalUnits, ProjectStatus=@ProjectStatus, 
                                        ProjectImage=@ProjectImage WHERE ProjectCode=@ProjectCode";

                    await _db.ExecuteAsync(sqlUpdate, prj, transaction);
                }
                string sqlDeleteUnits = "DELETE FROM Units WHERE ProjectCode = @ProjectCode";
                await _db.ExecuteAsync(sqlDeleteUnits, new { id }, transaction);

                if (prj.units != null && prj.units.Count > 0)
                {
                    prj.units.ForEach(u => u.ProjectCode = id);
                    string sqlInsertUnits = @"INSERT INTO Units (serial, unitName, Floor, TotalArea, MeterPrice, TotalPrice, unitImage, ProjectCode, ProjectName, ReservedStatus)
                    VALUES (@serial, @unitName, @Floor, @TotalArea, @MeterPrice, @TotalPrice, @unitImage, @ProjectCode, @ProjectName, @ReservedStatus)
                    SELECT CAST(SCOPE_IDENTITY() as int)";
                    await _db.ExecuteAsync(sqlInsertUnits, prj.units, transaction);
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

        public async Task<bool> DeleteProject(int id)
        {
            await _db.OpenAsync();
            using var transaction = _db.BeginTransaction();
                {
                    try
                    {
                        string deleteUnits = "DELETE Units WHERE ProjectCode = @id";
                       await _db.ExecuteAsync(deleteUnits, new {id},transaction);

                    string deleteProj = "DELETE Projects WHERE ProjectCode = @id";
                    await _db.ExecuteAsync(deleteProj, new { id }, transaction);

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

        public async Task<IEnumerable<Unit>> GetUnitsByProjectId(int projectId)
        { 
            string query = "SELECT * FROM Units WHERE ProjectCode = @projectId";
            return await _db.QueryAsync<Unit>(query, new { projectId });
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