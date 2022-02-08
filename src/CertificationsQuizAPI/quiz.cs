using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using CertificationsQuiz.Shared;
using AutoMapper;

namespace Headspring.CertificationsQuiz
{
    public class quiz
    {
        private readonly ILogger<quiz> _logger;
        private readonly IMapper _mapper;

        public quiz(ILogger<quiz> log)
        {
            _logger = log;
        }

        [FunctionName("quiz")]
        [OpenApiOperation(operationId: "GetQuizzes", tags: new[] { "Quiz" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public IActionResult GetQuizzes(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [CosmosDB(databaseName: "CertificationsQuiz", collectionName: "Quiz", ConnectionStringSetting = "CosmosDBConnection", SqlQuery = "SELECT * FROM c WHERE c.Type = 'Quiz'")]IEnumerable<Quiz> quizzes)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult(quizzes);
        }

        [FunctionName("quizById")]
        [OpenApiOperation(operationId: "GetQuizById", tags: new[] { "Quiz" })]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The **Id** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public IActionResult GetQuizById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "quiz/{id}")] HttpRequest req,
            [CosmosDB(databaseName: "CertificationsQuiz", collectionName: "Quiz", ConnectionStringSetting = "CosmosDBConnection", Id = "{id}", PartitionKey = "{id}")]Quiz quizById,
            string id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if (quizById == null)
            {
                _logger.LogInformation($"Item {id} not found");
                return new NotFoundResult();
            }

            return new OkObjectResult(quizById);
        }
    }
}

