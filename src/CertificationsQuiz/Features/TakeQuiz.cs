using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CertificationsQuiz.Infrastructure;
using MediatR;

namespace CertificationsQuiz.Features
{
    public class TakeQuiz
    {
        public class Query : IRequest<ViewModel>
        {
            public string Ids { get; set; }
        }

        public class ViewModel : IRequest
        {
            public IEnumerable<string> QuestionIds { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, ViewModel>
        {
            private readonly IQuizService _quizService;

            public QueryHandler(IQuizService quizService)
            {
                _quizService = quizService;
            }

            public async Task<ViewModel> Handle(Query query, CancellationToken cancellationToken)
            {
                var ids = query.Ids.Split("&").AsEnumerable();
                var questions = await _quizService.GetQuizzesQuestions(ids);
                return new ViewModel { QuestionIds = questions };
            }
        }
    }
}