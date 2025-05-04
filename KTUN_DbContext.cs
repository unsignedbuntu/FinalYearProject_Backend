namespace KTUN_Final_Year_Project
{
    using KTUN_Final_Year_Project.Entities;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Migrations;
    using System.Reflection.Emit;
    public class KTUN_DbContext : IdentityDbContext<Users, IdentityRole<int>, int,
                                                    IdentityUserClaim<int>, IdentityUserRole<int>,
                                                    IdentityUserLogin<int>, IdentityRoleClaim<int>,
                                                    IdentityUserToken<int>>
    {
#nullable disable
        public DbSet<Stores> Stores { get; set; } = null!;
        public DbSet<Suppliers> Suppliers { get; set; } = null!;
        public DbSet<LoyaltyPrograms> LoyaltyPrograms { get; set; } = null!;
        public DbSet<Categories> Categories { get; set; } = null!;
        public DbSet<Products> Products { get; set; } = null!;
        public DbSet<ProductSuppliers> ProductSuppliers { get; set; } = null!;
        public DbSet<Inventory> Inventory { get; set; } = null!;
        public DbSet<Sales> Sales { get; set; } = null!;
        public DbSet<SalesDetails> SalesDetails { get; set; } = null!;
        public DbSet<UserLoyalty> UserLoyalty { get; set; } = null!;
        public DbSet<KTUN_Final_Year_Project.Entities.UserStore> UserStore { get; set; } = null!;
        public DbSet<ProductRecommendations> ProductRecommendations { get; set; } = null!;
        public DbSet<ImageCache> ImageCache { get; set; } = null!;
        public DbSet<Orders> Orders { get; set; } = null!;
        public DbSet<OrderItems> OrderItems { get; set; } = null!;
        public DbSet<Reviews> Reviews { get; set; } = null!;
        public DbSet<SupportMessages> SupportMessages { get; set; } = null!;
        public DbSet<UserFavorite> UserFavorites { get; set; } = null!;
        public DbSet<UserCartItem> UserCartItems { get; set; } = null!;
        public KTUN_DbContext(DbContextOptions<KTUN_DbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Users>(entity => { entity.ToTable(name: "Users"); });
            modelBuilder.Entity<IdentityRole<int>>(entity => { entity.ToTable(name: "AspNetRoles"); });
            modelBuilder.Entity<IdentityUserRole<int>>(entity => { entity.ToTable("AspNetUserRoles"); entity.HasKey(key => new { key.UserId, key.RoleId }); });
            modelBuilder.Entity<IdentityUserClaim<int>>(entity => { entity.ToTable("AspNetUserClaims"); });
            modelBuilder.Entity<IdentityUserLogin<int>>(entity => { entity.ToTable("AspNetUserLogins"); entity.HasKey(key => new { key.LoginProvider, key.ProviderKey }); });
            modelBuilder.Entity<IdentityRoleClaim<int>>(entity => { entity.ToTable("AspNetRoleClaims"); });
            modelBuilder.Entity<IdentityUserToken<int>>(entity => { entity.ToTable("AspNetUserTokens"); entity.HasKey(key => new { key.UserId, key.LoginProvider, key.Name }); });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.Property(e => e.FullName).IsRequired(false);
                entity.Property(e => e.Address).IsRequired(false);
                entity.Property(e => e.NFC_CardID).IsRequired(false).HasMaxLength(450);
                entity.Property(e => e.Status).IsRequired().HasDefaultValue(true);
            });

            modelBuilder.Entity<Stores>(entity =>
            {
                entity.HasKey(e => e.StoreID);
                entity.ToTable("Stores");
                entity.Property(e => e.StoreName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Status).IsRequired().HasDefaultValue(true);
            });

            modelBuilder.Entity<Suppliers>(entity =>
            {
                entity.HasKey(e => e.SupplierID);
                entity.ToTable("Suppliers");
                entity.Property(e => e.SupplierName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ContactEmail).HasMaxLength(100);
                entity.Property(e => e.Status).IsRequired().HasDefaultValue(true);
            });

            modelBuilder.Entity<LoyaltyPrograms>(entity =>
            {
                entity.HasKey(e => e.LoyaltyProgramID);
                entity.ToTable("LoyaltyPrograms");
                entity.Property(e => e.ProgramName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.DiscountRate).HasColumnType("decimal(5, 2)");
                entity.Property(e => e.Status).IsRequired().HasDefaultValue(true);
            });

            modelBuilder.Entity<Categories>(entity =>
            {
                entity.HasKey(e => e.CategoryID);
                entity.ToTable("Categories");
                entity.Property(e => e.CategoryName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Status).IsRequired().HasDefaultValue(true);
                entity.HasOne(d => d.Store)
                      .WithMany(p => p.Categories)
                      .HasForeignKey(d => d.StoreID)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Products>(entity =>
            {
                entity.HasKey(e => e.ProductID);
                entity.ToTable("Products");
                entity.Property(e => e.ProductName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.Barcode).HasMaxLength(50);
                entity.Property(e => e.ImageUrl).HasMaxLength(2048);
                entity.Property(e => e.Status).IsRequired().HasDefaultValue(true);
                entity.HasIndex(e => e.Barcode).IsUnique().HasFilter("[Barcode] IS NOT NULL");
                entity.HasOne(d => d.Store)
                      .WithMany(p => p.Products)
                      .HasForeignKey(d => d.StoreID)
                      .OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(d => d.Category)
                      .WithMany(p => p.Products)
                      .HasForeignKey(d => d.CategoryID)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<ProductSuppliers>(entity =>
            {
                entity.HasKey(e => e.ProductSupplierID);
                entity.ToTable("ProductSuppliers");
                entity.Property(e => e.SupplyDate).HasColumnType("datetime2").HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Status).IsRequired().HasDefaultValue(true);
                entity.HasOne(d => d.Product)
                      .WithMany(p => p.ProductSuppliers)
                      .HasForeignKey(d => d.ProductID)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(d => d.Supplier)
                      .WithMany(p => p.ProductSuppliers)
                      .HasForeignKey(d => d.SupplierID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.HasKey(e => e.InventoryID);
                entity.ToTable("Inventory");
                entity.Property(e => e.ChangeType).HasMaxLength(50);
                entity.Property(e => e.ChangeDate).HasColumnType("datetime2").HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Status).IsRequired().HasDefaultValue(true);
                entity.HasOne(d => d.Product)
                      .WithMany(p => p.InventoryRecords)
                      .HasForeignKey(d => d.ProductID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Sales>(entity =>
            {
                entity.HasKey(e => e.SaleID);
                entity.ToTable("Sales");
                entity.Property(e => e.SaleDate).HasColumnType("datetime2").HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Status).IsRequired().HasDefaultValue(true);
                entity.HasOne(d => d.User)
                      .WithMany(p => p.Sales)
                      .HasForeignKey(d => d.UserID)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(d => d.Store)
                      .WithMany(p => p.Sales)
                      .HasForeignKey(d => d.StoreID)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<SalesDetails>(entity =>
            {
                entity.HasKey(e => e.SaleDetailID);
                entity.ToTable("SalesDetails");
                entity.Property(e => e.PriceAtSale).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Status).IsRequired().HasDefaultValue(true);
                entity.HasOne(d => d.Sale)
                      .WithMany(p => p.SalesDetails)
                      .HasForeignKey(d => d.SaleID)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(d => d.Store)
                      .WithMany(p => p.SalesDetails)
                      .HasForeignKey(d => d.StoreID)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<UserLoyalty>(entity =>
            {
                entity.HasKey(e => e.UserLoyaltyID);
                entity.ToTable("UserLoyalty");
                entity.Property(e => e.AccumulatedPoints).HasDefaultValue(0);
                entity.Property(e => e.EnrollmentDate).HasColumnType("datetime2").HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Status).IsRequired().HasDefaultValue(true);
                entity.HasOne(d => d.User)
                      .WithMany(p => p.UserLoyalty)
                      .HasForeignKey(d => d.UserID)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(d => d.LoyaltyProgram)
                      .WithMany(p => p.UserLoyaltyEntries)
                      .HasForeignKey(d => d.LoyaltyProgramID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<KTUN_Final_Year_Project.Entities.UserStore>(entity =>
            {
                entity.HasKey(us => new { us.UserID, us.StoreID });

                entity.Property(e => e.EnrollmentDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Status).IsRequired().HasDefaultValue(true);

                entity.HasOne(us => us.User)
                    .WithMany(u => u.UserStores)
                    .HasForeignKey(us => us.UserID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(us => us.Store)
                    .WithMany(s => s.UserStores)
                    .HasForeignKey(us => us.StoreID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ProductRecommendations>(entity =>
            {
                entity.HasKey(e => e.RecommendationID);
                entity.ToTable("ProductRecommendations");
                entity.Property(e => e.RecommendationDate).HasColumnType("datetime2").HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Status).IsRequired().HasDefaultValue(true);
                entity.HasOne(d => d.User)
                      .WithMany(p => p.ProductRecommendations)
                      .HasForeignKey(d => d.UserID)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(d => d.Product)
                      .WithMany(p => p.ProductRecommendations)
                      .HasForeignKey(d => d.ProductID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ImageCache>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.ToTable("ImageCache");
                entity.Property(e => e.PageID).HasMaxLength(100);
                entity.Property(e => e.HashValue).HasMaxLength(64);
                entity.HasIndex(e => e.HashValue).IsUnique().HasFilter("[HashValue] IS NOT NULL");
                entity.Property(e => e.Status).IsRequired().HasDefaultValue(true);
                entity.HasOne(d => d.Product)
                      .WithMany()
                      .HasForeignKey(d => d.ProductID)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Orders>(entity =>
            {
                entity.HasKey(e => e.OrderID);
                entity.ToTable("Orders");
                entity.Property(e => e.OrderDate).HasColumnType("datetime2").HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50).HasDefaultValue("Pending");
                entity.Property(e => e.ShippingAddress).HasMaxLength(4000);
                entity.HasOne(d => d.User)
                      .WithMany(p => p.Orders)
                      .HasForeignKey(d => d.UserID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OrderItems>(entity =>
            {
                entity.HasKey(e => e.OrderItemID);
                entity.ToTable("OrderItems");
                entity.Property(e => e.PriceAtPurchase).HasColumnType("decimal(18, 2)");
                entity.HasOne(d => d.Order)
                      .WithMany(p => p.OrderItems)
                      .HasForeignKey(d => d.OrderID)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(d => d.Product)
                      .WithMany(p => p.OrderItems)
                      .HasForeignKey(d => d.ProductID)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Reviews>(entity =>
            {
                entity.HasKey(e => e.ReviewID);
                entity.ToTable("Reviews");
                entity.Property(e => e.Comment).HasMaxLength(4000);
                entity.Property(e => e.ReviewDate).HasColumnType("datetime2").HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Status).IsRequired().HasDefaultValue(true);
                entity.HasOne(d => d.User)
                      .WithMany(p => p.Reviews)
                      .HasForeignKey(d => d.UserID)
                      .OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(d => d.Product)
                      .WithMany(p => p.Reviews)
                      .HasForeignKey(d => d.ProductID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SupportMessages>(entity =>
            {
                entity.HasKey(e => e.MessageID);
                entity.ToTable("SupportMessages");
                entity.Property(e => e.Subject).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Message).IsRequired().HasMaxLength(4000);
                entity.Property(e => e.Timestamp).HasColumnType("datetime2").HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("Open");
                entity.HasOne(d => d.User)
                      .WithMany(p => p.SupportMessages)
                      .HasForeignKey(d => d.UserID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Users - UserLoyalty ilişkisi
            modelBuilder.Entity<Users>()
                .HasMany(u => u.UserLoyalty)
                .WithOne(ul => ul.User)
                .HasForeignKey(ul => ul.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // Users - ProductRecommendations ilişkisi
            modelBuilder.Entity<Users>()
                .HasMany(u => u.ProductRecommendations)
                .WithOne(pr => pr.User)
                .HasForeignKey(pr => pr.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // Users - Orders ilişkisi
            modelBuilder.Entity<Users>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // Users - Reviews ilişkisi
            modelBuilder.Entity<Users>()
                .HasMany(u => u.Reviews)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserID)
                .OnDelete(DeleteBehavior.NoAction);

            // Users - SupportMessages ilişkisi
            modelBuilder.Entity<Users>()
                .HasMany(u => u.SupportMessages)
                .WithOne(sm => sm.User)
                .HasForeignKey(sm => sm.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // Products - ProductRecommendations ilişkisi
            modelBuilder.Entity<Products>()
                .HasMany(p => p.ProductRecommendations)
                .WithOne(pr => pr.Product)
                .HasForeignKey(pr => pr.ProductID)
                .OnDelete(DeleteBehavior.Cascade);

            // UserFavorites configuration
            modelBuilder.Entity<UserFavorite>(entity =>
            {
                entity.HasKey(e => e.UserFavoriteID);

                // Ensure the User-Product pair is unique
                entity.HasIndex(e => new { e.UserID, e.ProductID }).IsUnique();

                // Define relationships
                entity.HasOne(d => d.User)
                      .WithMany() // If Users doesn't have a collection of UserFavorites
                      .HasForeignKey(d => d.UserID)
                      .OnDelete(DeleteBehavior.Cascade); // Delete favorites if user is deleted

                entity.HasOne(d => d.Product)
                      .WithMany() // If Product doesn't have a collection of UserFavorites
                      .HasForeignKey(d => d.ProductID)
                      .OnDelete(DeleteBehavior.Cascade); // Delete favorites if product is deleted
            });

            // UserCartItems configuration
            modelBuilder.Entity<UserCartItem>(entity =>
            {
                entity.HasKey(e => e.UserCartItemID);

                // Ensure the User-Product pair is unique
                entity.HasIndex(e => new { e.UserID, e.ProductID }).IsUnique();

                // Define relationships
                entity.HasOne(d => d.User)
                      .WithMany() // If Users doesn't have a collection of UserCartItems
                      .HasForeignKey(d => d.UserID)
                      .OnDelete(DeleteBehavior.Cascade); // Delete cart items if user is deleted

                entity.HasOne(d => d.Product)
                      .WithMany() // If Product doesn't have a collection of UserCartItems
                      .HasForeignKey(d => d.ProductID)
                      .OnDelete(DeleteBehavior.Cascade); // Delete cart items if product is deleted
            });
        }
    }
}
