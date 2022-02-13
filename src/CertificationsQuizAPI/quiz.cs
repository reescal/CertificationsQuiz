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
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents.Client;
using System.Linq;

namespace Headspring.CertificationsQuiz
{
    public class quiz
    {
        private readonly ILogger<quiz> _logger;
        private readonly IMapper _mapper;
        private readonly Container _container;

        public quiz(ILogger<quiz> log, CosmosClient cosmosClient, IMapper mapper)
        {
            _logger = log;
            _mapper = mapper;
            _container = cosmosClient.GetContainer("CertificationsQuiz", "Items");
        }

        [FunctionName("quiz")]
        [OpenApiOperation(operationId: "GetQuizzes", tags: new[] { "Quiz" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(IEnumerable<Quiz>), Description = "The OK response")]
        public IActionResult GetQuizzes(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [CosmosDB(databaseName: "CertificationsQuiz", collectionName: "Items", ConnectionStringSetting = "CosmosDBConnection", SqlQuery = "SELECT * FROM c WHERE c.Type = 'Quiz'")]IEnumerable<Quiz> quizzes)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult(quizzes);
        }

        [FunctionName("quizById")]
        [OpenApiOperation(operationId: "GetQuizById", tags: new[] { "Quiz" })]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The **Id** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(Quiz), Description = "The OK response")]
        public IActionResult GetQuizById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "quiz/{id}")] HttpRequest req,
            [CosmosDB(databaseName: "CertificationsQuiz", collectionName: "Items", ConnectionStringSetting = "CosmosDBConnection", Id = "{id}", PartitionKey = "{id}")]Quiz quizById,
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

        [FunctionName("upsertQuiz")]
        [OpenApiOperation(operationId: "UpsertQuiz", tags: new[] { "Quiz" })]
        [OpenApiParameter(name: "id", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The **Id** parameter")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(PostQuiz), Description = "Quiz", Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Quiz), Description = "The OK response")]
        public async Task<IActionResult> UpsertQuiz(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "quiz/")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<PostQuiz>(requestBody);

            var mappedInput = _mapper.Map<SubmitQuiz>(input);
            ItemResponse<SubmitQuiz> response;

            string id = req.Query["id"];

            if (id == null)
            {
                mappedInput.id = Guid.NewGuid().ToString();
                response = await _container.CreateItemAsync<SubmitQuiz>(mappedInput, new PartitionKey(mappedInput.id));
            }
            else
            {
                mappedInput.id = id;
            
                try
                {
                    response = await _container.ReadItemAsync<SubmitQuiz>(id, new PartitionKey(id));
                }
                catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogInformation($"Item {id} not found");
                    return new NotFoundResult();
                }

                response = await _container.ReplaceItemAsync<SubmitQuiz>(mappedInput, id, new PartitionKey(id));
            }

            return new OkObjectResult(_mapper.Map<Quiz>(response.Resource));
        }

        [FunctionName("deleteQuiz")]
        [OpenApiOperation(operationId: "DeleteQuiz", tags: new[] { "Quiz" })]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The **Id** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public static async Task<IActionResult> DeleteQuiz(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "quiz/{id}")]HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "CosmosDBConnection")] DocumentClient client,
            string id)
        {
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("CertificationsQuiz", "Items");
            var document = client.CreateDocumentQuery(collectionUri).Where(t => t.Id == id)
                    .AsEnumerable().FirstOrDefault();

            if (document == null)
                return new NotFoundResult();
                
            await client.DeleteDocumentAsync(document.SelfLink, 
                new Microsoft.Azure.Documents.Client.RequestOptions { PartitionKey = new Microsoft.Azure.Documents.PartitionKey(document.Id) });
            return new OkResult();
        }
    }
}

