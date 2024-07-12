using Events.Data.Repositories.Interfaces;
using Events.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Data.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly EventsDbContext _context;

        public SubjectRepository(EventsDbContext context)
        {
            _context = context;
        }

        public async Task<List<Subject>> GetAllSubjects()
        {
            return await _context.Subjects.ToListAsync();
        }

        public async Task<Subject> GetSubjectById(int id)
        {
            return await _context.Subjects.FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}