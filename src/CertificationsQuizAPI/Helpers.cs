using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;

namespace Headspring.CertificationsQuiz
{
    public static class Helpers
    {
        public static async Task<IActionResult> Delete(DocumentClient client, string id)
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