using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using CertificationsQuiz.Features;
using CertificationsQuiz.Infrastructure;
using MediatR;

namespace CertificationsQuiz
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            //builder.Services.AddHttpClient<IQuizService, QuizService>(x => x.BaseAddress = new Uri("http://localhost:7071"));

            builder.Services.AddHttpClient("quiz", x => x.BaseAddress = new Uri(builder.Configuration["QuizAPIPrefix"]));

            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

            builder.Services.AddScoped<IQuizService, QuizService>();
            builder.Services.AddScoped<IQuestionService, QuestionService>();

            var assembly = Assembly.GetExecutingAssembly();
            builder.Services.AddMediatR(assembly);

            await builder.Build().RunAsync();
        }
    }
}
