namespace KTUN_Final_Year_Project
{
    using KTUN_Final_Year_Project.Entities;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Migrations;
    using System.Reflection.Emit;
    public class KTUN_DbContext : IdentityDbContext<Users, IdentityRole<int>, int>
    {
#nullable disable
        public DbSet<SalesDetails> SalesDetails { get; set; }

        public DbSet<Sales> Sales { get; set; }

        public DbSet<UserLoyalty> UserLoyalty { get; set; }

        public DbSet<ProductRecommendations> ProductRecommendations { get; set; }

        public DbSet<Entities.UserStore> UserStore { get; set; }

        public DbSet<CustomerFeedback> CustomerFeedback { get; set; }

        public DbSet<Inventory> Inventory { get; set; }

        public DbSet<ProductSuppliers> ProductSuppliers { get; set; }

        public DbSet<Products> Products { get; set; }

        public DbSet<Categories> Categories { get; set; }

        public DbSet<Stores> Stores { get; set; }

        public DbSet<Suppliers> Suppliers { get; set; }

        public DbSet<LoyaltyPrograms> LoyaltyPrograms { get; set; }
        
        public DbSet<ImageCache> ImageCache { get; set; }
        public KTUN_DbContext(DbContextOptions<KTUN_DbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Users>().ToTable("Users");

            modelBuilder.Entity<Users>().HasIndex(u => u.NFC_CardID).IsUnique();

            modelBuilder.Entity<SalesDetails>().HasKey(sd => sd.SaleDetailID);
            modelBuilder.Entity<SalesDetails>().ToTable("SalesDetails");
            modelBuilder.Entity<SalesDetails>(entity =>
            {
                entity.Property(sd => sd.PriceAtSale).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<Sales>().HasKey(s => s.SaleID);
            modelBuilder.Entity<Sales>().ToTable("Sales");
            modelBuilder.Entity<Sales>(entity =>
            {
                entity.Property(s => s.TotalAmount).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<UserLoyalty>().HasKey(ul => ul.UserLoyaltyID);
            modelBuilder.Entity<UserLoyalty>().ToTable("UserLoyalty");

            modelBuilder.Entity<ProductRecommendations>().HasKey(pr => pr.RecommendationID);
            modelBuilder.Entity<ProductRecommendations>().ToTable("ProductRecommendations");

            modelBuilder.Entity<Entities.UserStore>().HasKey(us => us.UserStoreID);
            modelBuilder.Entity<Entities.UserStore>().ToTable("UserStore");

            modelBuilder.Entity<CustomerFeedback>().HasKey(cf => cf.CustomerFeedbackID);
            modelBuilder.Entity<CustomerFeedback>().ToTable("CustomerFeedback");

            modelBuilder.Entity<Inventory>().HasKey(i => i.InventoryID);
            modelBuilder.Entity<Inventory>().ToTable("Inventory");

            modelBuilder.Entity<ProductSuppliers>().HasKey(ps => ps.ProductSupplierID);
            modelBuilder.Entity<ProductSuppliers>().ToTable("ProductSuppliers");

            modelBuilder.Entity<Products>().HasKey(p => p.ProductID);
            modelBuilder.Entity<Products>().ToTable("Products");
            modelBuilder.Entity<Products>(entity =>
            {
                entity.Property(p => p.Price).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<Categories>().HasKey(c => c.CategoryID);
            modelBuilder.Entity<Categories>().ToTable("Categories");

            modelBuilder.Entity<Stores>().HasKey(st => st.StoreID);
            modelBuilder.Entity<Stores>().ToTable("Stores");

            modelBuilder.Entity<Suppliers>().HasKey(su => su.SupplierID);
            modelBuilder.Entity<Suppliers>().ToTable("Suppliers");

            modelBuilder.Entity<LoyaltyPrograms>().HasKey(lp => lp.LoyaltyProgramID);
            modelBuilder.Entity<LoyaltyPrograms>().ToTable("LoyaltyPrograms");
            modelBuilder.Entity<LoyaltyPrograms>(entity =>
            {
                entity.Property(lp => lp.DiscountRate).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<ImageCache>().HasKey(ic => ic.ID);
            modelBuilder.Entity<ImageCache>().ToTable("ImageCache");

            modelBuilder.Entity<Sales>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserID);

            modelBuilder.Entity<UserLoyalty>()
                .HasOne(ul => ul.User)
                .WithMany()
                .HasForeignKey(ul => ul.UserID);

            modelBuilder.Entity<ProductRecommendations>()
                .HasOne(pr => pr.User)
                .WithMany()
                .HasForeignKey(pr => pr.UserID);

            modelBuilder.Entity<Entities.UserStore>()
                .HasOne(us => us.User)
                .WithMany()
                .HasForeignKey(us => us.UserID);

            modelBuilder.Entity<CustomerFeedback>()
                .HasOne(cf => cf.User)
                .WithMany()
                .HasForeignKey(cf => cf.UserID);

            modelBuilder.Entity<SalesDetails>()
                .HasOne(sd => sd.Sale)
                .WithMany()
                .HasForeignKey(sd => sd.SaleID);

            modelBuilder.Entity<SalesDetails>()
                .HasOne(sd => sd.Store)
                .WithMany()
                .HasForeignKey(sd => sd.StoreID);

            modelBuilder.Entity<Sales>()
                .HasOne(s => s.Store)
                .WithMany()
                .HasForeignKey(s => s.StoreID);

            modelBuilder.Entity<UserLoyalty>()
                .HasOne(ul => ul.LoyaltyProgram)
                .WithMany()
                .HasForeignKey(ul => ul.LoyaltyProgramID);

            modelBuilder.Entity<ProductRecommendations>()
                .HasOne(pr => pr.Product)
                .WithMany()
                .HasForeignKey(pr => pr.ProductID);

            modelBuilder.Entity<Entities.UserStore>()
                .HasOne(us => us.Store)
                .WithMany()
                .HasForeignKey(us => us.StoreID);

            modelBuilder.Entity<CustomerFeedback>()
                .HasOne(cf => cf.Product)
                .WithMany()
                .HasForeignKey(cf => cf.ProductID);

            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductID);

            modelBuilder.Entity<ProductSuppliers>()
                .HasOne(ps => ps.Supplier)
                .WithMany()
                .HasForeignKey(ps => ps.SupplierID);

            modelBuilder.Entity<ProductSuppliers>()
                .HasOne(ps => ps.Product)
                .WithMany()
                .HasForeignKey(ps => ps.ProductID);

            modelBuilder.Entity<Products>()
                .HasOne(p => p.Store)
                .WithMany()
                .HasForeignKey(p => p.StoreID);

            modelBuilder.Entity<Products>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryID);

            modelBuilder.Entity<Categories>()
                .HasOne(c => c.Store)
                .WithMany()
                .HasForeignKey(c => c.StoreID);
        }
    }
}
