using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApp1.Models;

namespace WebApp1.Interfaces
{
    public interface IProjectRepository
    {
        Task<DataTable> GetAllProjectsAsync();
        Task<int> UpsertProjectWithUnitsAsync(Project prj);
        Task<bool> DeleteProjectAsync(int id);
        Task<DataTable> GetUnitsByProjectIdAsync(int projectId);
        Task <string> UploadImage(IFormFile file, string folderName);
    }
}
