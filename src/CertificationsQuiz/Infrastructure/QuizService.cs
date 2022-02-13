using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using CertificationsQuiz.Shared;

namespace CertificationsQuiz.Infrastructure
{
    public class QuizService : IQuizService
    {
        private readonly HttpClient _httpClient;

        public QuizService(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient("quiz");
        }

        public async Task<IEnumerable<Quiz>> Get()
        {
            var apiResponse = await _httpClient.GetStreamAsync("api/quiz");
            return await JsonSerializer.DeserializeAsync<IEnumerable<Quiz>>
                (apiResponse, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        }

        public async Task<Quiz> GetById(string id)
        {
            var apiResponse = await _httpClient.GetStreamAsync($"api/quiz/{id}");
            return await JsonSerializer.DeserializeAsync<Quiz>
                (apiResponse, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        }

        public async Task<Quiz> Upsert(string id, PostQuiz quiz)
        {
            var apiResponse = id == null ? await _httpClient.PostAsJsonAsync($"api/quiz", quiz) : await _httpClient.PostAsJsonAsync($"api/quiz?id={id}", quiz);

            if (!apiResponse.IsSuccessStatusCode)
                return null;

            var stream = await apiResponse.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<Quiz>
                    (stream, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        }

        public async Task Delete(string id) => await _httpClient.DeleteAsync($"api/quiz/{id}");

        public async Task<IEnumerable<Question>> GetQuizQuestions(string id)
        {
            var apiResponse = await _httpClient.GetStreamAsync($"api/quiz/{id}/questions");
            return await JsonSerializer.DeserializeAsync<IEnumerable<Question>>
                (apiResponse, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        }
    }
}
