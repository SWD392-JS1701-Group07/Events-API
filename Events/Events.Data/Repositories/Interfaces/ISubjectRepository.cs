using Events.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Data.Repositories.Interfaces
{
    public interface ISubjectRepository
    {
        Task<Subject> GetSubjectById(int id);
        Task<Subject> GetSubjectByName(string name);
        Task<List<Subject>> GetAllSubjects();
        Task<bool> CreateSubject(Subject subject);  
        Task<bool> UpdateSubject(Subject subject);  
        
    }
}
