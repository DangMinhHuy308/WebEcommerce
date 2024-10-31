﻿using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        [HttpGet]
        public IActionResult Register()
        {
			return View(new RegisterVM());
		}
        [HttpPost]
        public async Task <IActionResult> Register(RegisterVM vm)
        {
			if (!ModelState.IsValid) { return View(vm); }
			var checkUserByEmail = await _userManager.FindByEmailAsync(vm.Email);
			if (checkUserByEmail != null)
			{
				_notification.Error("Email already exists");
				return View(vm);
			}
			var checkUserByUsername = await _userManager.FindByNameAsync(vm.UserName);
			if (checkUserByUsername != null)
			{
				_notification.Error("Username already exists");
				return View(vm);
			}

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
				return RedirectToAction("Product");
			}
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
			return View(vm);
		}
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
	}
}
