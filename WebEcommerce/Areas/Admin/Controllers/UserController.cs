using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebEcommerce.Models;
using WebEcommerce.Utilies;
using WebEcommerce.ViewModels;

namespace WebEcommerce.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class UserController : Controller
    {
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly INotyfService _notification;
		public UserController(UserManager<ApplicationUser> userManager,
									SignInManager<ApplicationUser> signInManager,
									INotyfService notyfService)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_notification = notyfService;
		}
		[Authorize(Roles ="Admin")]
		[HttpGet]
		public async Task<IActionResult> Index()
        {
			var users  = await _userManager.Users.ToListAsync();
			var vm = users.Select(x=> new UserVM()
			{
				Id = x.Id,
				UserName = x.UserName,
				FirstName = x.FirstName,
				LastName = x.LastName,	
				Email = x.Email
			}).ToList();
			foreach (var user in vm) { 
				var singleUser = await _userManager.FindByIdAsync(user.Id);
				var role = await _userManager.GetRolesAsync(singleUser);
				user.Role = role.FirstOrDefault();
			}
            return View(vm);
        }
        [HttpGet("admin/login")]
        public IActionResult Login()
		{
            if (!HttpContext.User.Identity!.IsAuthenticated)
            {
                return View(new LoginVM());
            }
            return RedirectToAction("Index", "User", new { area = "Admin" });
        }
		[HttpPost("admin/login")]
		public async Task<IActionResult> Login (LoginVM vm)
		{
			if (!ModelState.IsValid) { return View(vm); }
			var existingUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == vm.Username);
			if (existingUser == null) {
				_notification.Error("Username does not exist");
				return View(vm);
			}
			var verifyPassword = await _userManager.CheckPasswordAsync(existingUser, vm.Password);
			if (!verifyPassword) {
                _notification.Error("Password does not exist");
                return View(vm);
            }
			await _signInManager.PasswordSignInAsync(vm.Username, vm.Password, vm.RememberMe, true);
			_notification.Success("Login Successful");
            return RedirectToAction("Index","User", new{area ="Admin" });
        }
        [HttpGet]
        public IActionResult register()
        {
           return View(new RegisterVM());
        }
		[HttpPost]
		public async Task<IActionResult> register(RegisterVM vm)
		{
			if (!ModelState.IsValid) { return View(vm); }
			var checkUserByEmail = await _userManager.FindByEmailAsync(vm.Email);
			if (checkUserByEmail != null) {
				_notification.Error("Email don't exist");
				return View(vm);	

            }
			var checkUserByUserName = await _userManager.FindByNameAsync(vm.UserName);
			if (checkUserByUserName != null) {
				_notification.Error("User already exists");
				return View(vm);

            }
			var applicationUser = new ApplicationUser()
			{
				Email = vm.Email,
				UserName = vm.UserName,
				FirstName = vm.FirstName,
				LastName = vm.LastName,

			};
			var result = await _userManager.CreateAsync(applicationUser, vm.Password);
			if (result.Succeeded) { 
				if (vm.IsAdmin) {
					await _userManager.AddToRoleAsync(applicationUser,WebsiteRoles.WebsiteAdmin);
				}
				else
				{
                    await _userManager.AddToRoleAsync(applicationUser, WebsiteRoles.WebsiteAuthor);
                    _notification.Success("User registered successfully");
                    return RedirectToAction("Index", "User", new { area = "Admin" });
                }
            }

            return View(vm);
		}
        [Authorize(Roles = "Admin")]
        [HttpGet]
		public async Task<IActionResult> ResetPassword(string id)
		{
			var existingUser = await _userManager.FindByIdAsync(id);
			if(existingUser == null)
			{
				_notification.Error("User don't exist");
				return View();
			}
			var vm = new ResetPasswordVM()
			{
				Id = existingUser.Id,
				UserName = existingUser.UserName,
			};

            return View(vm);
		}
        [Authorize(Roles = "Admin")]
        [HttpPost]
		public async Task<IActionResult> ResetPasswordAsync(ResetPasswordVM vm)
		{
			if (!ModelState.IsValid) { return View(vm); }
			var existingUser = await _userManager.FindByIdAsync(vm.Id);
			if (existingUser != null) {
				_notification.Error("User don't exist");
				return View(vm);	

            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
            var result = await _userManager.ResetPasswordAsync(existingUser, token, vm.NewPassword);
            if (result.Succeeded)
            {
                _notification.Success("Password reset succuful");
                return RedirectToAction(nameof(Index));
            }
            return View(vm);
		}
        [HttpPost]
        public IActionResult Logout()
		{
			_signInManager.SignOutAsync();
			_notification.Success("Logout successfull");
			return RedirectToAction("Index", "Home", new { area = "" });
        }
        [HttpGet("AccessDenied")]
        [Authorize]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
