using ASP.NETCoreMVCAPP.Models;
using Microsoft.EntityFrameworkCore;

namespace ASP.NETCoreMVCAPP.Data
{
    public class StudentRepository : IStudentRepository
    {
        private readonly UniversityContext _context;
        public StudentRepository(UniversityContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _context.Students.Include(g => g.Group).ToListAsync();
        }

        public async Task<Student> GetByIdAsync(int id)
        {
            return await _context.Students.FindAsync(id);
        }

        public async Task<Student> GetStudentWithGroupByIdAsync(int id)
        {
            return await _context.Students.Include(g => g.Group).FirstOrDefaultAsync(s => s.StudentId == id);
        }

        public async Task<IEnumerable<Student>> GetStudentsWithGroupForGroupAsync(int groupId)
        {
            return await _context.Students.Include(g => g.Group).Where(g => g.GroupId == groupId).ToListAsync();
        }

        public IEnumerable<Group> GetGroups()
        {
            return _context.Groups.ToList();
        }

        public async Task CreateAsync(Student entity)
        {
            _context.Students.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Student entity)
        {
            _context.Students.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Student entity)
        {
            _context.Students.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public bool IsExists(int id)
        {
            return (_context.Students?.Any(e => e.StudentId == id)).GetValueOrDefault();
        }
    }
}
