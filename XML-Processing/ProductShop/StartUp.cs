using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DTO;
using ProductShop.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Xml.Serialization;
using ProductShop.DTO.OutputModels;

namespace ProductShop
{
    public class StartUp
    {
        private static readonly MapperConfiguration config = new MapperConfiguration(cfg => cfg.AddProfile(new ProductShopProfile()));
        private static readonly IMapper mapper = config.CreateMapper();

        public static void Main(string[] args)
        {
            ProductShopContext context = new ProductShopContext();
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();
            //string xml;

            //xml = File.ReadAllText("../../../Datasets/users.xml");
            //Console.WriteLine(ImportUsers(context, xml));
            //xml = File.ReadAllText("../../../Datasets/products.xml");
            //ImportProducts(context, xml);
            //xml = File.ReadAllText("../../../Datasets/categories.xml");
            //ImportCategories(context, xml);
            //xml = File.ReadAllText("../../../Datasets/categories-products.xml");
            //ImportCategoryProducts(context, xml);

            Console.WriteLine(GetUsersWithProducts(context));

        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(List<UserInputModel>), new XmlRootAttribute("Users"));

            var userInputModels = (List<UserInputModel>)serializer.Deserialize(new StringReader(inputXml));

            var users = mapper.Map<IEnumerable<User>>(userInputModels);

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count()}";
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(List<ProductInputModel>), new XmlRootAttribute("Products"));

            var productInputModels = (List<ProductInputModel>)serializer.Deserialize(new StringReader(inputXml));

            var products = mapper.Map<IEnumerable<Product>>(productInputModels);

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count()}";
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(List<CategoriesInputModel>), new XmlRootAttribute("Categories"));

            var categoriesInputModels = (List<CategoriesInputModel>)serializer.Deserialize(new StringReader(inputXml));

            IEnumerable<Category> categories = mapper.Map<IEnumerable<Category>>(categoriesInputModels).Where(x => x.Name != null);

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count()}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(List<CategoriesProductsInputModel>), new XmlRootAttribute("CategoryProducts"));

            var categoriesProductsInputModels = (List<CategoriesProductsInputModel>)serializer.Deserialize(new StringReader(inputXml));

            IEnumerable<CategoryProduct> categoriesProducts = mapper.Map<IEnumerable<CategoryProduct>>(categoriesProductsInputModels);

            context.CategoryProducts.AddRange(categoriesProducts);
            context.SaveChanges();

            return $"Successfully imported {categoriesProducts.Count()}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .OrderBy(x => x.Price)
                .Take(10)
                .Select(x => new ProductOutputModel
                {
                    Name = x.Name,
                    Price = x.Price,
                    Buyer = x.Buyer.FirstName + " " + x.Buyer.LastName
                })
                .ToList();

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            var serializer = new XmlSerializer(typeof(List<ProductOutputModel>), new XmlRootAttribute("Products"));

            StringWriter writer = new StringWriter();

            serializer.Serialize(writer, products, ns);
            return writer.ToString();
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var products = context.Users
                .Where(x => x.ProductsSold.Where(x => x.BuyerId != null).Count() > 0)
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Take(5)
                .Select(x => new UserOutputModel
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    ProductsSold = x.ProductsSold
                    .Where(x => x.BuyerId != null)
                    .Select(x => new ProductOutputModel
                    {
                        Name = x.Name,
                        Price = x.Price,
                    })
                    .ToList()
                })
                .ToList();

            return XmlConverter.Serialize<List<UserOutputModel>>(products, "Users");

        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Include(x => x.CategoryProducts)
                .Select(x => new CategoriesOutputModel
                {
                    Name = x.Name,
                    ProductsCount = x.CategoryProducts.Count,
                    AveragePrice = x.CategoryProducts.Average(x => x.Product.Price),
                    TotalRevenue = x.CategoryProducts.Sum(x => x.Product.Price)
                })
                .OrderByDescending(x => x.ProductsCount)
                .ThenBy(x => x.TotalRevenue)
                .ToList();

            return XmlConverter.Serialize(categories, "Categories");
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Include(x => x.ProductsSold)
                .ToList()
                .Select(x => new DTO.OutputModels.GetUsersWithProducts.UserOutputModel
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Age = x.Age != null ? x.Age.ToString() : null,
                    SoldProducts = new DTO.OutputModels.GetUsersWithProducts.SoldProductsModel
                    {
                        Count = x.ProductsSold.Where(x => x.BuyerId != null).Count(),
                        Products = x.ProductsSold.Where(x => x.BuyerId != null).Select(x => new DTO.OutputModels.GetUsersWithProducts.ProductOutputModel
                        {
                            Name = x.Name,
                            Price = x.Price
                        })
                        .OrderByDescending(x => x.Price)
                        .ToList()
                    }
                }
                )
                .Where(x => x.SoldProducts.Count >= 1)
                .OrderByDescending(x => x.SoldProducts.Count)
                
                .ToList();

            var result = new DTO.OutputModels.GetUsersWithProducts.ResultOutputModel
            {
                UsersCount = users.Where(x => x.SoldProducts.Count >= 1).Count(),
                Users = users.Take(10).ToList()
            };

            return XmlConverter.Serialize(result, "Users");
        }
    }
}