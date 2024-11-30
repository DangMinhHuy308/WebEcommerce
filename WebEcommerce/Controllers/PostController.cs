using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebEcommerce.Data;
using WebEcommerce.ViewModels;
using X.PagedList.Extensions;

namespace WebEcommerce.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notification;

        public PostController(ApplicationDbContext context, INotyfService notyfService)
        {
            _context = context;
            _notification = notyfService;
        }
        public IActionResult Index(int? page)
        {
            int pageSize = 6;
            int pageNum = page ?? 1;
            var post = _context.Posts.AsQueryable();
           
            var result = post.Select(x => new PostVM
            {
                Id = x.Id,
                Title = x.Title,
                ShortDescription = x.ShortDescription,
                Description = x.Description,
                ThumbnailUrl = x.ThumbnailUrl
            });
            // Thực hiện phân trang
            var pagedPostVM = result.ToPagedList(pageNum, pageSize);
            return View(pagedPostVM);
        }
        [HttpGet]
        public IActionResult Detail(int id)
        {
            var post = _context.Posts
                .Where(x => x.Id == id)
                .FirstOrDefault();  // Lấy bài viết theo id

            if (post == null)
            {
                _notification.Error("Post not found");
                return RedirectToAction("Index");
            }

            // Chuyển dữ liệu bài viết thành ViewModel
            var postDetailVM = new PostVM
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description,
                ThumbnailUrl = post.ThumbnailUrl,
                CreatedDate = post.CreatedDate,
                ShortDescription = post.ShortDescription
            };

            return View(postDetailVM);
        }

    }
}
