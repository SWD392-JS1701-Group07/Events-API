using CloudinaryDotNet;
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

        public async Task<bool> CreateSubject(Subject subject)
        {
            await _context.Subjects.AddAsync(subject);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Subject>> GetAllSubjects()
        {
            return await _context.Subjects.ToListAsync();
        }

        public async Task<Subject> GetSubjectById(int id)
        {
            return await _context.Subjects.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Subject> GetSubjectByName(string name)
        {
            return await _context.Subjects.FirstOrDefaultAsync(e => e.Name.Trim().ToLower() == name.Trim().ToLower());
        }

        public async Task<bool> UpdateSubject(Subject subject)
        {
            _context.Entry(subject).State = EntityState.Modified;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}