using System.Text.Json.Serialization;

namespace Quiz.Services.InputModels
{

    public class JsonAnswer
    {
        [JsonPropertyName("Answer")]
        public string Title { get; set; }
        public bool Correct { get; set; }
    }
}