using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quiz.Services;
using Quiz.Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quiz.Web.Controllers
{
    [Authorize]
    public class QuizController : Controller
    {
        private readonly IQuizService quizService;
        private readonly IUserAnswerService userAnswerService;

        public QuizController(IQuizService quizService, IUserAnswerService userAnswerService)
        {
            this.quizService = quizService;
            this.userAnswerService = userAnswerService;
        }

        public IActionResult Test(int quizId)
        {
            quizService.StartQuiz(User.Identity.Name, quizId);
            QuizViewModel quiz = quizService.GetQuizById(quizId);

            return View(quiz);
        }

        public IActionResult Results(int quizId)
        {
            var points = userAnswerService.GetUserResult(User.Identity.Name, quizId);
            return View(points);
        }

        public IActionResult Submit(int quizId)
        {
            foreach (var item in Request.Form)
            {
                var questionId = int.Parse(item.Key.Replace("q_", string.Empty));
                var answerId = int.Parse(item.Value);
                userAnswerService.AddUserAnswer(User.Identity.Name, questionId, answerId);
            }

            return RedirectToAction("Results", new { quizId });
        }
    }
}
