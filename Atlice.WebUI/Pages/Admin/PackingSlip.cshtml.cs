using Atlice.Domain.Abstract;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Syncfusion.Pdf;
using Syncfusion.HtmlConverter;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace Atlice.WebUI.Pages.Admin
{
    [Authorize(Roles = "Adminis")]
    public class PackingSlipModel : PageModel
    {
        private IDataRepository _dataRepository;
        private IServices _services;
        public PackingSlipModel(IDataRepository dataRepository, IServices services)
        {
            _dataRepository = dataRepository;
            _services = services;
        }
        [ViewData]
        public Order? order { get; set; } = new Order();
        public async Task<IActionResult> OnGet(Guid id)
        {
            Order? o = _dataRepository.Orders.FirstOrDefault(o => o.Id == id);
            if (o == null)
            {
                RedirectToPage("Fullfillment");
            }
            order = o;


            

            return Page();
        }

        public async Task<IActionResult> OnGetPrintSlip(Guid id)
        {
            Order order = _dataRepository.Orders.FirstOrDefault(x => x.Id == id);
            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter();
            BlinkConverterSettings blinkConverterSettings = new BlinkConverterSettings();
            //Set Blink viewport size.
            blinkConverterSettings.ViewPortSize = new Syncfusion.Drawing.Size(1280, 0);
            //Assign Blink converter settings to HTML converter.
            htmlConverter.ConverterSettings = blinkConverterSettings;
            //Convert URL to PDF document.
            PdfDocument document = htmlConverter.Convert("https://localhost:7048/admin/packingslip/" + order.Id);
            //Create a filestream.
            FileStream fileStream = new FileStream("HTML-to-PDF.pdf", FileMode.CreateNew, FileAccess.ReadWrite);
            //Save and close the PDF document.
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                string url = await _services.BackUpToCloud(ms.ToArray(), order.OrderNumber + ".pdf");
                TempData["pdfurl"] = url;
                document.Close(); 
            }
            return new JsonResult(new { url = TempData["pdfurl"] });
            


            
        }

        
    }
}
