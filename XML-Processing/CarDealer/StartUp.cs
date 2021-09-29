using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.DTO.OutputModels;
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
            string xml;

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //xml = File.ReadAllText("../../../Datasets/suppliers.xml");
            //Console.WriteLine(ImportSuppliers(context, xml));
            //xml = File.ReadAllText("../../../Datasets/parts.xml");
            //Console.WriteLine(ImportParts(context, xml));
            //xml = File.ReadAllText("../../../Datasets/cars.xml");
            //Console.WriteLine(ImportCars(context, xml));
            //xml = File.ReadAllText("../../../Datasets/customers.xml");
            //Console.WriteLine(ImportCustomers(context, xml));
            //xml = File.ReadAllText("../../../Datasets/sales.xml");
            //Console.WriteLine(ImportSales(context, xml));

            Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var suppliersInputModel = XmlConverter.Deserializer<SupplierInputModel>(inputXml, "Suppliers");

            var suppliers = mapper.Map<IEnumerable<Supplier>>(suppliersInputModel);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count()}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var partsInputModel = XmlConverter.Deserializer<PartInputModel>(inputXml, "Parts");

            var parts = mapper.Map<IEnumerable<Part>>(partsInputModel);

            var toImport = parts.Where(x => context.Suppliers.Any(y => y.Id == x.SupplierId)).ToList();

            context.Parts.AddRange(toImport);
            context.SaveChanges();

            return $"Successfully imported {toImport.Count()}";
        }
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var carsInputModel = XmlConverter.Deserializer<CarInputModel>(inputXml, "Cars");

            var cars = new List<Car>();

            foreach (var car in carsInputModel)
            {
                Car toAdd = new Car();
                toAdd.Make = car.Make;
                toAdd.Model = car.Model;
                toAdd.TravelledDistance = car.TravelledDistance;

                foreach (var part in car?.PartsId.Select(x => x.PartId).Distinct())
                {
                    toAdd.PartCars.Add(new PartCar { PartId = part });
                }

                cars.Add(toAdd);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count()}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            var customerInputModels = XmlConverter.Deserializer<CustomerInputModel>(inputXml, "Customers");

            var customers = mapper.Map<IEnumerable<Customer>>(customerInputModels);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count()}";
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            var salesInputModels = XmlConverter.Deserializer<SalesInputModel>(inputXml, "Sales");

            var sales = mapper.Map<IEnumerable<Sale>>(salesInputModels);

            var toImport = sales.Where(x => context.Cars.Any(y => y.Id == x.CarId));

            context.Sales.AddRange(toImport);
            context.SaveChanges();

            return $"Successfully imported {toImport.Count()}";
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(x => x.TravelledDistance > 2_000_000)
                .OrderBy(x => x.Make)
                .ThenBy(x => x.Model)
                .Take(10)
                .Select(x => new CarOutputModel
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .ToList();

            return XmlConverter.Serialize(cars, "cars").ToString();
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(x => x.Make == "BMW")
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .Select(x => new CarAttributeOutputModel
                {
                    Id = x.Id.ToString(),
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .ToList();

            return XmlConverter.Serialize(cars, "cars").ToString();
        }


        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(x => x.IsImporter == false)
                .Select(x => new SupplierAttributeOutputModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartsCount = x.Parts.Count()
                })
                .ToList();

            return XmlConverter.Serialize(suppliers, "suppliers").ToString();
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(x => new CarAttributeOutputModel
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance,
                    Parts = x.PartCars
                    .Select(y => new PartOutputModel
                    {
                        Name = y.Part.Name,
                        Price = y.Part.Price
                    })
                    .OrderByDescending(x => x.Price)
                    .ToList()
                })
                .OrderByDescending(x => x.TravelledDistance)
                .ThenBy(x => x.Model)
                .Take(5)
                .ToList();

            return XmlConverter.Serialize(cars, "cars");
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(x => x.Sales.Any())
                .Select(x => new CustomerOutputModel
                {
                    FullName = x.Name,
                    BoughtCarsCount = x.Sales.Count,
                    SpentMoney = x.Sales.Sum(y => y.Car.PartCars.Sum(z => z.Part.Price))
                })
                .OrderByDescending(x => x.SpentMoney)
                .ToList();

            return XmlConverter.Serialize(customers, "customers");
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Select(x => new SaleOutputModel
                {
                    Car = new CarAttributeOutputModel
                    {
                        Make = x.Car.Make,
                        Model = x.Car.Model,
                        TravelledDistance = x.Car.TravelledDistance
                    },
                    CustomerName = x.Customer.Name,
                    Discount = x.Discount,
                    Price = x.Car.PartCars.Sum(x => x.Part.Price),
                    PriceWithDiscount = x.Car.PartCars.Sum(x => x.Part.Price) - (x.Car.PartCars.Sum(x => x.Part.Price) * x.Discount / 100m),
                })
                .ToList();

            return XmlConverter.Serialize(sales, "sales");
        }
    }
}