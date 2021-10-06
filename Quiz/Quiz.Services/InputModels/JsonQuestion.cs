using System.Text.Json.Serialization;

namespace Quiz.Services.InputModels
{
    public class JsonQuestion
    {
        [JsonPropertyName("Question")]
        public string Title { get; set; }
        public JsonAnswer[] Answers { get; set; }
    }
}