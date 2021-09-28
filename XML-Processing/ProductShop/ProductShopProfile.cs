using AutoMapper;
using ProductShop.DTO;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            this.CreateMap<UserInputModel, User>();
            this.CreateMap<ProductInputModel, Product>();
            this.CreateMap<CategoriesInputModel, Category>();
            this.CreateMap<CategoriesProductsInputModel, CategoryProduct>();
        }
    }
}
