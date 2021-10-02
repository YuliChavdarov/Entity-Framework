using RealEstate.Data;
using RealEstate.Models;
using RealEstate.Services.DTO.OutputModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public class PropertiesService : IPropertiesService
    {
        private readonly ApplicationDbContext context;
        public PropertiesService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Add(string url, int size, int yardSize, int floor, int totalFloors,
            string districtName, int year, string propertyType, string buildingType, int price)
        {
            Property toAdd = new Property
            {
                Url = url,
                Size = size,
                YardSize = yardSize == 0 ? null : yardSize,
                Floor = floor == 0 ? null : floor,
                TotalFloors = totalFloors == 0 ? null : totalFloors,
                Year = year == 0 ? null : year,
                Price = price == 0 ? null : price
            };

            var district = context.Districts.FirstOrDefault(x => x.Name == districtName);
            if (district == null)
            {
                district = new District { Name = districtName };
            }
            toAdd.District = district;

            var propType = context.PropertyTypes.FirstOrDefault(x => x.Name == propertyType);
            if (propType == null)
            {
                propType = new PropertyType { Name = propertyType };
            }
            toAdd.PropertyType = propType;

            var buildType = context.BuildingTypes.FirstOrDefault(x => x.Name == buildingType);
            if (buildType == null)
            {
                buildType = new BuildingType { Name = buildingType };
            }
            toAdd.BuildingType = buildType;

            context.Properties.Add(toAdd);

            context.SaveChanges();
        }

        public decimal GetAveragePricePerSquareMeterOverall()
        {
            return context.Properties
                .Average(x => x.Price / (decimal)x.Size) ?? 0;
        }

        public decimal GetAveragePricePerSquareMeterInDistrict(int districtId)
        {
            if (context.Districts.FirstOrDefault(x => x.Id == districtId) == null)
            {
                throw new ArgumentException($"District with id {districtId} was not found.");
            }

            return context.Properties
                .Where(x => x.District.Id == districtId)
                .Average(x => x.Price / (decimal)x.Size) ?? 0;
        }

        public decimal GetAverageSizeOverall()
        {
            return (decimal)context.Properties.Average(x => x.Size);
        }

        public decimal GetAverageSizeInDistrict(int districtId)
        {
            if(context.Districts.Any(x => x.Id == districtId) == false)
            {
                throw new ArgumentException($"District with id {districtId} was not found.");
            }

            return (decimal)context.Properties
                .Where(x => x.District.Id == districtId)
                .Average(x => x.Size);
        }

        public IEnumerable<PropertyOutputModel> Search(int minPrice, int maxPrice, int minSize, int maxSize)
        {
            var properties = context.Properties
                .Where(x => x.Price >= minPrice && x.Price <= maxPrice && x.Size >= minSize && x.Size <= maxSize)
                .Select(x => new PropertyOutputModel
                {
                     Url = x.Url,
                     Size = x.Size,
                     YardSize = x.YardSize,
                     Floor = x.Floor,
                     TotalFloors = x.TotalFloors,
                     DistrictName = x.District.Name,
                     Year = x.Year,
                     PropertyType = x.PropertyType.Name,
                     BuildingType = x.BuildingType.Name,
                     Price = x.Price ?? 0
                })
                .ToList();
            return properties;
        }
    }
}
