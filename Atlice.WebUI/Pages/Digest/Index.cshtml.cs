using Atlice.Domain.Abstract;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using System.Data;
using System.Drawing.Printing;

namespace Atlice.WebUI.Pages.Digest
{
    [Authorize(Roles = "Citizen,Tourist,Adminis")]
    public class IndexModel : PageModel
    {
        public IServices _services;
        public int PageSize = 13;
        public IndexModel(IServices services)
        {
            _services = services;
        }
        [ViewData]
        public int NextPage { get; set; }
        [ViewData]
        public int MaxPages { get; set; }
        [ViewData]
        public int Number { get; set; }
        [ViewData]
        public string? Category { get; set; }
        [ViewData]
        public int? CatId { get; set; }
        [ViewData]
        public string? TagSlug { get; set; }
        [ViewData]
        public string? SearchValue { get; set; }

        public async Task<IActionResult> OnGet(int NextPage = 0, string catName = null, int TagSlugID = 0, string SearchValue = null)
        {
            var json = new News() { PostType = PostType.Photo, Title = "Hello World", Description = "This is a test", Photos = new List<Photo>() { new Photo { FilePath = "https://atlicemedia.blob.core.windows.net/atliceapp/0594510b-d5dc-4baf-a199-e5c4df0b42aelogo.jpg" } }, Created = DateTime.Now };
            if (NextPage > 0)
            {
                List<News> masterPosts = new List<News>
                {
                    json
                };
                int maxNumOfPages = (masterPosts.Count() / PageSize) + 1;
                var strView = _services.RenderToString("LoadMorePosts", masterPosts);
                return new JsonResult(new { listHTML = strView, maxpages = maxNumOfPages });
            }
            return Page();
        }
        public async Task<IActionResult> OnGetBlog()
        {
            var json = new News() { PostType = PostType.Photo, Title = "Hello World", Description = "This is a test", Photos = new List<Photo>() { new Photo { FilePath = "https://atlicemedia.blob.core.windows.net/atliceapp/0594510b-d5dc-4baf-a199-e5c4df0b42aelogo.jpg" } }, Created = DateTime.Now };
            if (NextPage > 0)
            {
                List<News> masterPosts = new List<News>
                {
                    json
                };
                int maxNumOfPages = (masterPosts.Count() / PageSize) + 1;
                var strView = _services.RenderToString("LoadMorePosts", masterPosts);
                return new JsonResult(new { listHTML = strView, maxpages = maxNumOfPages });
            }
            return new JsonResult(new { });
        }
    }
}
