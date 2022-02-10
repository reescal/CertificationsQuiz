using CertificationsQuiz.Shared;

namespace CertificationsQuiz.Features
{
    public class MappingProfile : Shared.MappingProfile
    {
        public MappingProfile()
        {
            CreateMap<Quiz, QuizUpsert.Command>();
            CreateMap<QuizUpsert.Command, PostQuiz>();
        }
    }
}