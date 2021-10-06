using Quiz.Services.InputModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Quiz.Services
{
    public class JsonImportService : IJsonImportService
    {
        private readonly IQuizService quizService;
        private readonly IQuestionService questionService;
        private readonly IAnswerService answerService;

        public JsonImportService(IQuizService quizService, IQuestionService questionService, IAnswerService answerService)
        {
            this.quizService = quizService;
            this.questionService = questionService;
            this.answerService = answerService;
        }
        public void Import(string fileName, string quizName)
        {
            var jsonQuestions = JsonSerializer.Deserialize<IEnumerable<JsonQuestion>>(File.ReadAllText($"{fileName}"));

            var quizId = quizService.Add(quizName);
            foreach (var question in jsonQuestions)
            {
                var questionId = questionService.Add(question.Title, quizId);
                foreach (var answer in question.Answers)
                {
                    answerService.Add(answer.Title, answer.Correct ? 1 : 0, answer.Correct, questionId);
                }
            }
        }
    }
}
