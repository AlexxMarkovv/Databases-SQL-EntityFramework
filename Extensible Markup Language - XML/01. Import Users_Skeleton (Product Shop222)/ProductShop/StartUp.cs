using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using ProductShop.Utilities;
using System.Xml;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            ProductShopContext context = new ProductShopContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            string inputXml = string.Empty;
            string result = string.Empty;

            //01. Import Users
            //inputXml = File.ReadAllText("../../../Datasets/users.xml");
            //result = ImportUsers(context, inputXml);

            //02. Import Products
            //inputXml = File.ReadAllText("../../../Datasets/products.xml");
            //result = ImportProducts(context, inputXml);

            //03. Import Categories
            //inputXml = File.ReadAllText("../../../Datasets/categories.xml");
            //result = ImportCategories(context, inputXml);

            //04. Import Categories and Products
            //inputXml = File.ReadAllText("../../../Datasets/categories-products.xml");
            //result = ImportCategoryProducts(context, inputXml);

            //05. Export Products In Range
            //result = GetProductsInRange(context);

            //06. Export Sold Products
            //result = GetSoldProducts(context);

            //07. Export Categories By Products Count
            //result = GetCategoriesByProductsCount(context);

            //08. Export Users and Products
            result = GetUsersWithProducts(context);


            Console.WriteLine(result);  
        }

        private static IMapper CreatMapper()
        {
            MapperConfiguration configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });

            return new Mapper(configuration);
        }

        //1. Import Users
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            IMapper mapper = CreatMapper();
            XmlConverter converter = new XmlConverter();

            ImportUserDto[] usersDtos =
                converter.Deserialize<ImportUserDto[]>(inputXml, "Users");

            User[] users = mapper.Map<User[]>(usersDtos);

            context.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Length}";
        }

        //2. Import Products
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var mapper = CreatMapper();
            var xmlConverter = new XmlConverter();

            ImportProductDto[] productDtos =
                xmlConverter.Deserialize<ImportProductDto[]>(inputXml, "Products");

            Product[] products = mapper.Map<Product[]>(productDtos);

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Length}";
        }

        //3. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            IMapper mapper = CreatMapper();
            var xmlConverter = new XmlConverter();

            ImportCategoryDto[] categoryDtos =
                xmlConverter.Deserialize<ImportCategoryDto[]>(inputXml, "Categories");

            Category[] categories = mapper.Map<Category[]>(categoryDtos)
                .Where(c => c.Name != null)
                .ToArray();

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Length}";
        }

        //04. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            IMapper mapper = CreatMapper();
            var xmlConverter = new XmlConverter();

            ImportCategoryProductDto[] categoryProductDtos = xmlConverter
                .Deserialize<ImportCategoryProductDto[]>(inputXml, "CategoryProducts");

            var validCategoryIds = context.Categories.Select(c => c.Id).ToArray();
            var validProductIds = context.Products.Select(p => p.Id).ToArray();

            CategoryProduct[] categoryProducts =
                mapper.Map<CategoryProduct[]>(categoryProductDtos)
                .Where(cp => validCategoryIds.Contains(cp.CategoryId)
                            && validProductIds.Contains(cp.ProductId))
                .ToArray();

            //foreach (var dto in categoryProductDtos)
            //{
            //    if (validCategoryIds.Contains(dto.CategoryId) && validProductIds.Contains(dto.ProductId))
            //    {
            //        var categoryProduct = mapper.Map<CategoryProduct>(dto);
            //        categoryProducts.Add(categoryProduct);
            //    }
            //}

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Length}";
        }

        //05. Export Products In Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            XmlConverter xmlConverter = new XmlConverter();

            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Take(10)
                .Select(p => new ExportProductDto
                {
                    Name = p.Name,
                    Price = p.Price,
                    BuyerName = $"{p.Buyer.FirstName} {p.Buyer.LastName}"
                })
                .ToArray();

            return xmlConverter.Serialize<ExportProductDto[]>(products, "Products");
        }

        //06. Export Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            XmlConverter xmlConverter = new XmlConverter();

            var users = context.Users
                .Where(u => u.ProductsSold.Any())
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .Select(u => new ExportUserInfoDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold.Select(p => new ProductDto()
                    {
                        Name = p.Name,
                        Price = p.Price
                    })
                    .ToArray()
                })
                .ToArray();

            return xmlConverter.Serialize(users, "Users");
        }


        //07. Export Categories By Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Select(c => new ExportCategoryDto
                {
                    Name = c.Name,
                    Count = c.CategoryProducts.Count,
                    AveragePrice = c.CategoryProducts.Average(cp => cp.Product.Price),
                    TotalRevenue = c.CategoryProducts.Sum(cp => cp.Product.Price)
                })
                .OrderByDescending(dto => dto.Count)
                .ThenBy(dto => dto.TotalRevenue)
                .ToArray();

            return new XmlConverter().Serialize(categories, "Categories");
        }

        //08. Export Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any())
                .AsNoTracking()
                .OrderByDescending(u => u.ProductsSold.Count)
                .Select(u => new ExportUserDto()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new ExportSoldProductsWithCountDto()
                    {
                        Count = u.ProductsSold.Count,
                        SoldProducts = u.ProductsSold
                            .Select(p => new ExportProductNamePriceDto()
                            {
                                Name = p.Name,
                                Price = p.Price,
                            })
                            .OrderByDescending(p => p.Price)
                            .ToArray()
                    }

                })
                .ToArray();

            ExportUsersWithCountDto resultDto = new()
            {
                Count = users.Length,
                Users = users
                 .Take(10)
                 .ToArray()
            };


            return new XmlConverter().Serialize(resultDto, "Users");
        }






    }
}