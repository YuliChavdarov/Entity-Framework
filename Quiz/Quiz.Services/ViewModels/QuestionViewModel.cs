using System.Collections;
using System.Collections.Generic;

namespace Quiz.Services.ViewModels
{
    public class QuestionViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public IEnumerable<AnswerViewModel> Answers { get; set; }
    }
}