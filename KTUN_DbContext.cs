namespace KTUN_Final_Year_Project
{
    using KTUN_Final_Year_Project.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Migrations;
    using System.Reflection.Emit;
    public class KTUN_DbContext : DbContext
    {
#nullable disable
        public DbSet<SalesDetails> SalesDetails { get; set; }

        public DbSet<Sales> Sales { get; set; }

        public DbSet<UserLoyalty> UserLoyalty { get; set; }

        public DbSet<ProductRecommendations> ProductRecommendations { get; set; }

        public DbSet<UserStore> UserStore { get; set; }

        public DbSet<CustomerFeedback> CustomerFeedback { get; set; }

        public DbSet<Users> Users { get; set; }

        public DbSet<Inventory> Inventory { get; set; }

        public DbSet<ProductSuppliers> ProductSuppliers { get; set; }

        public DbSet<Products> Products { get; set; }

        public DbSet<Categories> Categories { get; set; }

        public DbSet<Stores> Stores { get; set; }

        public DbSet<Suppliers> Suppliers { get; set; }

        public DbSet<LoyaltyPrograms> LoyaltyPrograms { get; set; }

        public KTUN_DbContext(DbContextOptions<KTUN_DbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder Modelbuilder)
        {
            Modelbuilder.Entity<SalesDetails>().HasKey(sd => sd.SaleDetailID);
            Modelbuilder.Entity<SalesDetails>().ToTable("SalesDetails");

            Modelbuilder.Entity<Sales>().HasKey(s => s.SaleID);
            Modelbuilder.Entity<Sales>().ToTable("Sales");

            Modelbuilder.Entity<UserLoyalty>().HasKey(ul => ul.UserLoyaltyID);
            Modelbuilder.Entity<UserLoyalty>().ToTable("UserLoyalty");

            Modelbuilder.Entity<ProductRecommendations>().HasKey(pr => pr.RecommendationID);
            Modelbuilder.Entity<ProductRecommendations>().ToTable("ProductRecommendations");

            Modelbuilder.Entity<UserStore>().HasKey(us => us.UserStoreID);
            Modelbuilder.Entity<UserStore>().ToTable("UserStore");

            Modelbuilder.Entity<CustomerFeedback>().HasKey(cf => cf.CustomerFeedbackID);
            Modelbuilder.Entity<CustomerFeedback>().ToTable("CustomerFeedback");

            Modelbuilder.Entity<Users>().HasKey(u => u.UserID);
            Modelbuilder.Entity<Users>().HasIndex(u => u.NFC_CardID).IsUnique();
            Modelbuilder.Entity<Users>().ToTable("Users");

            Modelbuilder.Entity<Inventory>().HasKey(i => i.InventoryID);
            Modelbuilder.Entity<Inventory>().ToTable("Inventory");

            Modelbuilder.Entity<ProductSuppliers>().HasKey(ps => ps.ProductSupplierID);
            Modelbuilder.Entity<ProductSuppliers>().ToTable("ProductSuppliers");

            Modelbuilder.Entity<Products>().HasKey(p => p.ProductID);
            Modelbuilder.Entity<Products>().ToTable("Products");

            Modelbuilder.Entity<Categories>().HasKey(c => c.CategoryID);
            Modelbuilder.Entity<Categories>().ToTable("Categories");

            Modelbuilder.Entity<Stores>().HasKey(st => st.StoreID);
            Modelbuilder.Entity<Stores>().ToTable("Stores");

            Modelbuilder.Entity<Suppliers>().HasKey(su => su.SupplierID);
            Modelbuilder.Entity<Suppliers>().ToTable("Suppliers");

            Modelbuilder.Entity<LoyaltyPrograms>().HasKey(lp => lp.LoyaltyProgramID);
            Modelbuilder.Entity<LoyaltyPrograms>().ToTable("LoyaltyPrograms");


            Modelbuilder.Entity<SalesDetails>()
            .HasOne(sd => sd.Sale)
            .WithMany()
            .HasForeignKey(sd => sd.SaleID);

            Modelbuilder.Entity<SalesDetails>()
            .HasOne(sd => sd.Store)
            .WithMany()
            .HasForeignKey(sd => sd.StoreID);



            Modelbuilder.Entity<Sales>()
            .HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserID)
            .HasPrincipalKey(u => u.UserID);

            Modelbuilder.Entity<Sales>()
            .HasOne(s => s.Store)
            .WithMany()
            .HasForeignKey(s => s.StoreID);




            Modelbuilder.Entity<UserLoyalty>()
            .HasOne(ul => ul.User)
            .WithMany()
            .HasForeignKey(ul => ul.UserID)
            .HasPrincipalKey(u => u.UserID);


            Modelbuilder.Entity<UserLoyalty>()
             .HasOne(ul => ul.LoyaltyProgram)
             .WithMany()
             .HasForeignKey(ul => ul.LoyaltyProgramID);




            Modelbuilder.Entity<ProductRecommendations>()
            .HasOne(pr => pr.User)
            .WithMany()
            .HasForeignKey(pr => pr.UserID)
            .HasPrincipalKey(u => u.UserID);

            Modelbuilder.Entity<ProductRecommendations>()
           .HasOne(pr => pr.Product)
           .WithMany()
           .HasForeignKey(pr => pr.ProductID);




            Modelbuilder.Entity<UserStore>()
            .HasOne(us => us.User)
            .WithMany()
            .HasForeignKey(us => us.UserID)
            .HasPrincipalKey(u => u.UserID);

            Modelbuilder.Entity<UserStore>()
           .HasOne(us => us.Store)
           .WithMany()
           .HasForeignKey(us => us.StoreID);




            Modelbuilder.Entity<CustomerFeedback>()
            .HasOne(cf => cf.User)
            .WithMany()
            .HasForeignKey(cf => cf.UserID)
            .HasPrincipalKey(u => u.UserID);

            Modelbuilder.Entity<CustomerFeedback>()
            .HasOne(cf => cf.Product)
            .WithMany()
            .HasForeignKey(cf => cf.ProductID);




            Modelbuilder.Entity<Inventory>()
            .HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductID);




            Modelbuilder.Entity<ProductSuppliers>()
            .HasOne(ps => ps.Supplier)
            .WithMany()
            .HasForeignKey(ps => ps.SupplierID);

            Modelbuilder.Entity<ProductSuppliers>()
            .HasOne(ps => ps.Product)
            .WithMany()
            .HasForeignKey(ps => ps.ProductID);



            Modelbuilder.Entity<Products>()
            .HasOne(p => p.Store)
            .WithMany()
            .HasForeignKey( p => p.StoreID);


            Modelbuilder.Entity<Products>()
            .HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryID);

            Modelbuilder.Entity<Categories>()
                .HasOne(c => c.Store)
                .WithMany()
                .HasForeignKey(c => c.StoreID);

            base.OnModelCreating(Modelbuilder);
        }
    }
}
