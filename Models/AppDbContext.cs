using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ООО_обувь.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrdersProduct> OrdersProducts { get; set; }

    public virtual DbSet<PickUpPoint> PickUpPoints { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductStockItem> ProductStockItems { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Street> Streets { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost; Database=bookstore; Username=postgres; Password=postgres");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Categories_pkey");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("City_pkey");

            entity.ToTable("City");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Manufacturers_pkey");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Orders_pkey");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.АдресПунктаВыдачи).HasColumnName("Адрес пункта выдачи");
            entity.Property(e => e.ДатаДоставки).HasColumnName("Дата доставки");
            entity.Property(e => e.ДатаЗаказа).HasColumnName("Дата заказа");
            entity.Property(e => e.КодДляПолучения).HasColumnName("Код для получения");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_11_Orders_status_id");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_10_Orders_user_id");

            entity.HasOne(d => d.АдресПунктаВыдачиNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.АдресПунктаВыдачи)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_9_Orders_Адрес пункта выдачи");
        });

        modelBuilder.Entity<OrdersProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("OrdersProduct_pkey");

            entity.ToTable("OrdersProduct");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Count).HasColumnName("count");
            entity.Property(e => e.OrdersId).HasColumnName("orders_id");
            entity.Property(e => e.Price)
                .HasPrecision(12, 2)
                .HasColumnName("price");
            entity.Property(e => e.ProductId).HasColumnName("product_id");

            entity.HasOne(d => d.Orders).WithMany(p => p.OrdersProducts)
                .HasForeignKey(d => d.OrdersId)
                .HasConstraintName("fk_12_OrdersProduct_orders_id");

            entity.HasOne(d => d.Product).WithMany(p => p.OrdersProducts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_13_OrdersProduct_product_id");
        });

        modelBuilder.Entity<PickUpPoint>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Pick-up_point_pkey");

            entity.ToTable("Pick-up_point");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.House).HasColumnName("house");
            entity.Property(e => e.Index).HasColumnName("index");
            entity.Property(e => e.StreetId).HasColumnName("street_id");

            entity.HasOne(d => d.Street).WithMany(p => p.PickUpPoints)
                .HasForeignKey(d => d.StreetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_3_Pick-up_point_street_id");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Products_pkey");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Article).HasColumnName("article");
            entity.Property(e => e.CategoriesId).HasColumnName("categories_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.ManufacturersId).HasColumnName("manufacturers_id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Price)
                .HasPrecision(12, 2)
                .HasColumnName("price");
            entity.Property(e => e.Sale)
                .HasPrecision(5, 2)
                .HasColumnName("sale");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.UnitsId).HasColumnName("units_id");

            entity.HasOne(d => d.Categories).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoriesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_7_Products_categories_id");

            entity.HasOne(d => d.Manufacturers).WithMany(p => p.Products)
                .HasForeignKey(d => d.ManufacturersId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_6_Products_manufacturers_id");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Products)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_5_Products_supplier_id");

            entity.HasOne(d => d.Units).WithMany(p => p.Products)
                .HasForeignKey(d => d.UnitsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_4_Products_units_id");
        });

        modelBuilder.Entity<ProductStockItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Product_Stock_Items_pkey");

            entity.ToTable("Product_Stock_Items");

            entity.HasIndex(e => e.ProductId, "Product_Stock_Items_product_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Count).HasColumnName("count");
            entity.Property(e => e.ProductId).HasColumnName("product_id");

            entity.HasOne(d => d.Product).WithOne(p => p.ProductStockItem)
                .HasForeignKey<ProductStockItem>(d => d.ProductId)
                .HasConstraintName("fk_8_Product_Stock_Items_product_id");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Roles_pkey");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Status_pkey");

            entity.ToTable("Status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Street>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Street_pkey");

            entity.ToTable("Street");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.Name).HasColumnName("name");

            entity.HasOne(d => d.City).WithMany(p => p.Streets)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_2_Street_city_id");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Suppliers_pkey");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Units_pkey");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Users_pkey");

            entity.HasIndex(e => e.Login, "Users_login_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Login).HasColumnName("login");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.Patronyc).HasColumnName("patronyc");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Surname).HasColumnName("surname");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("fk_1_Users_role_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
