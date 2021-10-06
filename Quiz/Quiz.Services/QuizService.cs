using Microsoft.EntityFrameworkCore;
using Quiz.Data;
using Quiz.Models;
using Quiz.Services.InputModels;
using Quiz.Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quiz.Services
{
    public class QuizService : IQuizService
    {
        private readonly ApplicationDbContext context;
        public QuizService(ApplicationDbContext applicationDbContext)
        {
            this.context = applicationDbContext;
        }

        public int Add(string title)
        {
            var quiz = new Models.Quiz
            {
                Title = title
            };

            context.Quizzes.Add(quiz);
            context.SaveChanges();

            return quiz.Id;
        }

        public QuizViewModel GetQuizById(int quizId)
        {
            var quiz = context.Quizzes
                .Include(x => x.Questions)
                .ThenInclude(x => x.Answers)
                .FirstOrDefault(x => x.Id == quizId);

            var quizViewModel = new QuizViewModel
            {
                Id = quiz.Id,
                Title = quiz.Title,
                Questions = quiz.Questions
                .Select(question => new QuestionViewModel
                {
                    Id = question.Id,
                    Title = question.Title,
                    Answers = question.Answers.Select(answer => new AnswerViewModel
                    {
                        Id = answer.Id,
                        Title = answer.Title
                    })
                    .ToList()
                })
                .ToList()
            };

            return quizViewModel;
        }

        public void AddUserAnswers(QuizInputModel quizInputModel)
        {
            var userAnswers = new List<UserAnswer>();

            foreach (var question in quizInputModel.Questions)
            {
                var userAnswer = new UserAnswer
                {
                    IdentityUserId = quizInputModel.UserId,
                    AnswerId = question.AnswerId
                };

                userAnswers.Add(userAnswer);
            }

            context.UserAnswers.AddRange(userAnswers);
            context.SaveChanges();
        }

        public IEnumerable<UserQuizViewModel> GetQuizzesByUsername(string username)
        {
            var quizzes = context.Quizzes
                .Select(x => new UserQuizViewModel
                {
                    QuizId = x.Id,
                    Title = x.Title
                })
                .ToList();

            foreach (var quiz in quizzes)
            {
                var answersFromUserCount = context.UserAnswers
                    .Count(x => x.IdentityUser.UserName == username && quiz.QuizId == x.Question.QuizId && x.AnswerId != null);

                if(answersFromUserCount == 0)
                {
                    quiz.QuizStatus = QuizStatus.NotStarted;
                }
                else if (answersFromUserCount == context.Questions.Count(x => x.QuizId == quiz.QuizId))
                {
                    quiz.QuizStatus = QuizStatus.Finished;
                }
                else
                {
                    quiz.QuizStatus = QuizStatus.InProgress;
                }
            }

            return quizzes;
        }

        public void StartQuiz(string username, int quizId)
        {
            if(context.UserAnswers.Any(x => x.IdentityUser.UserName == username && x.Question.QuizId == quizId))
            {
                return;
            }
            else
            {
                var questions = context.Questions.Where(x => x.QuizId == quizId).ToList();
                foreach (var question in questions)
                {
                    context.UserAnswers.Add(new UserAnswer
                    {
                        AnswerId = null,
                        IdentityUserId = context.Users.First(x => x.UserName == username).Id,
                        QuestionId = question.Id
                    });
                }

                context.SaveChanges();
            }
        }
    }
}
