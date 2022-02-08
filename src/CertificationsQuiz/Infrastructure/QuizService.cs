using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CertificationsQuiz.Shared;

namespace CertificationsQuiz.Infrastructure
{
    public class QuizService : IQuizService
    {
        private readonly HttpClient _httpClient;

        public QuizService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Quiz>> Get()
        {
            var apiResponse = await _httpClient.GetStreamAsync($"api/hs_certificationsquiz");
            return await JsonSerializer.DeserializeAsync<IEnumerable<Quiz>>
                (apiResponse, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        }
    }
}
