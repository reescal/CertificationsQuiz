using System.Collections.Generic;
using System.Threading.Tasks;
using CertificationsQuiz.Shared;

namespace CertificationsQuiz.Infrastructure
{
    interface IQuizService
    {
        Task<IEnumerable<Quiz>> Get();
    }
}
