using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Atlice.WebUI.Models
{
    public class vCardActionResult : IActionResult
    {
        private readonly VCard _vCard;

        public vCardActionResult(VCard vCard)
        {
            _vCard = vCard;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var fileName = _vCard.GetFullName().Replace(".", "_").Replace(" ", "_") + ".vcf";
            var disposition = "attachment; filename=" + fileName;
            var response = context.HttpContext.Response;
            response.ContentType = "text/vcard";
            response.Headers.Add("Content-disposition", disposition);
            var bytes = Encoding.UTF8.GetBytes(_vCard.ToString());
            await response.Body.WriteAsync(bytes, 0, bytes.Length);
        }
        public class BaseController : Controller
        {
            public IActionResult VCard(VCard vCard)
            {
                return new vCardActionResult(vCard);
            }
        }

    }
}
