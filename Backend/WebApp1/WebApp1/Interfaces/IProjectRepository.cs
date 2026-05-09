using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApp1.Models;

namespace WebApp1.Interfaces
{
    public interface IProjectRepository
    {
        Task<IEnumerable<Project>> GetAllProjects();
        Task<int> UpsertProjectWithUnits(Project prj);
        Task<bool> DeleteProject(int id);
        Task<IEnumerable<Unit>> GetUnitsByProjectId(int projectId);
        Task <string> UploadImage(IFormFile file, string folderName);
    }
}
