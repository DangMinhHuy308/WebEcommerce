using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using WebEcommerce.Models;

namespace WebEcommerce.ViewModels
{
    public class CreateProductVM
    {
        public int Id { get; set; }
        public string? ProductName { get; set; }
        public string? Alias { get; set; }
        public int CategoryId { get; set; }
        public int SupplierId { get; set; }
        public string? Image { get; set; }
        public decimal? Price { get; set; }
        public decimal? Sale { get; set; }
        public decimal? OriginalPrice { get; set; }
        public string? UnitDescription { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsSale { get; set; }
        public IFormFile? Thumbnail { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>(); 
        public List<SelectListItem> Suppliers { get; set; } = new List<SelectListItem>(); 

    }
}
