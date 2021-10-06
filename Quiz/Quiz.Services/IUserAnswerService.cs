namespace Quiz.Services
{
    public interface IUserAnswerService
    {
        public void AddUserAnswer(string username, int questionId, int answerId);
        public int GetUserResult(string username, int quizId);
    }
}