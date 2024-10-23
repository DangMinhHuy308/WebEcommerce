using Microsoft.AspNetCore.Identity;
using WebEcommerce.Data;
using WebEcommerce.Models;

namespace WebEcommerce.Utilies
{
	public class DbInitializer: IDbInitializer
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		public DbInitializer(ApplicationDbContext context,
							   UserManager<ApplicationUser> userManager,
							   RoleManager<IdentityRole> roleManager)
		{
			_context = context;
			_userManager = userManager;
			_roleManager = roleManager;
		}
		public void Initialize()
		{
			if (!_roleManager.RoleExistsAsync(WebsiteRoles.WebsiteAdmin).GetAwaiter().GetResult())
			{
				_roleManager.CreateAsync(new IdentityRole(WebsiteRoles.WebsiteAdmin)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(WebsiteRoles.WebsiteAuthor)).GetAwaiter().GetResult();
				_userManager.CreateAsync(new ApplicationUser()
				{
					UserName = "admin@gmail.com",
					Email = "admin@gmail.com",
					FirstName = "Super",
					LastName = "Admin"
				}, "Admin@0011").Wait();

				var appUser = _context.ApplicationUsers!.FirstOrDefault(x => x.Email == "admin@gmail.com");
				if (appUser != null)
				{
					_userManager.AddToRoleAsync(appUser, WebsiteRoles.WebsiteAdmin).GetAwaiter().GetResult();
				}

				_context.SaveChanges();

			}
		}
	}
}
