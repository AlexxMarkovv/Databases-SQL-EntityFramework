using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            ProductShopContext context = new ProductShopContext();

            string result = string.Empty;
            string inputJson = string.Empty;

            //01.Import Users
            //inputJson = File.ReadAllText(@"../../../Datasets/users.json");
            //result = ImportUsers(context, inputJson);

            //02.Import Products
            //inputJson = File.ReadAllText(@"../../../Datasets/products.json");
            //result = ImportProducts(context, inputJson);

            //03.Import Categories
            //inputJson = File.ReadAllText(@"../../../Datasets/categories.json");
            //result = ImportCategories(context, inputJson);

            //04.Import CategoryProducts
            //inputJson = File.ReadAllText(@"../../../Datasets/categories-products.json");
            //result = ImportCategoryProducts(context, inputJson);

            //05.Export Products in Range
            //result = GetProductsInRange(context);

            //06. Export Sold Products
            //result = GetSoldProducts(context);

            //07. Export Categories by Products Count
            //result = GetCategoriesByProductsCount(context);

            //08. Export Users and Products
            //result = GetUsersWithProducts(context);

            Console.WriteLine(result);
        }

        // 01.Import Users
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            IMapper mapper = CreateMapper();

            ImportUserDto[] userDtos
                = JsonConvert.DeserializeObject<ImportUserDto[]>(inputJson);

            ICollection<User> validUsers = new HashSet<User>();
            foreach (var userDto in userDtos)
            {
                User user = mapper.Map<User>(userDto);

                validUsers.Add(user);
            }

            //Here we have all valid users ready for the DB
            context.Users.AddRange(validUsers);
            context.SaveChanges();

            return $"Successfully imported {validUsers.Count}";
        }

        // 02.Import Products
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            IMapper mapper = CreateMapper();

            var productDtos = JsonConvert.DeserializeObject<ImportProductDto[]>(inputJson);
            Product[] products = mapper.Map<Product[]>(productDtos);

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Length}";
        }

        // 03.Import Categories
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            IMapper mapper = CreateMapper();

            var categoryDtos = JsonConvert.DeserializeObject<ImportCategoryDto[]>(inputJson);
            ImportCategoryDto[] categories = mapper.Map<ImportCategoryDto[]>(categoryDtos);

            ICollection<Category> validCategories = new HashSet<Category>();
            foreach (ImportCategoryDto categoryDto in categoryDtos)
            {
                if (String.IsNullOrEmpty(categoryDto.Name))
                {
                    continue;
                }

                Category category = mapper.Map<Category>(categoryDto);
                validCategories.Add(category);
            }


            context.Categories.AddRange(validCategories);
            context.SaveChanges();

            return $"Successfully imported {validCategories.Count}";
        }

        // 04.Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            IMapper mapper = CreateMapper();

            ImportCategoryProductDto[] categoryProductDtos =
                JsonConvert.DeserializeObject<ImportCategoryProductDto[]>(inputJson);

            var categoriesAndProducts = mapper.Map<CategoryProduct[]>(categoryProductDtos);

            context.CategoriesProducts.AddRange(categoriesAndProducts);
            context.SaveChanges();

            return $"Successfully imported {categoriesAndProducts.Length}";
        }

        // 05. Export Products in Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            //var products = context.Products
            //    .Where(p => p.Price >= 500 && p.Price <= 1000)
            //    .Select(p => new ExportProductInRangeDto
            //    {
            //        ProductName = p.Name,
            //        ProductPrice = p.Price,
            //        SellerName = p.Seller.FirstName + " " + p.Seller.LastName,
            //    })
            //    .OrderBy(p => p.ProductPrice)
            //    .ToArray();

            IMapper mapper = CreateMapper();

            ExportProductInRangeDto[] productsDtos = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .AsNoTracking()
                .ProjectTo<ExportProductInRangeDto>(mapper.ConfigurationProvider)
                .ToArray();


            return JsonConvert.SerializeObject(productsDtos, Formatting.Indented);
        }

         //06. Export Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            var usersWithSoldProducts = context.Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .AsNoTracking()
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    soldProducts = u.ProductsSold
                    .Select(ps => new
                    {
                        name = ps.Name,
                        price = ps.Price,
                        buyerFirstName = ps.Buyer.FirstName,
                        buyerLastName = ps.Buyer.LastName
                    })
                    .ToArray()
                })
                .OrderBy(u => u.lastName)
                .ThenBy(u => u.firstName)
                .ToArray();

            return JsonConvert.SerializeObject(usersWithSoldProducts, Formatting.Indented);
        }

        //07. Export Categories by Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categoriesByProducts = context.Categories
                .OrderByDescending(c => c.CategoriesProducts.Count())
                .Select(c => new
                {
                    category = c.Name,
                    productsCount = c.CategoriesProducts.Count,
                    averagePrice = (c.CategoriesProducts.Any()
                        ? c.CategoriesProducts.Average(cp => cp.Product.Price)
                        : 0).ToString("F2"),
                    totalRevenue = (c.CategoriesProducts.Any()
                        ? c.CategoriesProducts.Sum(cp => cp.Product.Price)
                        : 0).ToString("F2")
                })
                .AsNoTracking()
                .ToArray();

            return JsonConvert.SerializeObject(categoriesByProducts, Formatting.Indented);
        }

        //08. Export Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .Select(u => new
                { // UserDTO
                    u.FirstName,
                    u.LastName,
                    u.Age,
                    SoldProducts = new
                    { //ProductWrapperDTO
                        Count = u.ProductsSold
                            .Count(p => p.Buyer != null),
                        Products = u.ProductsSold
                            .Where(p => p.Buyer != null)
                            .Select(p => new
                            { //ProductDTO
                                p.Name,
                                p.Price
                            })
                            .ToArray()
                    }
                })
                .AsNoTracking()
                .OrderByDescending(u => u.SoldProducts.Count)
                .ToArray();

            var userWrapperDto = new
            {
                UsersCount = users.Length,
                Users = users
            };

            return JsonConvert.SerializeObject(userWrapperDto, Formatting.Indented,
                new JsonSerializerSettings 
                {   
                    ContractResolver = new CamelCasePropertyNamesContractResolver(), 
                    NullValueHandling = NullValueHandling.Ignore 
                });
        }



        private static IMapper CreateMapper()
        {
            return new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));
        }
    }
}