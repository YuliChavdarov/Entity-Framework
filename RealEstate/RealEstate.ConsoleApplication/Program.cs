using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Services;
using System;

namespace RealEstate.ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            ApplicationDbContext context = new ApplicationDbContext();



            ITagsService tagsService = new TagsService(context);
            
            //tagsService.BulkAssignTags();

            Console.OutputEncoding = System.Text.Encoding.Unicode;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Choose an option");
                Console.WriteLine("1. Property search");
                Console.WriteLine("2. Most expensive districts");

                bool parsed = int.TryParse(Console.ReadLine(), out int option);
                if(parsed == true && option >= 1 && option <= 2)
                {
                    switch (option)
                    {
                        case 1:
                            PropertySearch(context);
                            break;
                        case 2:
                            GetMostExpensiveDistricts(context);
                            break;
                        default:
                            break;
                    }
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        private static void PropertySearch(ApplicationDbContext context)
        {
            Console.WriteLine("Min price: ");
            int minPrice = int.Parse(Console.ReadLine());
            Console.WriteLine("Max price: ");
            int maxPrice = int.Parse(Console.ReadLine());
            Console.WriteLine("Min size: ");
            int minSize = int.Parse(Console.ReadLine());
            Console.WriteLine("Max size: ");
            int maxSize = int.Parse(Console.ReadLine());

            IPropertiesService service = new PropertiesService(context);
            var properties = service.Search(minPrice, maxPrice, minSize, maxSize);
            foreach (var property in properties)
            {
                Console.WriteLine($"{property.DistrictName}; {property.BuildingType}; {property.PropertyType}; {property.Size} ;{property.Price}€");
            }
        }

        private static void GetMostExpensiveDistricts(ApplicationDbContext context)
        {
            Console.WriteLine("Top count:");
            int count = int.Parse(Console.ReadLine());

            IDistrictsService service = new DistrictsService(context);
            var districts = service.GetMostExpensiveDistricts(count);
            foreach (var district in districts)
            {
                Console.WriteLine($"{district.Name}; {district.PropertiesCount}; {district.AveragePricePerSquareMeter:0.00}€");
            }
        }
    }
}
