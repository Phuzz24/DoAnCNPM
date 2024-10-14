using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DoAnCNPM.Models
{
    public class DBDienThoaiContext : IdentityDbContext<User> // Thay DbContext thành IdentityDbContext<User>
    {
        public DBDienThoaiContext(DbContextOptions<DBDienThoaiContext> options)
            : base(options) { }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }  // Thêm DbSet cho Categories
        public DbSet<Product> Products { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<RevenueStatistic> RevenueStatistics { get; set; }
        public DbSet<Cart> Carts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cấu hình quan hệ Product - Brand (1 Brand có nhiều Product)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.Brand_ID)
                .OnDelete(DeleteBehavior.SetNull); // Khi xóa Brand, Brand_ID trong Product sẽ được đặt về null

            // Cấu hình quan hệ Product - Category (1 Category có nhiều Product)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.Category_ID)
                .OnDelete(DeleteBehavior.SetNull); // Khi xóa Category, Category_ID trong Product sẽ được đặt về null

            base.OnModelCreating(modelBuilder);
        }
    }
}
