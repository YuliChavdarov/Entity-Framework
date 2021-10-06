using Quiz.Services.ViewModels;
using System.Collections.Generic;

namespace Quiz.Services
{
    public interface IQuizService
    {
        public int Add(string title);
        public QuizViewModel GetQuizById(int quizId);
        public IEnumerable<UserQuizViewModel> GetQuizzesByUsername(string username);
        public void StartQuiz(string username, int quizId);
    }
}