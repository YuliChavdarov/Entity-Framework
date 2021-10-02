using RealEstate.Data;
using RealEstate.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace RealEstate.Importer
{
    class Program
    {
        static void Main(string[] args)
        {
            ImportPropertiesFromJsonFile(@"imot.bg-raw-data-2021-03-18.json");
        }

        private static void ImportPropertiesFromJsonFile(string path)
        {
            var context = new ApplicationDbContext();
            IPropertiesService propertiesService = new PropertiesService(context);

            var propertiesAsJson = JsonSerializer.Deserialize<List<PropertyAsJson>>(File.ReadAllText(path));

            foreach (var p in propertiesAsJson)
            {
                propertiesService.Add(p.Url, p.Size, p.YardSize, p.Floor, p.TotalFloors, p.DistrictName, p.Year, p.PropertyType, p.BuildingType, p.Price);
            }
        }
    }
}
