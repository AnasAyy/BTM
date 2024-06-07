using BTMBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BTMBackend.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PublicList> PublicLists { get; set; }
        public DbSet<FCMtoken> FCMtokens { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<ContactUs> ContactUs { get; set; }
        public DbSet<ExternalLink> ExternalLinks { get; set; }
        public DbSet<Statistic> Statistics { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
        public DbSet<Accessories_Features> accessories_Features { get; set; }
        public DbSet<AboutUs> AboutUs { get; set; }
        public DbSet<AdSlider> AdSliders { get; set; }
        public DbSet<OTPMessage> OTPMessages { get; set; }
        public DbSet<CustomerProduct> CustomerProducts { get; set; }
        public DbSet<CustomerProductPart> CustomerProductParts { get; set; }
        public DbSet<WhoWeAre> WhoWeAres { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Notification> Notifications { get; set; }


        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .Property(p => p.OfferPrice)
                .HasPrecision(18, 2); // Adjust precision and scale as needed

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2); // Adjust precision and scale as needed
        }
    }
}
