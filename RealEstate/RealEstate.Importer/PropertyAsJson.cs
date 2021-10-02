using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace RealEstate.Importer
{
    public class PropertyAsJson
    {
        public string Url { get; set; }
        public int Size { get; set; }
        public int YardSize { get; set; }
        public int Floor { get; set; }
        public int TotalFloors { get; set; }
        [JsonPropertyName("District")]
        public string DistrictName { get; set; }
        public int Year { get; set; }
        [JsonPropertyName("Type")]
        public string PropertyType { get; set; }
        public string BuildingType { get; set; }
        public int Price { get; set; }
    }
}
