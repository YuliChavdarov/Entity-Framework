using Newtonsoft.Json;

namespace SoftJail.DataProcessor.ExportDto
{
    public class OfficerOutputModelJSON
    {
        [JsonProperty("OfficerName")]
        public string FullName { get; set; }
        [JsonProperty("Department")]
        public string DepartmentName { get; set; }
    }
}