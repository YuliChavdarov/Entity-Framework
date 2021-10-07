using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftJail.DataProcessor.ExportDto
{
    public class PrisonerOutputModelJSON
    {
        public int Id { get; set; }
        [JsonProperty("Name")]
        public string FullName { get; set; }
        public int CellNumber { get; set; }
        public IEnumerable<OfficerOutputModelJSON> Officers { get; set; }
        public decimal TotalOfficerSalary { get; set; }
    }
}
