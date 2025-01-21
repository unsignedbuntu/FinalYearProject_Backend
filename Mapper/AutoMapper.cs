using AutoMapper;
using KTUN_Final_Year_Project.DTOs;
using KTUN_Final_Year_Project.Entities;
using KTUN_Final_Year_Project.ResponseDTOs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KTUN_Final_Year_Project.Mapper // Replace with your namespace  
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {

            CreateMap<SalesDetailsDTO, SalesDetails>();
            CreateMap<SalesDetails, SalesDetailsDTO>();

            CreateMap<SalesDTO, Sales>();
            CreateMap<Sales, SalesDTO>();

            CreateMap<UserLoyaltyDTO, UserLoyalty>();
            CreateMap<UserLoyalty, UserLoyaltyDTO>();

            CreateMap<ProductRecommendationsDTO, ProductRecommendations>();
            CreateMap<ProductRecommendations, ProductRecommendationsDTO>();

            CreateMap<UserStoreDTO, UserStore>();
            CreateMap<UserStore, UserStoreDTO>();

            CreateMap<CustomerFeedbackDTO, CustomerFeedback>();
            CreateMap<CustomerFeedback, CustomerFeedbackDTO>();

            CreateMap<UsersDTO, Users>();
            CreateMap<Users, UsersDTO>();

            CreateMap<InventoryDTO, Inventory>();
            CreateMap<Inventory, InventoryDTO>();

            CreateMap<ProductSuppliersDTO, ProductSuppliers>();
            CreateMap<ProductSuppliers, ProductSuppliersDTO>();

            CreateMap<ProductsDTO, Products>();
            CreateMap<Products, ProductsDTO>();

            CreateMap<StoresDTO, Stores>();
            CreateMap<Stores, StoresDTO>();

            CreateMap<CategoriesDTO, Categories>();
            CreateMap<Categories, CategoriesDTO>();

            CreateMap<SuppliersDTO, Suppliers>();
            CreateMap<Suppliers, SuppliersDTO>();

            CreateMap<LoyaltyProgramsDTO, LoyaltyPrograms>();
            CreateMap<LoyaltyPrograms, LoyaltyProgramsDTO>();

            CreateMap<SalesDetailsResponseDTO, SalesDetails>();
            CreateMap<SalesDetails, SalesDetailsResponseDTO>();

            CreateMap<SalesResponseDTO, Sales>();
            CreateMap<Sales, SalesResponseDTO>();

            CreateMap<UserLoyaltyResponseDTO, UserLoyalty>();
            CreateMap<UserLoyalty, UserLoyaltyResponseDTO>();

            CreateMap<ProductRecommendationsResponseDTO, ProductRecommendations>();
            CreateMap<ProductRecommendations, ProductRecommendations>();

            CreateMap<UserStoreResponseDTO, UserStore>();
            CreateMap<UserStore, UserStoreResponseDTO>();

            CreateMap<CustomerFeedbackResponseDTO, CustomerFeedback>();
            CreateMap<CustomerFeedback, CustomerFeedbackResponseDTO>();

            CreateMap<UsersResponseDTO, Users>();
            CreateMap<Users, UsersResponseDTO>();

            CreateMap<InventoryResponseDTO, Inventory>();
            CreateMap<Inventory, InventoryResponseDTO>();

            CreateMap<ProductSuppliersResponseDTO, ProductSuppliers>();
            CreateMap<ProductSuppliers,ProductSuppliersResponseDTO>();

            CreateMap<ProductsResponseDTO, Products>();
            CreateMap<Products, ProductsResponseDTO>();

            CreateMap<StoresResponseDTO, Stores>();
            CreateMap<Stores, StoresResponseDTO>();

            CreateMap<CategoriesResponseDTO, Categories>();
            CreateMap<Categories, CategoriesResponseDTO>();

            CreateMap<SuppliersResponseDTO, Suppliers>();
            CreateMap<Suppliers, SuppliersResponseDTO>();

            CreateMap<LoyaltyProgramsResponseDTO, LoyaltyPrograms>();
            CreateMap<LoyaltyPrograms, LoyaltyProgramsResponseDTO>();

            CreateMap<ImageCacheResponseDTO, ImageCache>();
            CreateMap<ImageCache, ImageCacheResponseDTO>();

        }
    }
}