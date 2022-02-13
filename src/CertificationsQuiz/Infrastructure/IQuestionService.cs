using System.Threading.Tasks;
using CertificationsQuiz.Shared;

namespace CertificationsQuiz.Infrastructure
{
    public interface IQuestionService
    {
        Task<Question> GetById(string id);
        Task<Question> Upsert(string id, PostQuestion question);
        Task Delete(string id);
    }
}