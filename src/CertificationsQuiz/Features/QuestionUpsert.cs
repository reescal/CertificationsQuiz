using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CertificationsQuiz.Infrastructure;
using CertificationsQuiz.Shared;
using MediatR;

namespace CertificationsQuiz.Features
{
    public class QuestionUpsert
    {
        public class Query : IRequest<Command>
        {
            public string Id { get; set; }
        }

        public class Command : Question, IRequest<Question> { }

        public class QueryHandler : IRequestHandler<Query, Command>
        {
            private readonly IQuestionService _questionService;
            private readonly IMapper _mapper;

            public QueryHandler(IQuestionService questionService, IMapper mapper)
            {
                _questionService = questionService;
                _mapper = mapper;
            }

            public async Task<Command> Handle(Query query, CancellationToken cancellationToken)
            {
                var quiz = await _questionService.GetById(query.Id);
                return _mapper.Map<Command>(quiz);
            }
        }

        public class CommandHandler : IRequestHandler<Command, Question>
        {
            private readonly IQuestionService _questionService;
            private readonly IMapper _mapper;

            public CommandHandler(IQuestionService questionService, IMapper mapper)
            {
                _questionService = questionService;
                _mapper = mapper;
            }

            public async Task<Question> Handle(Command message, CancellationToken cancellationToken)
                => await _questionService.Upsert(message.Id, _mapper.Map<PostQuestion>(message));
        }
    }
}