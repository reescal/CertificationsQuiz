using System.Collections.Generic;
using System.Threading.Tasks;
using CertificationsQuiz.Shared;

namespace CertificationsQuiz.Infrastructure
{
    public interface IQuizService
    {
        Task<IEnumerable<Quiz>> Get();
        Task<Quiz> GetById(string id);
        Task<Quiz> Upsert(string id, PostQuiz quiz);
        Task Delete(string id);

        Task<IEnumerable<Question>> GetQuizQuestions(string id);
    }
}
