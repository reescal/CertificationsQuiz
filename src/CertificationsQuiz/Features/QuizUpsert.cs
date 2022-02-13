using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CertificationsQuiz.Infrastructure;
using CertificationsQuiz.Shared;
using MediatR;

namespace CertificationsQuiz.Features
{
    public class QuizUpsert
    {
        public class Query : IRequest<Command>
        {
            public string Id { get; set; }
        }

        public class Command : Quiz, IRequest<Quiz> { }

        public class QueryHandler : IRequestHandler<Query, Command>
        {
            private readonly IQuizService _quizService;
            private readonly IMapper _mapper;

            public QueryHandler(IQuizService quizService, IMapper mapper)
            {
                _quizService = quizService;
                _mapper = mapper;
            }

            public async Task<Command> Handle(Query query, CancellationToken cancellationToken)
            {
                var quiz = await _quizService.GetById(query.Id);
                return _mapper.Map<Command>(quiz);
            }
        }

        public class CommandHandler : IRequestHandler<Command, Quiz>
        {
            private readonly IQuizService _quizService;
            private readonly IMapper _mapper;
        
            public CommandHandler(IQuizService quizService, IMapper mapper)
            {
                _quizService = quizService;
                _mapper = mapper;
            }
        
            public async Task<Quiz> Handle(Command message, CancellationToken cancellationToken) 
                => await _quizService.Upsert(message.Id, _mapper.Map<PostQuiz>(message));
        }
    }
}