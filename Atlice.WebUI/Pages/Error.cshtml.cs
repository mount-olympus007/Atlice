using Atlice.Domain.Abstract;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace Atlice.WebUI.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        private readonly IDataRepository repository;
        public ErrorModel(IDataRepository _repository)
        {
            repository = _repository;
        }
        [BindProperty]
        public ErrorFeedback Input { get; set; } = new ErrorFeedback();
        

        public async Task OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            Input.TimeStamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time);
            Input.RequestId = RequestId;
            Input.Id = Guid.NewGuid();
            if (User.Identity is not null && User.Identity.IsAuthenticated)
            {
                Input.FirstName= User.Identity.Name;
                Input.LastName = User.Identity.Name;
            }
            else
            {
                if (HttpContext.Connection.RemoteIpAddress is not null)
                {
                    Input.FirstName = HttpContext.Request.Headers["User-Agent"].ToString();
                    Input.LastName = HttpContext.Connection.RemoteIpAddress.ToString();

                }
            }
            await repository.SaveErrorFeedback(Input);

            
        }
        public async Task<IActionResult> OnPostSubmitErrorAsync()
        {
            if (ModelState.IsValid && Input is not null)
            {
                ErrorFeedback? errorFeedback = repository.ErrorFeedbacks.FirstOrDefault(x => x.Id == Input.Id);
                if(errorFeedback != null)
                {
                    if (User.Identity is not null && User.Identity.IsAuthenticated)
                    {
                        errorFeedback.LastName = User.Identity.Name;
                        await repository.SaveErrorFeedback(errorFeedback);

                    }
                    else
                    {
                        if (HttpContext.Connection.RemoteIpAddress is not null)
                        {
                            errorFeedback.FirstName = HttpContext.Request.Headers["User-Agent"].ToString();
                            errorFeedback.LastName = HttpContext.Connection.RemoteIpAddress.ToString();

                        }
                        await repository.SaveErrorFeedback(errorFeedback);
                        return RedirectToPage("/Identity/Account/Login");
                    }
                }
               
                return RedirectToPage("/Identity/Account/Login");
            }
            return RedirectToPage("/Identity/Account/Login");
        }

    }
}
