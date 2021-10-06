namespace Quiz.Services
{
    public interface IAnswerService
    {
        public int Add(string title, int points, bool isCorrect, int questionId);
    }
}