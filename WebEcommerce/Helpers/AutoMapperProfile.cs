using AutoMapper;
using WebEcommerce.Controllers;
using WebEcommerce.Models;
using WebEcommerce.ViewModels;

namespace WebEcommerce.Helpers
{
	public class AutoMapperProfile: Profile
	{
		public AutoMapperProfile() { 
			CreateMap<RegisterVM, ApplicationUser>();
		}
	}
}
