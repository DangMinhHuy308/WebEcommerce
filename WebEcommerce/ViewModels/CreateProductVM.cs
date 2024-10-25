using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebEcommerce.ViewModels
{
    public class CreateProductVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public decimal? Sale { get; set; }
        public decimal? OriginalPrice { get; set; }

        public DateTime CreatedDate { get; set; }
        public int CategoryId { get; set; }

        public string? Image { get; set; }
        public IFormFile? Thumbnail { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }

    }
}
