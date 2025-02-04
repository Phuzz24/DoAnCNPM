using Microsoft.EntityFrameworkCore;

namespace DoAnCNPM.Models
{
    public class DBDienThoaiContext : DbContext
    {
        public DBDienThoaiContext(DbContextOptions<DBDienThoaiContext> options)
            : base(options) { }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<User> Users { get; set; }  // Thêm DbSet cho Users
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<RevenueStatistic> RevenueStatistics { get; set; }
        public DbSet<AdminActivity> AdminActivities { get; set; }

        public DbSet<Cart> Carts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cấu hình quan hệ Product - Brand (1 Brand có nhiều Product)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.Brand_ID)
                .OnDelete(DeleteBehavior.SetNull);

            // Cấu hình quan hệ Product - Category (1 Category có nhiều Product)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.Category_ID)
                .OnDelete(DeleteBehavior.SetNull);

            // Liên kết một-nhiều giữa User và Customer
            modelBuilder.Entity<User>()
                .HasMany(u => u.Customers)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.User_ID); // Chỉ rõ cột ngoại là User_ID
                                                // Thiết lập khóa ngoại cho Cart -> Customer
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Customer)  // Mỗi Cart thuộc về một Customer
                .WithMany(cu => cu.Carts)  // Một Customer có nhiều Cart
                .HasForeignKey(c => c.Customer_ID)  // Khóa ngoại là Customer_ID
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete nếu cần

            // Thiết lập khóa ngoại cho Cart -> Product
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Product)  // Mỗi Cart thuộc về một Product
                .WithMany(p => p.Carts)  // Một Product có thể có nhiều Cart
                .HasForeignKey(c => c.Product_ID)  // Khóa ngoại là Product_ID
                .OnDelete(DeleteBehavior.Restrict);  // Nếu không muốn xóa Produ

            modelBuilder.Entity<Order>()
               .HasOne(o => o.Customer)
               .WithMany(c => c.Orders)
               .HasForeignKey(o => o.Customer_ID);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.Order_ID);

            base.OnModelCreating(modelBuilder);
        }
    }
}