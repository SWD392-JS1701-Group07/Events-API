using AutoMapper;
using Events.Business.Services.Interfaces;
using Events.Data.Repositories.Interfaces;
using Events.Models.DTOs;
using Events.Models.DTOs.Request;
using Events.Models.DTOs.Response;
using Events.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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

        public async Task<BaseResponse> CreateSubject(CreateSubjectDTO createSubject)
        {
            if (createSubject.Name.Trim().IsNullOrEmpty())
            {
                return new BaseResponse
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Data = null,
                    Message = "Subject can not be null"
                };
            }

            var subjectExist = await _subjectRepository.GetSubjectByName(createSubject.Name);

            if(subjectExist != null)
            {
                return new BaseResponse
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Data = null,
                    Message = "Subject is already existed"
                };
            }
            else
            {
                Subject subjectCreate = new Subject
                {
                    Name = createSubject.Name,
                    Description = createSubject.Description,
                };

                var subjectResult = await _subjectRepository.CreateSubject(subjectCreate);

                if (subjectResult)
                {
                    return new BaseResponse
                    {
                        StatusCode = 200,
                        IsSuccess = true,
                        Data = subjectCreate,
                        Message = "Create successfully"
                    };
                }
                else
                {
                    return new BaseResponse
                    {
                        StatusCode = 500,
                        IsSuccess = false,
                        Data = null,
                        Message = "Create subject failed"
                    };
                }
            }
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

        public async Task<BaseResponse> UpdateSubject(int id, CreateSubjectDTO createSubject)
        {
            var subject = await _subjectRepository.GetSubjectById(id);
            if(subject == null)
            {
                return new BaseResponse
                {
                    StatusCode = 404,
                    IsSuccess = false,
                    Data = null,
                    Message = "Subject not found"
                };
            }
            else
            {
                if (createSubject.Name.Trim().IsNullOrEmpty())
                {
                    return new BaseResponse
                    {
                        StatusCode = 500,
                        IsSuccess = false,
                        Data = null,
                        Message = "Subject can not be null"
                    };
                }

                var subjectExist = await _subjectRepository.GetSubjectByName(createSubject.Name);

                if (subjectExist != null && subjectExist.Id != id)
                {
                    return new BaseResponse
                    {
                        StatusCode = 500,
                        IsSuccess = false,
                        Data = null,
                        Message = "Subject is already existed"
                    };
                }
                else
                {
                    subject.Name = createSubject.Name;
                    subject.Description = createSubject.Description;

                    var result = await _subjectRepository.UpdateSubject(subject);

                    if (result)
                    {
                        return new BaseResponse
                        {
                            StatusCode = 200,
                            IsSuccess = true,
                            Data = result,
                            Message = "Update successfully"
                        };
                    }
                    else
                    {
                        return new BaseResponse
                        {
                            StatusCode = 500,
                            IsSuccess = false,
                            Data = null,
                            Message = "Update subject failed"
                        };
                    }
                }
            }
        }
    }
}
