using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Headspring.CertificationsQuiz
{
    public static class Helper
    {
        public static async Task<List<T>> GetQueryResultsAsync<T>(Container _container, string query)
        {
            List<T> items = new List<T>();

            QueryDefinition queryDefinition = new QueryDefinition(query);

            var feedIterator = _container.GetItemQueryIterator<T>(queryDefinition);

            while (feedIterator.HasMoreResults)
                foreach (var result in await feedIterator.ReadNextAsync())
                    items.Add(result);
            
            return items;
        }
    }
}