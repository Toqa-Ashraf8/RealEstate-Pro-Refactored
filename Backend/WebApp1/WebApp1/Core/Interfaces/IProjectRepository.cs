using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApp1.Core.Models;

namespace WebApp1.Core.Interfaces
{
    public interface IProjectRepository
    {
        Task<IEnumerable<Project>> GetAllProjects();
        Task<(int id, bool saved, bool updated)> UpsertProjectWithUnits(Project prj);
        Task<bool> DeleteProject(int id);
        Task<IEnumerable<Unit>> GetUnitsByProjectId(int projectId);
        Task <string> UploadImage(IFormFile file, string folderName);
    }
}
