using RealEstate.Models;
using RealEstate.Services.DTO.OutputModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public interface IDistrictsService
    {
        IEnumerable<DistrictOutputModel> GetMostExpensiveDistricts(int count);
    }
}
