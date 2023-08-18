using ASP.NETCoreMVCAPP.Models;
using Microsoft.EntityFrameworkCore;

namespace ASP.NETCoreMVCAPP.Data
{
    public class GroupRepository : IGroupRepository
    {
        private readonly UniversityContext _context;

        public GroupRepository(UniversityContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Group>> GetAllAsync()
        {
            return await _context.Groups.Include(c => c.Course).ToListAsync();
        }

        public async Task<Group> GetByIdAsync(int id)
        {
            return await _context.Groups.FindAsync(id);
        }

        public async Task<Group> GetGroupWithCourseByIdAsync(int id)
        {
            return await _context.Groups.Include(c => c.Course).FirstOrDefaultAsync(g => g.GroupId == id);
        }

        public async Task<IEnumerable<Group>> GetGroupsWithCourseForCourseAsync(int courseId)
        {
            return await _context.Groups.Include(c => c.Course).Where(g => g.CourseId == courseId).ToListAsync();
        }

        public IEnumerable<Course> GetCourses()
        {
            return _context.Courses.ToList();
        }

        public async Task<Group> GetGroupWithCourseAndStudentsByIdAsync(int id)
        {
            return await _context.Groups.Include(c => c.Course).Include(s => s.Students).FirstOrDefaultAsync(c => c.GroupId == id);
        }

        public async Task CreateAsync(Group entity)
        {
            _context.Groups.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Group entity)
        {
            _context.Groups.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Group entity)
        {
            _context.Groups.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public bool IsExists(int id)
        {
            return (_context.Groups?.Any(e => e.GroupId == id)).GetValueOrDefault();
        }
    }
}