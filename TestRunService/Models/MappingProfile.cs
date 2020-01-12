using AutoMapper;
using SG.TestRunService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TestRunSessionRequest, TestRunSession>();
            CreateMap<TestRunSession, TestRunSessionResponse>()
                .ForMember(dto => dto.TestRunCount,
                    cfg =>
                    {
                        cfg.MapFrom(s => s.TestRuns.Count());
                    });

            CreateMap<TestRun, TestRunDto>()
                .ReverseMap();
        }
    }
}
