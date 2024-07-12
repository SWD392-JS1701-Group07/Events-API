using AutoMapper;
using Events.Business.Services.Interfaces;
using Events.Data.Repositories.Interfaces;
using Events.Models.DTOs;
using Events.Models.DTOs.Response;
using Events.Models.Models;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Business.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly IMapper _mapper;

        public SubjectService(ISubjectRepository subjectRepository,
                              IMapper mapper)
        {
            _subjectRepository = subjectRepository;
            _mapper = mapper;
        }
        public async Task<BaseResponse> GetAllSubjects()
        {
            var subjects = await _subjectRepository.GetAllSubjects();

            var subjectsDTO = _mapper.Map<List<SubjectDTO>>(subjects);

            return new BaseResponse
            {
                StatusCode = 200,
                IsSuccess = true,
                Data = subjectsDTO,
                Message = string.Empty
            };
        }

        public async Task<BaseResponse> GetSubjectById(int id)
        {
            var subject = await _subjectRepository.GetSubjectById(id);

            if(subject == null)
            {
                return new BaseResponse
                {
                    StatusCode = 404,
                    IsSuccess = false,
                    Data = null,
                    Message = "Subject unfound"
                };
            }
            else
            {
                var subjectsDTO = _mapper.Map<SubjectDTO>(subject);
                return new BaseResponse
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Data = subjectsDTO,
                    Message = string.Empty
                };
            }
        }
    }
}
