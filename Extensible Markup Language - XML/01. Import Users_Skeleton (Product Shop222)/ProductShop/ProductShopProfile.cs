using AutoMapper;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            CreateMap<ImportUserDto, User>();
            CreateMap<ImportProductDto, Product>();
            CreateMap<ImportCategoryDto, Category>();
            CreateMap<ImportCategoryProductDto, CategoryProduct>();

            CreateMap<Product, ExportProductDto>();

            CreateMap<Category, ExportCategoryDto>();
                //.ForMember(d => d.AveragePrice, opt =>
                //             opt.MapFrom(s => s.CategoryProducts
                //                    .Average(cp => cp.Product.Price)))
                //.ForMember(d => d.TotalRevenue, opt =>
                //             opt.MapFrom(s => s.CategoryProducts
                //                    .Sum(cp => cp.Product.Price)))
                //.ForMember(d => d.Count, opt =>
                //             opt.MapFrom(s => s.CategoryProducts.Count));
        }
    }
}
