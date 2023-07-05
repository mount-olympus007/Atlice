using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Atlice.Domain.Abstract;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atlice.WebUI.Pages.Tap
{
    public class tap_try_laterModel : PageModel
    {
        private IDataRepository repository;
        private UserManager<ApplicationUser> userManager;
        private IServices services;
        public tap_try_laterModel(IServices _services, IDataRepository _repository, UserManager<ApplicationUser> _userManager)
        {
            services = _services;
            repository = _repository;
            userManager = _userManager;
        }
        [BindProperty]
        public InputModel? Input { get; set; }
        public class InputModel
        {
            public string? Email { get; set; }
            public string? Phone { get; set; }
        }
        public async Task OnGet(string invitecode, string email)
        {
            if (!string.IsNullOrEmpty(invitecode) && !string.IsNullOrEmpty(email))
            {
                var pageOwner = userManager.Users.FirstOrDefault(x => x.Id.ToString() == invitecode);
                var newUser = userManager.Users.FirstOrDefault(x => x.Email == email);
                if (pageOwner != null && newUser is not null)
                {
                    var roles = await userManager.GetRolesAsync(newUser);
                    if(roles.Count() == 1 && roles.First() == "Lead")
                    {
                        await userManager.RemoveFromRolesAsync(newUser, roles);
                        await userManager.AddToRoleAsync(newUser, "Invited");
                    }
                    await repository.SaveAdminNote(new AdminNote(pageOwner.Id, newUser.Email ??= "", "Invited"));
                   
                    Input = new InputModel
                    {
                        Email = newUser.Email,

                    };
                }
                
            }
           
        }
      
    }
}
