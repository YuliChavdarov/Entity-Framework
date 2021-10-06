namespace Quiz.Services
{
    public interface IJsonImportService
    {
        public void Import(string fileName, string quizName);
    }
}