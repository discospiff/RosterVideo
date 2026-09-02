using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RosterVideo.Models;
using RosterVideo.Services;

namespace RosterVideo.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IRosterStore _store;

        public IndexModel(IRosterStore store)
        {
            _store = store;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public IList<RosterEntry> Entries { get; private set; } = new List<RosterEntry>();

        public void OnGet()
        {
            LoadEntries();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                LoadEntries();
                return Page();
            }

            var entry = new RosterEntry
            {
                FirstName = Input.FirstName!,
                LastName = Input.LastName!,
                Major = Input.Major,
                Shortcut = Input.Shortcut!,
                WhereUsed = Input.WhereUsed
            };

            _store.Add(entry);

            // PRG: redirect to GET to avoid repost
            return RedirectToPage();
        }

        private void LoadEntries()
        {
            Entries = _store.GetAll()
                .OrderByDescending(e => e.CreatedAt)
                .ToList();
        }

        public class InputModel
        {
            [Required]
            [StringLength(50)]
            public string? FirstName { get; set; }

            [Required]
            [StringLength(50)]
            public string? LastName { get; set; }

            [StringLength(100)]
            public string? Major { get; set; }

            [Required]
            [StringLength(100)]
            public string? Shortcut { get; set; }

            [StringLength(200)]
            public string? WhereUsed { get; set; }
        }
    }
}
