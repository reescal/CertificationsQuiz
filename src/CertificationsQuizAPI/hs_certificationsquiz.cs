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

namespace Headspring.CertificationsQuiz
{
    public class hs_certificationsquiz
    {
        private readonly ILogger<hs_certificationsquiz> _logger;

        public hs_certificationsquiz(ILogger<hs_certificationsquiz> log)
        {
            _logger = log;
        }

        [FunctionName("hs_certificationsquiz")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [CosmosDB(databaseName: "CertificationsQuiz", collectionName: "Quiz", ConnectionStringSetting = "CosmosDBConnection", SqlQuery = "SELECT * FROM c WHERE c.Type = 'Quiz'")]IEnumerable<Quiz> certs)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult(certs);
        }
    }
}

