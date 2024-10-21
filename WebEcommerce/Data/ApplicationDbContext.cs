using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebEcommerce.Models;

namespace WebEcommerce.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
		public DbSet<ApplicationUser> ApplicationUsers { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Invoice> Invoices { get; set; }
		public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
		public DbSet<Status> Statuses { get; set; }
		public DbSet<Supplier> Suppliers { get; set; }
	}

}
