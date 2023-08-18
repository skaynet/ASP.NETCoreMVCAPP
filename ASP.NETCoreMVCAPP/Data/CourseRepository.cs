using ASP.NETCoreMVCAPP.Models;
using Microsoft.EntityFrameworkCore;

namespace ASP.NETCoreMVCAPP.Data
{
    public class CourseRepository : ICourseRepository
    {
        private readonly UniversityContext _context;

        public CourseRepository(UniversityContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _context.Courses.ToListAsync();
        }

        public async Task<Course> GetByIdAsync(int id)
        {
            return await _context.Courses.FindAsync(id);
        }

        public async Task<Course> GetCourseWithGroupsByIdAsync(int id)
        {
            return await _context.Courses.Include(c => c.Groups).FirstOrDefaultAsync(c => c.CourseId == id);
        }

        public async Task CreateAsync(Course entity)
        {
            _context.Courses.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Course entity)
        {
            _context.Courses.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Course entity)
        {
            _context.Courses.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public bool IsExists(int id)
        {
            return (_context.Courses?.Any(e => e.CourseId == id)).GetValueOrDefault();
        }
    }
}