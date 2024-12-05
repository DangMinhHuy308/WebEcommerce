using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebEcommerce.Data;
using WebEcommerce.Models;
using WebEcommerce.Utilies;
using WebEcommerce.ViewModels;

namespace WebEcommerce.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly INotyfService _notification;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
		private readonly IWebHostEnvironment _webHostEnvironment;
        // Constructor để khởi tạo cấu hình cho class
        public AccountController(UserManager<ApplicationUser> userManager,
                                    SignInManager<ApplicationUser> signInManager,
                                    INotyfService notyfService, ApplicationDbContext context, IMapper mapper,IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _notification = notyfService;
            _mapper = mapper;
			_webHostEnvironment = webHostEnvironment;	
        }
        // Hiển thị thông tin người dùng
        [HttpGet]
		public async Task<IActionResult> Profile()
		{
            // Lấy thông tin người dùng hiện tại
            var user = await _userManager.GetUserAsync(User);
			var vm = new ProfileVM
			{
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email,
				PhoneNumber = user.PhoneNumber,
				Address = user.Address,
				Gender = user.Gender,
				Image = user.Image 
			};

			return View(vm);
		}
        // Cập nhật hồ sơ người dùng
        [HttpPost]
		public async Task<IActionResult> Profile(ProfileVM vm)
		{
            // Kiểm tra tính hợp lệ của dữ liệu
            if (!ModelState.IsValid) { return View(vm); }
            // Lấy thông tin người dùng hiện tại
            var user = await _userManager.GetUserAsync(User);
            // Cập nhật thông tin người dùng
            user.FirstName = vm.FirstName;
			user.LastName = vm.LastName;
			user.Email = vm.Email;
			user.PhoneNumber = vm.PhoneNumber;
			user.Address = vm.Address;
			user.Gender = vm.Gender;

			if (vm.Thumbnail != null)
			{
				user.Image = UploadImage(vm.Thumbnail); 
				vm.Image = user.Image;
			}

			_context.ApplicationUsers.Update(user);
			await _context.SaveChangesAsync();
			_notification.Success("Profile updated successfully!");
			return View(vm);
		}
        // Hiển thị trang đăng ký
        [HttpGet]
        public IActionResult Register()
        {
			return View(new RegisterVM());
		}
        // Xử lý đăng ký người dùng
        [HttpPost]
        public async Task <IActionResult> Register(RegisterVM vm)
        {
			// Kiểm tra tính hợp lệ của dữ liệu
            if (!ModelState.IsValid) { return View(vm); }
            // Kiểm tra email đã tồn tại chưa
            var checkUserByEmail = await _userManager.FindByEmailAsync(vm.Email);
			if (checkUserByEmail != null)
			{
				_notification.Error("Email already exists");
				return View(vm);
			}
            // Kiểm tra tên người dùng đã tồn tại chưa
            var checkUserByUsername = await _userManager.FindByNameAsync(vm.UserName);
			if (checkUserByUsername != null)
			{
				_notification.Error("Username already exists");
				return View(vm);
			}
            // Tạo đối tượng user mới
            var applicationUser = new ApplicationUser()
			{
				Email = vm.Email,
				UserName = vm.UserName,
				FirstName = vm.FirstName,
				LastName = vm.LastName,
				PhoneNumber = vm.PhoneNumber,
				Address = vm.Address,
				Gender = vm.Gender,
			};
            // Kiểm tra xem có ảnh đại diện không
            if (vm.Thumbnail != null)
			{
				applicationUser.Image = UploadImage(vm.Thumbnail);
			}

			var result = await _userManager.CreateAsync(applicationUser, vm.Password);
			if (result.Succeeded)
			{
				if (vm.IsAdmin)
				{
					await _userManager.AddToRoleAsync(applicationUser, WebsiteRoles.WebsiteAdmin);
				}
				else
				{
					await _userManager.AddToRoleAsync(applicationUser, WebsiteRoles.WebsiteCustomer);
				}
				_notification.Success("User registered successfully");
				return RedirectToAction("Index", "Home");
			}
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
			return View(vm);
		}
		// Hiển thị trang đăng nhập
		[HttpGet]
		public IActionResult Login()
		{
			// kiểm tra người dùng đã đăng nhập chưa
			if (!HttpContext.User.Identity!.IsAuthenticated)
			{
				return View(new LoginVM());
			}
			return RedirectToAction("Index", "Home");
		}
		// Xư lí đăng nhập
		[HttpPost]
		public async Task<IActionResult> Login(LoginVM vm)
		{
            // Kiểm tra tính hợp lệ của dữ liệu
            if (!ModelState.IsValid) { return View(vm); }
			// Kiểm tra sự tồn tại của tài khoản nếu không có thì thông báo không tồn tại tài khoản
			var existingUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == vm.Username);
			if (existingUser == null)
			{
				_notification.Error("Username does not exist");
				return View(vm);
			}
			// Kiểm tra mật khẩu của tài khoản 
			var verifyPassword = await _userManager.CheckPasswordAsync(existingUser, vm.Password);
			if (!verifyPassword)
			{
				_notification.Error("Password does not exist");
				return View(vm);
			}
			// Đăng nhập người dùng
			await _signInManager.PasswordSignInAsync(vm.Username, vm.Password, vm.RememberMe, true);
			_notification.Success("Login Successful");
			return RedirectToAction("Index", "Home");
		}
        // Đăng xuất tài khoản
        public async Task<IActionResult> Logout()
        {
			await HttpContext. SignOutAsync();
            await _signInManager.SignOutAsync();
            _notification.Success("Logout successfull");
            return RedirectToAction("Index", "Home");
        }
		// Upload hình ảnh
        private string UploadImage(IFormFile file)
		{
			string uniqueFileName = "";
			var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "thumbnails");
			uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
			var filePath = Path.Combine(folderPath, uniqueFileName);
			using (FileStream fileStream = System.IO.File.Create(filePath))
			{
				file.CopyTo(fileStream);
			}
			return uniqueFileName;

		}
		public async Task LoginByGoogle()
		{
			await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
				new AuthenticationProperties
				{
					RedirectUri = Url.Action("GoogleResponse")
				});
		}
		public async Task<ActionResult> GoogleResponse(RegisterVM vm)
		{
			var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
			if(!result.Succeeded) { return RedirectToAction("Login"); }
			var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim  => new
			{
				claim.Issuer,
				claim.OriginalIssuer,
				claim.Type,
				claim.Value
			});
			var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
			string UserName = email.Split('@')[0];
			var existingUser= await _userManager.FindByNameAsync(email);
			if (existingUser == null) {
				var password = new PasswordHasher<ApplicationUser>();
				var hashedPassword = password.HashPassword(null, "123456789");

                var newUser = new ApplicationUser
                {
                    UserName = UserName,
                    Email = email,
                }; ;
				newUser.PasswordHash = hashedPassword;
				var createUser = await _userManager.CreateAsync(newUser);
				if (!createUser.Succeeded) {
					_notification.Error("Đăng ký tài khoản thất bại");
					return RedirectToAction("Login");
				}
				else
				{
					await _signInManager.SignInAsync(newUser, isPersistent: false);
                    _notification.Success("Đăng ký tài khoản thành công");
					return RedirectToAction("Index", "Home");

                }
			}
            else
            {
				await _signInManager.SignInAsync(existingUser, isPersistent: false);
			}
			return RedirectToAction("Login");
		}
	}
}
