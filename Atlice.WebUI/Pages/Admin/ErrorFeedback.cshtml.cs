using Atlice.Domain.Abstract;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace Atlice.WebUI.Pages.Admin
{
    [Authorize(Roles = "Adminis")]
    public class ErrorFeedbackModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDataRepository _dataRepository;
        public ErrorFeedbackModel(UserManager<ApplicationUser> userManager, IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
            _userManager = userManager;
        }
        [ViewData]
        public List<ErrorFeedback> ErrorFeedbacks { get; set; } = new List<ErrorFeedback>();
        public void OnGet()
        {
            ErrorFeedbacks = _dataRepository.ErrorFeedbacks.ToList();
        }
    }
}
