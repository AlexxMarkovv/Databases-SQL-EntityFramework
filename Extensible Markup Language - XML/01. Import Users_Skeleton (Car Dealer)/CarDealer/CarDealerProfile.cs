using AutoMapper;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            //Import
            this.CreateMap<ImportSupplierDto, Supplier>();
            this.CreateMap<ImportPartDto, Part>();
            this.CreateMap<ImportCarDto, Car>();
            this.CreateMap<ImportCustomerDto, Customer>();
            this.CreateMap<ImportSaleDto, Sale>();

            //Export
            this.CreateMap<Car, ExportCarsBmwDto>();
            this.CreateMap<Sale, ExportSalesPerCustomerDto>();
            this.CreateMap<Supplier, ExportLocalSuppliersDto>();
        }
    }
}
