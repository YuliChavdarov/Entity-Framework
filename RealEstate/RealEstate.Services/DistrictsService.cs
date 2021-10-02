using RealEstate.Data;
using RealEstate.Services.DTO.OutputModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public class DistrictsService : IDistrictsService
    {
        private readonly ApplicationDbContext context;
        public DistrictsService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<DistrictOutputModel> GetMostExpensiveDistricts(int count)
        {
            var districts = context.Districts
                .Select(x => new DistrictOutputModel
                {
                    Name = x.Name,
                    PropertiesCount = x.Properties.Count,
                    AveragePricePerSquareMeter = x.Properties.Where(x => x.Price.HasValue).Average(x => x.Price / (decimal)x.Size) ?? 0
                })
                .OrderByDescending(x => x.AveragePricePerSquareMeter)
                .Take(count)
                .ToList();

            return districts;
        }
    }
}
