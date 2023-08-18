using ASP.NETCoreMVCAPP.Models;

namespace ASP.NETCoreMVCAPP.Data
{
    public interface IGroupRepository : IRepository<Group>
    {
        Task<Group> GetGroupWithCourseAndStudentsByIdAsync(int id);
        Task<Group> GetGroupWithCourseByIdAsync(int id);
        Task<IEnumerable<Group>> GetGroupsWithCourseForCourseAsync(int courseId);
        IEnumerable<Course> GetCourses();
    }
}