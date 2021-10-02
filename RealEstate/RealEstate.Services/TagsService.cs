using RealEstate.Data;
using RealEstate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public class TagsService : ITagsService
    {
        private readonly ApplicationDbContext context;
        private readonly IPropertiesService propertiesService;
        public TagsService(ApplicationDbContext context)
        {
            this.context = context;
            this.propertiesService = new PropertiesService(context);
        }

        public void Add(string name)
        {
            context.Tags.Add(new Tag { Name = name });
            context.SaveChanges();
        }

        public void BulkAssignTags()
        {
            var allPropertiesGroupedByDistrictId = context.Properties.ToList().GroupBy(x => x.DistrictId);
            Tag expensivePropertyTag = GetTag("скъп-имот");
            Tag cheapPropertyTag = GetTag("евтин-имот");

            Tag oldPropertyTag = GetTag("нов-имот");
            Tag newPropertyTag = GetTag("стар-имот");

            Tag bigPropertyTag = GetTag("голям-имот");
            Tag smallPropertyTag = GetTag("малък-имот");

            Tag firstFloorPropertyTag = GetTag("първи-етаж");
            Tag lastFloorPropertyTag = GetTag("последен-етаж");

            foreach (var group in allPropertiesGroupedByDistrictId)
            {
                decimal averagePricePerSquareMeterInDistrict = group.Average(x => x.Price / (decimal)x.Size) ?? 0;
                decimal averageSizeInDistrict = (decimal)group.Average(x => x.Size);

                foreach (var prop in group)
                {
                    if (prop.Price.HasValue)
                    {
                        if ((decimal)prop.Price / prop.Size >= averagePricePerSquareMeterInDistrict)
                        {
                            prop.Tags.Add(expensivePropertyTag);
                        }
                        else
                        {
                            prop.Tags.Add(cheapPropertyTag);
                        }
                    }

                    if (prop.Year.HasValue)
                    {
                        if (prop.Year >= DateTime.Now.Year - 15)
                        {
                            prop.Tags.Add(newPropertyTag);
                        }
                        else
                        {
                            prop.Tags.Add(oldPropertyTag);
                        }
                    }

                    if (prop.Size >= averageSizeInDistrict)
                    {
                        prop.Tags.Add(bigPropertyTag);
                    }
                    else
                    {
                        prop.Tags.Add(smallPropertyTag);
                    }

                    if (prop.Floor.HasValue)
                    {
                        if (prop.Floor.Value == 1)
                        {
                            prop.Tags.Add(firstFloorPropertyTag);
                        }
                        else if (prop.TotalFloors.HasValue && prop.Floor == prop.TotalFloors)
                        {
                            prop.Tags.Add(lastFloorPropertyTag);
                        }
                    }
                }
            }

            context.SaveChanges();
        }

        private Tag GetTag(string tagName)
        {
            return context.Tags.FirstOrDefault(x => x.Name == tagName) ?? new Tag { Name = tagName };
        }
    }
}
