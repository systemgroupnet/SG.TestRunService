using AutoMapper;
using SG.TestRunService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Dto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TestRunSessionDto, TestRunSession>().ReverseMap();
            CreateMap<TestRunDto, TestRun>().ReverseMap();
        }
    }
}
