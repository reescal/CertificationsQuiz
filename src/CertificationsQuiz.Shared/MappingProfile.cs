using AutoMapper;

namespace CertificationsQuiz.Shared
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SubmitQuiz, Quiz>();
            CreateMap<PostQuiz, SubmitQuiz>();

            CreateMap<SubmitQuestion, Question>();
            CreateMap<PostQuestion, SubmitQuestion>();
        }
    }
}