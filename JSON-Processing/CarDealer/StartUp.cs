using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        private static readonly MapperConfiguration config = new MapperConfiguration(cfg => cfg.AddProfile(new CarDealerProfile()));
        private static readonly IMapper mapper = config.CreateMapper();

        public static void Main(string[] args)
        {
            CarDealerContext context = new CarDealerContext();
            string json;

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //json = File.ReadAllText("../../../Datasets/suppliers.json");
            //Console.WriteLine(ImportSuppliers(context, json));
            //json = File.ReadAllText("../../../Datasets/parts.json");
            //Console.WriteLine(ImportParts(context, json));
            //json = File.ReadAllText("../../../Datasets/cars.json");
            //Console.WriteLine(ImportCars(context, json));
            //json = File.ReadAllText("../../../Datasets/customers.json");
            //Console.WriteLine(ImportCustomers(context, json));
            //json = File.ReadAllText("../../../Datasets/sales.json");
            //Console.WriteLine(ImportSales(context, json));

            Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliersInputModel = JsonConvert.DeserializeObject<IEnumerable<SupplierInputModel>>(inputJson);

            var suppliers = mapper.Map<IEnumerable<Supplier>>(suppliersInputModel);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count()}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var partsInputModel = JsonConvert.DeserializeObject<IEnumerable<PartInputModel>>(inputJson);

            var parts = mapper.Map<IEnumerable<Part>>(partsInputModel);

            context.Parts.AddRange(parts.Where(x => context.Suppliers.Any(y => y.Id == x.SupplierId)));

            return $"Successfully imported {context.SaveChanges()}.";
        }
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var carsInputModel = JsonConvert.DeserializeObject<IEnumerable<CarInputModel>>(inputJson);

            var cars = new List<Car>();

            foreach (var car in carsInputModel)
            {
                Car toAdd = new Car();
                toAdd.Make = car.Make;
                toAdd.Model = car.Model;
                toAdd.TravelledDistance = car.TravelledDistance;

                foreach (var part in car?.PartsId.Distinct())
                {
                    toAdd.PartCars.Add(new PartCar { PartId = part });
                }

                cars.Add(toAdd);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customerInputModels = JsonConvert.DeserializeObject<IEnumerable<CustomerInputModel>>(inputJson);

            var customers = mapper.Map<IEnumerable<Customer>>(customerInputModels);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count()}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var salesInputModels = JsonConvert.DeserializeObject<IEnumerable<SalesInputModel>>(inputJson);

            var sales = mapper.Map<IEnumerable<Sale>>(salesInputModels);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count()}.";
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(x => x.BirthDate)
                .ThenBy(x => x.IsYoungDriver)
                .Select(x => new
                {
                    x.Name,
                    BirthDate = x.BirthDate.ToString("dd/MM/yyyy"),
                    x.IsYoungDriver
                })
                .ToList();

            return JsonConvert.SerializeObject(customers, Formatting.Indented);
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(x => x.Make == "Toyota")
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .Select(x => new
                {
                    x.Id,
                    x.Make,
                    x.Model,
                    x.TravelledDistance
                })
                .ToList();

            return JsonConvert.SerializeObject(cars, Formatting.Indented);
        }


        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(x => x.IsImporter == false)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    PartsCount = x.Parts.Count
                })
                .ToList();

            return JsonConvert.SerializeObject(suppliers, Formatting.Indented);
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(x => new
                {
                    car = new
                    {
                        x.Make,
                        x.Model,
                        x.TravelledDistance,
                    },
                    parts = x.PartCars
                    .Select(y => new
                    {
                        y.Part.Name,
                        Price = y.Part.Price.ToString("0.00")
                    })
                    .ToList()
                })
                .ToList();

            return JsonConvert.SerializeObject(cars, Formatting.Indented);
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(x => x.Sales.Any())
                .Select(x => new
                {
                    FullName = x.Name,
                    BoughtCars = x.Sales.Count,
                    SpentMoney = x.Sales.Sum(y => y.Car.PartCars.Sum(z => z.Part.Price))
                })
                .OrderByDescending(x => x.SpentMoney)
                .ThenBy(x => x.BoughtCars)
                .ToList();

            return JsonConvert.SerializeObject(customers, Formatting.Indented, 
                new JsonSerializerSettings(){ ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Select(x => new
                {
                    car = new
                    {
                        x.Car.Make,
                        x.Car.Model,
                        x.Car.TravelledDistance
                    },
                    customerName = x.Customer.Name,
                    Discount = x.Discount.ToString("0.00"),
                    price = x.Car.PartCars.Sum(x => x.Part.Price).ToString("0.00"),
                    priceWithDiscount = (x.Car.PartCars.Sum(x => x.Part.Price) - (x.Car.PartCars.Sum(x => x.Part.Price) * (x.Discount / 100))).ToString("0.00"),
                })
                .Take(10)
                .ToList();

            return JsonConvert.SerializeObject(sales, Formatting.Indented);
        }
    }
}