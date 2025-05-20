using System.Collections.Generic;

namespace KTUN_Final_Year_Project.DTOs
{
    public class GlobalSearchResultDto
    {
        public List<ProductSearchResultDto> Products { get; set; } = new List<ProductSearchResultDto>();
        public List<CategorySearchResultDto> Categories { get; set; } = new List<CategorySearchResultDto>();
        public List<StoreSearchResultDto> Stores { get; set; } = new List<StoreSearchResultDto>();
        public List<SupplierSearchResultDto> Suppliers { get; set; } = new List<SupplierSearchResultDto>();
    }

    public class ProductSearchResultDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string? ImageUrl { get; set; } // Changed to ImageUrl as per product entity
    }

    public class CategorySearchResultDto
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
    }

    public class StoreSearchResultDto
    {
        public int StoreID { get; set; }
        public string StoreName { get; set; }
    }

    public class SupplierSearchResultDto
    {
        public int SupplierID { get; set; }
        public string SupplierName { get; set; }
    }
} 