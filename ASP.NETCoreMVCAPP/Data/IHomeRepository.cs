using ASP.NETCoreMVCAPP.Models;

namespace ASP.NETCoreMVCAPP.Data
{
    public interface IHomeRepository
    {
        Task<IEnumerable<Student>> GetAllAsync();
    }
}
