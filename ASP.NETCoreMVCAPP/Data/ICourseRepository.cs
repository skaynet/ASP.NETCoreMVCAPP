using ASP.NETCoreMVCAPP.Models;

namespace ASP.NETCoreMVCAPP.Data
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<Course> GetCourseWithGroupsByIdAsync(int id);
    }
}