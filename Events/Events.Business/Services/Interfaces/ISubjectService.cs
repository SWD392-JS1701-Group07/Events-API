using Events.Models.DTOs.Request;
using Events.Models.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Business.Services.Interfaces
{
    public interface ISubjectService
    {
        Task<BaseResponse> GetAllSubjects();
        Task<BaseResponse> GetSubjectById(int id);
        Task<BaseResponse> CreateSubject(CreateSubjectDTO createSubject);
        Task<BaseResponse> UpdateSubject(int id, CreateSubjectDTO createSubject);
    }
}
