using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using WebApplication1.Models;

namespace WebApplication1.Data
{
	public class ApplicationDbContext : DbContext
	{

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
				
		}

		public DbSet<Category> category { get; set; }

		public DbSet<Product> Product { get; set; }

	}
}
