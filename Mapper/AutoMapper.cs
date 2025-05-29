using AutoMapper;
using KTUN_Final_Year_Project.DTOs;
using KTUN_Final_Year_Project.Entities;
using KTUN_Final_Year_Project.ResponseDTOs;
using System;
using System.Linq;

namespace KTUN_Final_Year_Project.Mapper
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            // DTO -> Entity
            CreateMap<CategoriesDTO, Categories>();
            CreateMap<ImageCacheDTO, ImageCache>()
                .ForMember(dest => dest.Image, opt => opt.Ignore());
            CreateMap<InventoryDTO, Inventory>();
            CreateMap<LoyaltyProgramsDTO, LoyaltyPrograms>();
            CreateMap<OrdersDTO, Orders>();
            CreateMap<OrderItemsDTO, OrderItems>();
            CreateMap<ProductsDTO, Products>();
            CreateMap<ProductSuppliersDTO, ProductSuppliers>();
            CreateMap<ReviewsDTO, Reviews>()
                .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => int.Parse(src.UserID)));
            CreateMap<SalesDTO, Sales>();
            CreateMap<SalesDetailsDTO, SalesDetails>();
            CreateMap<StoresDTO, Stores>();
            CreateMap<SuppliersDTO, Suppliers>();
            CreateMap<SupportMessagesDTO, SupportMessages>()
                .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => int.Parse(src.UserID)))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message));
            CreateMap<SupportMessageDTO, SupportMessages>()
                .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.UserID))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.MessageContent));
            CreateMap<UserLoyaltyDTO, UserLoyalty>();
            CreateMap<UsersDTO, Users>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
            CreateMap<UserStoreDTO, UserStore>();
            CreateMap<ProductRecommendationsDTO, ProductRecommendations>();
            CreateMap<FavoriteListDTO, FavoriteList>();
            CreateMap<FavoriteListItemDTO, FavoriteListItem>()
                .ForMember(dest => dest.ProductID, opt => opt.MapFrom(src => src.ProductId));

            // Mappings for UserInformation
            CreateMap<UserInformationDto, UserInformation>(); // DTO -> Entity
            CreateMap<UserInformation, UserInformationDto>(); // Entity -> DTO (basic)

            // Mappings for UserAddress
            CreateMap<UserAddressDto, UserAddress>(); // DTO -> Entity
            CreateMap<UserAddress, UserAddressDto>(); // Entity -> DTO (basic)

            // Entity -> ResponseDTO
            CreateMap<Categories, CategoriesResponseDTO>()
                .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store != null ? src.Store.StoreName : string.Empty))
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products != null ? src.Products.Count : 0));
            
            CreateMap<ImageCache, ImageCacheResponseDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : null))
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.SupplierName : null))
                .ForMember(dest => dest.Base64Image, opt => opt.Ignore())
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => $"/api/imagecache/image/{src.ID}"));
            
            CreateMap<Inventory, InventoryResponseDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : string.Empty))
                .ForMember(dest => dest.Barcode, opt => opt.MapFrom(src => src.Product != null ? src.Product.Barcode : null))
                .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Product != null && src.Product.Store != null ? src.Product.Store.StoreName : null));
            
            CreateMap<LoyaltyPrograms, LoyaltyProgramsResponseDTO>();
            
            CreateMap<Orders, OrdersResponseDTO>()
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User != null ? src.User.Email : string.Empty));
            
            CreateMap<OrderItems, OrderItemsResponseDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : string.Empty))
                .ForMember(dest => dest.Barcode, opt => opt.MapFrom(src => src.Product != null ? src.Product.Barcode : null));
            
            CreateMap<Products, ProductsResponseDTO>()
                .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store != null ? src.Store.StoreName : string.Empty))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : string.Empty))
                .ForMember(dest => dest.SupplierNames, opt => opt.MapFrom(src => src.ProductSuppliers != null ? string.Join(", ", src.ProductSuppliers.Select(ps => ps.Supplier != null ? ps.Supplier.SupplierName : string.Empty)) : null))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.InStock, opt => opt.MapFrom(src => src.StockQuantity > 0));
            
            CreateMap<ProductSuppliers, ProductSuppliersResponseDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : string.Empty))
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.SupplierName : string.Empty))
                .ForMember(dest => dest.Barcode, opt => opt.MapFrom(src => src.Product != null ? src.Product.Barcode : null));
            
            CreateMap<Reviews, ReviewsResponseDTO>()
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : string.Empty));
            
            CreateMap<Sales, SalesResponseDTO>()
                .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store != null ? src.Store.StoreName : string.Empty))
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
                .ForMember(dest => dest.SalesDetails, opt => opt.MapFrom(src => src.SalesDetails));
            
            CreateMap<SalesDetails, SalesDetailsResponseDTO>()
                 .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store != null ? src.Store.StoreName : string.Empty))
                 ;
            
            CreateMap<Stores, StoresResponseDTO>();
            
            CreateMap<Suppliers, SuppliersResponseDTO>();
            
            CreateMap<SupportMessages, SupportMessagesResponseDTO>()
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User != null ? src.User.Email : string.Empty));
            
            CreateMap<UserLoyalty, UserLoyaltyResponseDTO>()
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
                .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.LoyaltyProgram != null ? src.LoyaltyProgram.ProgramName : string.Empty));
            
            CreateMap<Users, UsersResponseDTO>();
            
            CreateMap<UserStore, UserStoreResponseDTO>()
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
                .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store != null ? src.Store.StoreName : string.Empty));
            
            CreateMap<ProductRecommendations, ProductRecommendationsResponseDTO>()
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : string.Empty));
            
            CreateMap<FavoriteList, FavoriteListResponseDTO>()
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty));
            
            CreateMap<FavoriteListItem, FavoriteListItemResponseDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : null))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product != null ? src.Product.Price : (decimal?)null))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product != null ? src.Product.ImageUrl : null))
                .ForMember(dest => dest.InStock, opt => opt.MapFrom(src => src.Product != null && src.Product.StockQuantity > 0))
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => 
                    (src.Product != null && src.Product.ProductSuppliers != null && src.Product.ProductSuppliers.Any()) 
                    ? (src.Product.ProductSuppliers.First().Supplier != null ? src.Product.ProductSuppliers.First().Supplier.SupplierName : null) 
                    : null
                ));

            // Mappings for UserInformation -> UserInformationResponseDto
            CreateMap<UserInformation, UserInformationResponseDto>();
                // .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User != null ? src.User.Email : null)); // Example if UserEmail is needed

            // Mappings for UserAddress -> UserAddressResponseDto
            CreateMap<UserAddress, UserAddressResponseDto>();
                // .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User != null ? src.User.Email : null)); // Example if UserEmail is needed
        }
    }
}