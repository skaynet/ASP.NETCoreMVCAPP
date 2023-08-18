using ASP.NETCoreMVCAPP.Models;

namespace ASP.NETCoreMVCAPP.Data
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<Student> GetStudentWithGroupByIdAsync(int id);
        Task<IEnumerable<Student>> GetStudentsWithGroupForGroupAsync(int groupId);
        IEnumerable<Group> GetGroups();
    }
}
