using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Newtonsoft.Json;

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
    }
}