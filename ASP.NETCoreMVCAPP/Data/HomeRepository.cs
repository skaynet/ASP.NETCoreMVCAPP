using ASP.NETCoreMVCAPP.Models;
using Microsoft.EntityFrameworkCore;

namespace ASP.NETCoreMVCAPP.Data
{
    public class HomeRepository : IHomeRepository
    {
        private readonly UniversityContext _context;

        public HomeRepository(UniversityContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _context.Students.Include(s => s.Group).Include(g => g.Group.Course).ToListAsync();
        }
    }
}
