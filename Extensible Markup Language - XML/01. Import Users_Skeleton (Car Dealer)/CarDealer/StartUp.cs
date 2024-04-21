using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Castle.Core.Resource;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CarDealer;

public class StartUp
{
    public static void Main()
    {
        CarDealerContext context = new CarDealerContext();

        //context.Database.EnsureDeleted();
        //context.Database.EnsureCreated();

        string inputXml = string.Empty;
        string result = string.Empty;

        //09.Import Suppliers
        //inputXml = File.ReadAllText(@"../../../Datasets/suppliers.xml");
        //result = ImportSuppliers(context, inputXml);

        //10. Import Parts
        //inputXml = File.ReadAllText(@"../../../Datasets/parts.xml");
        //result = ImportParts(context, inputXml);

        //11. Import Cars
        //inputXml = File.ReadAllText(@"../../../Datasets/cars.xml");
        //result = ImportCars(context, inputXml);

        //12. Import Customers
        //inputXml = File.ReadAllText(@"../../../Datasets/customers.xml");
        //result = ImportCustomers(context, inputXml);

        //13. Import Sales
        //inputXml = File.ReadAllText(@"../../../Datasets/sales.xml");
        //result = ImportSales(context, inputXml);

        //14. Export Cars With Distance
        //result = GetCarsWithDistance(context);

        //15. Export Cars From Make BMW
        //result = GetCarsFromMakeBmw(context);

        //16. Export Local Suppliers
        //result = GetLocalSuppliers(context);

        //17. Export Cars With Their List Of Parts
        result = GetCarsWithTheirListOfParts(context);

        //18. Export Total Sales by Customer
        //result = GetTotalSalesByCustomer(context);

        //19. Export Sales with Applied Discount
        //result = GetSalesWithAppliedDiscount(context);

        Console.WriteLine(result);
    }

    
    private static IMapper CreateMapper()
    {
        MapperConfiguration config = new MapperConfiguration(config =>
        {
            config.AddProfile<CarDealerProfile>();
        });

        return new Mapper(config);
    }


    //Import Data
    //09 Import Suppliers
    public static string ImportSuppliers(CarDealerContext context, string inputXml)
    {
        IMapper mapper = CreateMapper();

        XmlSerializer xmlSerializer =
            new XmlSerializer(typeof(ImportSupplierDto[]), new XmlRootAttribute("Suppliers"));

        using StringReader reader = new StringReader(inputXml);

        ImportSupplierDto[] suppliersDtos = (ImportSupplierDto[])xmlSerializer.Deserialize(reader);

        Supplier[] suppliers = mapper.Map<Supplier[]>(suppliersDtos);

        context.AddRange(suppliers);
        context.SaveChanges();

        return $"Successfully imported {suppliers.Length}";
    }

    //10. Import Parts
    public static string ImportParts(CarDealerContext context, string inputXml)
    {
        IMapper mapper = CreateMapper();

        XmlSerializer serializer =
            new XmlSerializer(typeof(ImportPartDto[]), new XmlRootAttribute("Parts"));

        using StringReader reader = new(inputXml);
        ImportPartDto[] importPartDtos = (ImportPartDto[])serializer.Deserialize(reader);

        var validSuppliersIds = context.Suppliers
            .Select(s => s.Id)
            .ToArray();

        Part[] parts = mapper.Map<Part[]>(importPartDtos 
            .Where(p => validSuppliersIds.Contains(p.SupplierId)));

        context.AddRange(parts);
        context.SaveChanges();

        return $"Successfully imported {parts.Length}";
    }

    //11. Import Cars
    public static string ImportCars(CarDealerContext context, string inputXml)
    {
        IMapper mapper = CreateMapper();

        XmlSerializer xmlSerializer = 
            new XmlSerializer(typeof(ImportCarDto[]), new XmlRootAttribute("Cars"));

        using StringReader reader = new(inputXml);

        ImportCarDto[] importCarDtos = (ImportCarDto[])xmlSerializer.Deserialize(reader);

        List<Car> cars = new List<Car>();

        foreach (var carDto in importCarDtos)
        {
            Car car = mapper.Map<Car>(carDto);

            int[] carPartIds = carDto.PartsIds
                .Select(p => p.PartId)
                .Distinct()
                .ToArray();

            List<PartCar> carParts = new List<PartCar>();

            foreach(var id in carPartIds)
            {
                carParts.Add(new PartCar
                {
                    Car = car,
                    PartId = id
                });
            }

            car.PartsCars = carParts;
            cars.Add(car);
        }

        context.AddRange(cars);
        context.SaveChanges();

        return $"Successfully imported {cars.Count}";
    }

    //12. Import Customers
    public static string ImportCustomers(CarDealerContext context, string inputXml)
    {
        IMapper mapper = CreateMapper();

        XmlSerializer xmlSerializer =
            new XmlSerializer(typeof(ImportCustomerDto[]), new XmlRootAttribute("Customers"));

        using StringReader reader = new(inputXml);

        ImportCustomerDto[] importCustomerDtos = 
            (ImportCustomerDto[])xmlSerializer.Deserialize(reader);

        Customer[] customers = mapper.Map<Customer[]>(importCustomerDtos);

        context.AddRange(customers);
        context.SaveChanges();

        return $"Successfully imported {customers.Length}";
    }

    //13. Import Sales
    public static string ImportSales(CarDealerContext context, string inputXml)
    {
        IMapper mapper = CreateMapper();

        XmlSerializer xmlSerializer =
            new XmlSerializer(typeof(ImportSaleDto[]), new XmlRootAttribute("Sales"));

        using StringReader reader = new StringReader(inputXml);

        ImportSaleDto[] importSaleDtos = (ImportSaleDto[])xmlSerializer.Deserialize(reader);

        int[] carsIds = context.Cars
            .Select(car =>  car.Id)
            .ToArray();

        Sale[] sales = mapper.Map<Sale[]>(importSaleDtos
            .Where(importSaleDto => carsIds.Contains(importSaleDto.CarId)));

        context.AddRange(sales);
        context.SaveChanges();

        return $"Successfully imported {sales.Length}";
    }


    //Query and Export Data
    //14. Export Cars With Distance
    public static string GetCarsWithDistance(CarDealerContext context)
    {
        IMapper mapper = CreateMapper();

        var cars = context.Cars
            .Where(c => c.TraveledDistance > 2_000_000)
            .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
            .Take(10)
            .ProjectTo<ExportCarsWithDistanceDto>(mapper.ConfigurationProvider)
            .ToArray();

        XmlSerializer xmlSerializer =
            new XmlSerializer(typeof(ExportCarsWithDistanceDto[]), new XmlRootAttribute("cars"));

        var xmlns = new XmlSerializerNamespaces();
        xmlns.Add(string.Empty, string.Empty);

        StringBuilder stringBuilder = new StringBuilder();
        using (StringWriter sw = new StringWriter(stringBuilder))
        {
            xmlSerializer.Serialize(sw, cars, xmlns);
        }

        return stringBuilder.ToString().TrimEnd();
    }

    //15. Export Cars From Make BMW
    public static string GetCarsFromMakeBmw(CarDealerContext context)
    {
        var carsBmwDtos = context.Cars
            .Where(c => c.Make.ToUpper() == "BMW")
            .OrderBy(c => c.Model)
            .ThenByDescending(c => c.TraveledDistance)
            .Select(c => new ExportCarsBmwDto()
            {
                Id = c.Id,
                Model = c.Model,
                TraveledDistance = c.TraveledDistance,
            })
            .ToArray();

        XmlSerializer xmlSerializer = new(typeof(ExportCarsBmwDto[]), new XmlRootAttribute("cars"));
       

        StringBuilder stringBuilder = new StringBuilder();
        using (StringWriter sw = new StringWriter(stringBuilder))
        {
            var xmlns = new XmlSerializerNamespaces();
            xmlns.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(sw, carsBmwDtos, xmlns);
        }

        return stringBuilder.ToString().TrimEnd();
    }


    //16. Export Local Suppliers
    public static string GetLocalSuppliers(CarDealerContext context)
    {
        var localSuppliers = context.Suppliers
            .Where(s => !s.IsImporter)
            .Select(s => new ExportLocalSuppliersDto()
            {
                Id = s.Id,
                Name = s.Name,
                PartsCount = s.Parts.Count
            })
            .ToArray();

        return SerializeToXml(localSuppliers, "suppliers");
    }

    //17. Export Cars With Their List Of Parts
    public static string GetCarsWithTheirListOfParts(CarDealerContext context)
    {
        ExportCarPartsDto[] carsPartsDtos = context.Cars
             .OrderByDescending(c => c.TraveledDistance)
             .ThenBy(c => c.Model)
             .Take(5)
             .Select(c => new ExportCarPartsDto()
             {
                 Make = c.Make,
                 Model = c.Model,
                 TraveledDistance = c.TraveledDistance,
                 Parts = c.PartsCars.Select(pc => new ExportPartDto()
                 {
                     Name = pc.Part.Name,
                     Price = pc.Part.Price
                 })
                 .OrderByDescending(p => p.Price)
                 .ToArray()
             })
             .ToArray();

        return SerializeToXml(carsPartsDtos, "cars");
    }

    //18. Export Total Sales by Customer
    public static string GetTotalSalesByCustomer(CarDealerContext context)
    {
        var tempDto = context.Customers
            .Where(c => c.Sales.Any())
            .Select(c => new
            {
                FullName = c.Name,
                BoughtCars = c.Sales.Count,
                SpentMoney = c.Sales.Select(s => new
                {
                    Prices = c.IsYoungDriver
                    ? s.Car.PartsCars.Sum(pc => Math.Round((double)pc.Part.Price * 0.95, 2))
                    : s.Car.PartsCars.Sum(pc => (double)pc.Part.Price)
                }).ToArray(),
            }).ToArray();

        ExportSalesPerCustomerDto[] totalSales = tempDto
            .OrderByDescending(x => x.SpentMoney.Sum(sm => sm.Prices))
            .Select(x => new ExportSalesPerCustomerDto()
            {
                FullName = x.FullName,
                BoughtCars = x.BoughtCars,
                SpentMoney = x.SpentMoney.Sum(sp => sp.Prices).ToString("f2")
            })
            .ToArray();

        //var totalSales = context.Customers
        //    .Where(c => c.Sales.Any())
        //    .Select(c => new ExportSalesPerCustomerDto
        //    {
        //        FullName = c.Name,
        //        BoughtCars = c.Sales.Count,
        //        SpentMoney = c.Sales.Sum(s =>
        //            s.Car.PartsCars.Sum(pc =>
        //            Math.Round(c.IsYoungDriver ? pc.Part.Price * 0.95m : pc.Part.Price, 2)))
        //    })
        //    .OrderByDescending(x => x.SpentMoney)
        //    .ToArray();

        return SerializeToXml<ExportSalesPerCustomerDto[]>(totalSales, "customers");
    }

    //19. Export Sales with Applied Discount
    public static string GetSalesWithAppliedDiscount(CarDealerContext context)
    {
        var salesDto = context.Sales
            .Select(s => new ExportSalesWithDiscountDto()
            {
                Car = new ExportCarAttributesDto()
                {
                    Make = s.Car.Make,
                    Model = s.Car.Model,
                    TraveledDistance = s.Car.TraveledDistance
                },
                Discount = (int)s.Discount,
                CustomerName = s.Customer.Name,
                Price = s.Car.PartsCars.Sum(ps => ps.Part.Price),
                PriceWithDiscount =
                        Math.Round((double)(s.Car.PartsCars
                            .Sum(p => p.Part.Price) * (1 - (s.Discount / 100))), 4)
            })
            .ToArray();

        return SerializeToXml<ExportSalesWithDiscountDto[]>(salesDto, "sales");
    }



    /// <summary>
    /// Generic method to Serialize DTOs to XML
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dto"></param>
    /// <param name="xmlRootAttribute"></param>
    /// <returns></returns>
    private static string SerializeToXml<T>(T dto, string xmlRootAttribute)
    {
        XmlSerializer xmlSerializer = 
            new XmlSerializer(typeof(T), new XmlRootAttribute(xmlRootAttribute));

        StringBuilder stringBuilder = new();

        using (StringWriter stringWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture))
        {
            XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces();
            xmlns.Add(string.Empty, string.Empty);

            try
            {
                xmlSerializer.Serialize(stringWriter, dto, xmlns);
            }
            catch (Exception)
            {

                throw;
            }
        }

        return stringBuilder.ToString().TrimEnd();
    }











}