using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using CertificationsQuiz.Shared;

namespace CertificationsQuiz.Infrastructure
{
    public class QuestionService : IQuestionService
    {
        private readonly HttpClient _httpClient;

        public QuestionService(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient("quiz");
        }

        public async Task<Question> GetById(string id)
        {
            var apiResponse = await _httpClient.GetStreamAsync($"api/question/{id}");
            return await JsonSerializer.DeserializeAsync<Question>
                (apiResponse, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        }

        public async Task<Question> Upsert(string id, PostQuestion question)
        {
            var apiResponse = id == null ? await _httpClient.PostAsJsonAsync($"api/question", question) : await _httpClient.PostAsJsonAsync($"api/question/?id={id}", question);

            if (!apiResponse.IsSuccessStatusCode)
                return null;

            var stream = await apiResponse.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<Question>
                (stream, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        }

        public async Task Delete(string id) => await _httpClient.DeleteAsync($"api/question/{id}");
    }
}