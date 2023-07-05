using Atlice.Domain.Abstract;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atlice.WebUI.Pages.Admin
{
    [Authorize(Roles = "Adminis")]
    public class AdminNotesModel : PageModel
    {

        private readonly IDataRepository _repository;
        public AdminNotesModel(IDataRepository repository)
        {
            _repository = repository;
        }

        [ViewData]
        public List<NoteModel> NoteModels { get; set; } = new List<NoteModel>();
        public class NoteModel
        {
            public ApplicationUser User { get; set; } = new ApplicationUser();
            public AdminNote? AdminNote { get; set; }
        }
        public IActionResult OnGet()
        {
            foreach(var note in _repository.AdminNotes.OrderByDescending(x=>x.When).Take(100).ToList())
            {
                var user = _repository.Users.FirstOrDefault(x => x.Id == note.UserId);
                if (user != null)
                {
                    NoteModel n = new()
                    {
                        AdminNote = note,
                        User = user
                    };
                    NoteModels.Add(n);
                }
                
            }
            return Page();
        }

        public IActionResult OnGetProfile(string username)
        {
            var user = _repository.Users.FirstOrDefault(x=>x.UserName == username);
            return RedirectToPage("/admin/userprofile/", new { id = user.Id });
        }

        public IActionResult OnPostFindNote(string? lastname, string? action, string? date)
        {
            if (!string.IsNullOrEmpty(lastname))
            {

                foreach(var user in _repository.Users.Where(x=>x.LastName is not null))
                {
                    var last = user.LastName;
                    if(last is not null)
                    {
                        if (last.ToLower().Contains(lastname.ToLower()))
                        {
                            var notes = _repository.AdminNotes.Where(x => x.UserId == user.Id);
                            if (!string.IsNullOrEmpty(action))
                            {
                                notes = notes.Where(x => x.What.ToLower().Contains(action.ToLower())).ToList();
                            }
                            if (!string.IsNullOrEmpty(date))
                            {
                                notes = notes.Where(x => x.When == DateTime.Parse(date)).ToList();
                            }
                            foreach (var note in notes)
                            {
                                NoteModel nm = new()
                                {
                                    User = user,
                                    AdminNote = note
                                };
                                NoteModels.Add(nm);
                            }
                        }
                    }
                    
                }

            }
            else
            {
                List<AdminNote> notes = new List<AdminNote>();
                if (!string.IsNullOrEmpty(action))
                {
                    foreach(var note in _repository.AdminNotes.Where(x => x.What.ToLower().Contains(action.ToLower())))
                    {
                        notes.Add(note);
                        
                    }
                }
                if (!string.IsNullOrEmpty(date))
                {
                    foreach (var note in _repository.AdminNotes.Where(x => x.When == DateTime.Parse(date)))
                    {
                        notes.Add(note);
                    }

                }
                foreach (var note in notes)
                {
                    var user = _repository.Users.FirstOrDefault(x => x.Id == note.UserId);
                    if (user is not null)
                    {
                        NoteModels.Add(new NoteModel { AdminNote = note, User = user });
                    }
                }
            }
            return Page();
            
        }
    }
}
