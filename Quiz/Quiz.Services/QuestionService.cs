using Quiz.Data;
using Quiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly ApplicationDbContext context;

        public QuestionService(ApplicationDbContext applicationDbContext)
        {
            this.context = applicationDbContext;
        }

        public int Add(string title, int quizId)
        {
            var question = new Question()
            {
                Title = title,
                QuizId = quizId
            };

            context.Questions.Add(question);
            context.SaveChanges();

            return question.Id;
        }
    }
}
