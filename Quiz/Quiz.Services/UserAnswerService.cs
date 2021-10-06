using Microsoft.EntityFrameworkCore;
using Quiz.Data;
using Quiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Services
{
    public class UserAnswerService : IUserAnswerService
    {
        private readonly ApplicationDbContext context;
        public UserAnswerService(ApplicationDbContext applicationDbContext)
        {
            this.context = applicationDbContext;
        }
        public void AddUserAnswer(string username, int questionId, int answerId)
        {
            var userId = context.Users.FirstOrDefault(x => x.UserName == username).Id;

            var userAnswer = context.UserAnswers.FirstOrDefault(x => x.IdentityUserId == userId && x.QuestionId == questionId);
            userAnswer.AnswerId = answerId;

            context.SaveChanges();
        }

        public int GetUserResult(string username, int quizId)
        {
            string userId = context.Users.First(x => x.UserName == username).Id;
            return context.UserAnswers
                .Where(x => x.IdentityUserId == userId && x.Question.QuizId == quizId)
                .Where(x => x.Answer.IsCorrect == true)
                .Sum(x => x.Answer.Points);
        }
    }
}
