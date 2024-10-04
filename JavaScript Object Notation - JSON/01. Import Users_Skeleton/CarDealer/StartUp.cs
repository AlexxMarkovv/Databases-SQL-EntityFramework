using AutoMapper;
using CarDealer.Data;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();

            string jsonInput = string.Empty;
            string result = string.Empty;

            //09. Import Suppliers
            //jsonInput = File.ReadAllText("../../../Datasets/suppliers.json");
            //result = ImportSuppliers(context, jsonInput);

            //10. Import Parts
            //jsonInput = File.ReadAllText("../../../Datasets/parts.json");
            //result = ImportParts(context, jsonInput);

            //11. Import Cars
            //jsonInput = File.ReadAllText("../../../Datasets/cars.json");
            //result = ImportCars(context, jsonInput);

            //12.Import Customers
            //jsonInput = File.ReadAllText("../../../Datasets/customers.json");
            //result = ImportCustomers(context, jsonInput);

            //13.Import Sales
            //jsonInput = File.ReadAllText("../../../Datasets/sales.json");
            //result = ImportSales(context, jsonInput);

            //14. Export Ordered Customers
            //result = GetOrderedCustomers(context);

            //15. Export Cars from Make Toyota
            //result = GetCarsFromMakeToyota(context);

            //16. Export Local Suppliers
            //result = GetLocalSuppliers(context);

            //17. Export Cars with Their List of Parts
            //result = GetCarsWithTheirListOfParts(context);

            //18. Export Total Sales by Customer
            result = GetTotalSalesByCustomer(context);

            Console.WriteLine(result);
        }

        //Mapper Creator Method
        private static IMapper CreatMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });

            IMapper mapper = config.CreateMapper();
            return mapper;
        }

        //09.Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            IMapper mapper = CreatMapper();

            ImportSupplierDto[] supplierDtos = JsonConvert.DeserializeObject<ImportSupplierDto[]>(inputJson);
            Supplier[] suppliers = mapper.Map<Supplier[]>(supplierDtos);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Length}.";
        }

        //10. Import Parts
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            IMapper mapper = CreatMapper();

            ImportPartDto[] partsDtos = JsonConvert.DeserializeObject<ImportPartDto[]>(inputJson);
           
            ICollection<Part> parts = new List<Part>();

            foreach (var part in partsDtos)
            {
                if (context.Suppliers.Any(s => s.Id == part.SupplierId))
                {
                    parts.Add(mapper.Map<Part>(part));
                }
            }

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}.";
        }

        //11. Import Cars
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            IMapper mapper = CreatMapper();

            ImportCarDto[] carsDtos = JsonConvert.DeserializeObject<ImportCarDto[]>(inputJson);
            Car[] cars = mapper.Map<Car[]>(carsDtos);

            ICollection<Car> carsToAdd = new HashSet<Car>();

            foreach (var carDto in carsDtos)
            {
                Car currCar = mapper.Map<Car>(carDto);

                foreach (var id in carDto.PartsIds)
                {
                    if (context.Parts.Any(p => p.Id == id))
                    {
                        currCar.PartsCars.Add(new PartCar
                        {
                            PartId = id
                        });
                    }
                }

                carsToAdd.Add(currCar);
            }

            context.Cars.AddRange(carsToAdd);
            context.SaveChanges();

            return $"Successfully imported {carsToAdd.Count}.";
        }

        //12. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            IMapper mapper = CreatMapper();

            ImportCustomerDto[] customersDtos = JsonConvert.DeserializeObject<ImportCustomerDto[]>(inputJson);
            Customer[] customers = mapper.Map<Customer[]>(customersDtos);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Length}.";
        }

        //13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            IMapper mapper = CreatMapper();

            ImportSaleDto[] saleDtos = JsonConvert.DeserializeObject<ImportSaleDto[]>(inputJson);
            Sale[] sales = mapper.Map<Sale[]>(saleDtos);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Length}.";
        }

        //14. Export Ordered Customers
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new
                {
                    c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    c.IsYoungDriver
                })
                .ToArray();

            return JsonConvert.SerializeObject(customers, Formatting.Indented);
        }

        //15. Export Cars from Make Toyota
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var toyotaCars = context.Cars
                .Where(c => c.Make == "Toyota")
                .AsNoTracking()
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .Select(c => new
                {
                    c.Id,
                    c.Make,
                    c.Model,
                    c.TraveledDistance
                })
                .ToArray();

            return JsonConvert.SerializeObject(toyotaCars, Formatting.Indented);
        }

        //16. Export Local Suppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var localSuppliers = context.Suppliers
                .Where(s => s.IsImporter.Equals(false))
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToArray();


            return JsonConvert.SerializeObject(localSuppliers, Formatting.Indented);
        }

        //17. Export Cars with Their List of Parts
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carsWithParts = context.Cars
                .Select(c => new
                {
                    car = new
                    {
                        c.Make,
                        c.Model,
                        c.TraveledDistance
                    },
                    parts = c.PartsCars
                    .Select(pc => new
                    {
                        Name = pc.Part.Name,
                        Price = pc.Part.Price.ToString()
                    })
                    .ToArray()
                })
                .ToArray();

            return JsonConvert.SerializeObject(carsWithParts, Formatting.Indented);
        }

        //18. Export Total Sales by Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Count() > 0)
                .Select(c => new
                {
                    fullName = c.Name,
                    boughtCars = c.Sales.Count(),
                    spentMoney = c.Sales.Sum(s => s.Car.PartsCars.Sum(pc => pc.Part.Price))
                })
                .OrderByDescending(x => x.spentMoney)
                .ThenByDescending(x => x.boughtCars)
                .ToList();

            return JsonConvert.SerializeObject(customers, Formatting.Indented);
        }
    }
}