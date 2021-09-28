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

namespace ProductShop
{
    public class StartUp
    {
        private static readonly MapperConfiguration config = new MapperConfiguration(cfg => cfg.AddProfile(new ProductShopProfile()));
        private static readonly IMapper mapper = config.CreateMapper();

        public static void Main(string[] args)
        {
            //ProductShopContext context = new ProductShopContext();
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //var json = File.ReadAllText("../../../Datasets/users.json");
            //ImportUsers(context, json);
            //json = File.ReadAllText("../../../Datasets/products.json");
            //ImportProducts(context, json);
            //json = File.ReadAllText("../../../Datasets/categories.json");
            //ImportCategories(context, json);
            //json = File.ReadAllText("../../../Datasets/categories-products.json");
            //ImportCategoryProducts(context, json);

            Console.WriteLine(GetUsersWithProducts(context));

        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var userInputModels = JsonConvert.DeserializeObject<List<UserInputModel>>(inputJson);

            List<User> users = mapper.Map<List<User>>(userInputModels);

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var productInputModels = JsonConvert.DeserializeObject<List<ProductInputModel>>(inputJson);

            List<Product> products = mapper.Map<List<Product>>(productInputModels);

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var categoriesInputModels = JsonConvert.DeserializeObject<IEnumerable<CategoriesInputModel>>(inputJson);

            IEnumerable<Category> categories = mapper.Map<IEnumerable<Category>>(categoriesInputModels).Where(x => x.Name != null);

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count()}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoriesProductsInputModels = JsonConvert.DeserializeObject<IEnumerable<CategoriesProductsInputModel>>(inputJson);

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
                .Select(x => new
                {
                    name = x.Name,
                    price = x.Price,
                    seller = x.Seller.FirstName + " " + x.Seller.LastName
                }
                );

            return JsonConvert.SerializeObject(products, Formatting.Indented);
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var products = context.Users
                .Where(x => x.ProductsSold.Where(x => x.BuyerId != null).Count() > 0)
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    soldProducts = x.ProductsSold
                    .Where(x => x.BuyerId != null)
                    .Select(x => new
                    {
                        name = x.Name,
                        price = x.Price,
                        buyerFirstName = x.Buyer.FirstName,
                        buyerLastName = x.Buyer.LastName
                    })
                })
                .ToList();

            return JsonConvert.SerializeObject(products, Formatting.Indented);
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Include(x => x.CategoryProducts)
                .Select(x => new
                {
                    category = x.Name,
                    productsCount = x.CategoryProducts.Count,
                    averagePrice = x.CategoryProducts.Average(x => x.Product.Price).ToString("0.00"),
                    totalRevenue = x.CategoryProducts.Sum(x => x.Product.Price).ToString("0.00")
                })
                .OrderByDescending(x => x.productsCount)
                .ToList();

            return JsonConvert.SerializeObject(categories, Formatting.Indented);
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    age = x.Age,
                    soldProducts = new
                    {
                        count = x.ProductsSold.Where(x => x.BuyerId != null).Count(),
                        products = x.ProductsSold.Where(x => x.BuyerId != null).Select(x => new
                        {
                            name = x.Name,
                            price = x.Price
                        }
                        )
                    }
                }
                )
                .Where(x => x.soldProducts.count >= 1)
                .OrderByDescending(x => x.soldProducts.count)
                .ToList();

            var result = new
            {
                usersCount = users.Where(x => x.soldProducts.count >= 1).Count(),
                users = users
            };

            return JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        }
    }
}