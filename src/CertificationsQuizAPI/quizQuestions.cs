using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using CertificationsQuiz.Shared;
using AutoMapper;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;

namespace Headspring.CertificationsQuiz
{
    public class quizQuestions
    {
        private readonly ILogger<quiz> _logger;
        private readonly IMapper _mapper;
        private readonly Container _container;
        private readonly CosmosConfiguration _settings;

        public quizQuestions(ILogger<quiz> log, CosmosClient cosmosClient, IMapper mapper, IOptions<CosmosConfiguration> options)
        {
            _logger = log;
            _mapper = mapper;
            _settings = options.Value;
            _container = cosmosClient.GetContainer(_settings.Database, _settings.Container);
        }

        [FunctionName("questionById")]
        [OpenApiOperation(operationId: "GetQuestionById", tags: new[] { "Question" })]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The **Id** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(Question), Description = "The OK response")]
        public IActionResult GetQuizById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "question/{id}")] HttpRequest req,
            [CosmosDB(databaseName: "%CosmosConfiguration:Database%", collectionName: "%CosmosConfiguration:Container%", ConnectionStringSetting = "CosmosDBConnection", Id = "{id}", PartitionKey = "{id}")]Question questionById,
            string id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if (questionById == null)
            {
                _logger.LogInformation($"Item {id} not found");
                return new NotFoundResult();
            }

            return new OkObjectResult(questionById);
        } 

        [FunctionName("upsertQuestion")]
        [OpenApiOperation(operationId: "UpsertQuestion", tags: new[] { "Question" })]
        [OpenApiParameter(name: "id", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The **Id** parameter")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(PostQuestion), Description = "Question", Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Question), Description = "The OK response")]
        public async Task<IActionResult> UpsertQuiz(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "question/")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<PostQuestion>(requestBody);

            var mappedInput = _mapper.Map<SubmitQuestion>(input);

            try
            {
                await _container.ReadItemAsync<SubmitQuiz>(mappedInput.QuizId, new PartitionKey(mappedInput.QuizId));
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogInformation($"Item {mappedInput.QuizId} not found");
                return new NotFoundResult();
            }

            ItemResponse<SubmitQuestion> response;

            string id = req.Query["id"];

            if (id == null)
            {
                mappedInput.id = Guid.NewGuid().ToString();
                response = await _container.CreateItemAsync<SubmitQuestion>(mappedInput, new PartitionKey(mappedInput.id));
            }
            else
            {
                mappedInput.id = id;
            
                try
                {
                    response = await _container.ReadItemAsync<SubmitQuestion>(id, new PartitionKey(id));
                }
                catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogInformation($"Item {id} not found");
                    return new NotFoundResult();
                }

                response = await _container.ReplaceItemAsync<SubmitQuestion>(mappedInput, id, new PartitionKey(id));
            }

            return new OkObjectResult(_mapper.Map<Question>(response.Resource));
        }

        [FunctionName("deleteQuestion")]
        [OpenApiOperation(operationId: "DeleteQuestion", tags: new[] { "Question" })]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The **Id** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> DeleteQuestion(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "question/{id}")]HttpRequest req,
            [CosmosDB(databaseName: "%CosmosConfiguration:Database%", collectionName: "%CosmosConfiguration:Container%", ConnectionStringSetting = "CosmosDBConnection", Id = "{id}", PartitionKey = "{id}")]Question questionById,
            string id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if (questionById == null)
            {
                _logger.LogInformation($"Item {id} not found");
                return new NotFoundResult();
            }
            
            var response = await _container.DeleteItemAsync<Question>(questionById.Id, new PartitionKey(questionById.Id));

            return new OkResult();
        }
    }
}

