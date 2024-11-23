using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebEcommerce.Models;

namespace WebEcommerce.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
		public DbSet<ApplicationUser>? ApplicationUsers { get; set; }
		public DbSet<Product>? Products { get; set; }
		public DbSet<Category>? Categories { get; set; }
		public DbSet<Invoice>? Invoices { get; set; }
		public DbSet<InvoiceDetail>? InvoiceDetails { get; set; }
		public DbSet<Supplier>? Suppliers { get; set; }

        public DbSet<Message>? Messages { get; set; }
        public DbSet<Coupon>? Coupons { get; set; }
        public DbSet<Shipping>? Shippings { get; set; }


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// config the Product 
			modelBuilder.Entity<Product>()
				.HasKey(p => p.ProductId);

			modelBuilder.Entity<Product>()
				.Property(p => p.ProductName)
				.IsRequired()
				.HasMaxLength(100);
			modelBuilder.Entity<Product>()
				.Property(p => p.Price)
				.HasColumnType("decimal(18,2)");

			modelBuilder.Entity<Product>()
				.Property(p => p.OriginalPrice)
				.HasColumnType("decimal(18,2)");

			modelBuilder.Entity<Product>()
				.Property(p => p.Sale)
				.HasColumnType("decimal(18,2)");
			// config the Category 
			modelBuilder.Entity<Category>()
				.HasKey(c => c.CategoryId);

			modelBuilder.Entity<Category>()
				.Property(c => c.CategoryName)
				.IsRequired()
				.HasMaxLength(50);

			// config the Invoice 
			modelBuilder.Entity<Invoice>()
				.HasKey(i => i.InvoiceId);

			modelBuilder.Entity<Invoice>()
				.HasOne(i => i.ApplicationUser)
				.WithMany(u => u.Invoices)
				.HasForeignKey(i => i.ApplicationUserId);

			modelBuilder.Entity<InvoiceDetail>()
				.HasKey(id => id.Id);



			// config the Invoice Detail
			modelBuilder.Entity<InvoiceDetail>()
				 .HasOne(id => id.Invoice)
				 .WithMany(i => i.InvoiceDetails)
				 .HasForeignKey(id => id.InvoiceId);

			modelBuilder.Entity<InvoiceDetail>()
				.HasOne(id => id.Product)
				.WithMany(p => p.InvoiceDetails)
				.HasForeignKey(id => id.ProductId);


			// config the Supplier 
			modelBuilder.Entity<Supplier>()
				.HasKey(s => s.SupplierId);

			modelBuilder.Entity<Supplier>()
				.Property(s => s.CompanyName)
				.IsRequired()
				.HasMaxLength(100);
			//config the Message
			modelBuilder.Entity<Message>()
				.HasOne(m => m.FromUser)
				.WithMany(u => u.SentMessages)
				.HasForeignKey(m => m.FromUserId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Message>()
				.HasOne(m => m.ToUser)
				.WithMany(u => u.ReceivedMessages)
				.HasForeignKey(m => m.ToUserId)
				.OnDelete(DeleteBehavior.Restrict);
			//config the Coupon
            modelBuilder.Entity<Coupon>()
				.HasKey(c => c.Id);
			//config the Shipping
			modelBuilder.Entity<Shipping>()
				.HasKey(c => c.Id);
		}
	}


}
