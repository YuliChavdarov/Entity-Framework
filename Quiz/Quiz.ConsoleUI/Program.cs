using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quiz.Data;
using Quiz.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Quiz.ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var context = serviceProvider.GetService<ApplicationDbContext>();

            //context.Database.EnsureDeleted();
            //context.Database.Migrate();

            var jsonimportService = serviceProvider.GetService<IJsonImportService>();

            //jsonimportService.Import("EF-Core-Quiz.json", "EF-Core-Quiz-v2");
            
            var quizService = serviceProvider.GetService<IQuizService>();
            var questionService = serviceProvider.GetService<IQuestionService>();
            var answerService = serviceProvider.GetService<IAnswerService>();
            var userAnswerService = serviceProvider.GetService<IUserAnswerService>();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            services.AddDbContext<ApplicationDbContext>(options 
                => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddTransient<IQuizService, QuizService>();
            services.AddTransient<IQuestionService, QuestionService>();
            services.AddTransient<IAnswerService, AnswerService>();
            services.AddTransient<IUserAnswerService, UserAnswerService>();
            services.AddTransient<IJsonImportService, JsonImportService>();
        }
    }
}
