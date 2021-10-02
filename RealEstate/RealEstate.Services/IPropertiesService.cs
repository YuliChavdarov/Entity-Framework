using RealEstate.Services.DTO.OutputModels;
using System;
using System.Collections.Generic;

namespace RealEstate.Services
{
    public interface IPropertiesService
    {
        void Add(string url, int size, int yardSize, int floor, int totalFloors,
            string districtName, int year, string propertyType, string buildingType, int price);
        IEnumerable<PropertyOutputModel> Search(int minPrice, int maxPrice, int minSize, int maxSize);
        public decimal GetAveragePricePerSquareMeterOverall();
        public decimal GetAveragePricePerSquareMeterInDistrict(int districtId);
        public decimal GetAverageSizeOverall();
        public decimal GetAverageSizeInDistrict(int districtId);
    }
}
