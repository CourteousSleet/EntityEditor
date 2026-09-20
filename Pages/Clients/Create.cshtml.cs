using System;
using System.Threading.Tasks;
using EntityEditor.Data;
using EntityEditor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EntityEditor.Pages.Clients
{
    public class CreateModel : PageModel
    {
        private readonly EntityEditorContext _context;
        public CreateModel(EntityEditorContext context) => _context = context;

        [BindProperty]
        public ClientInput Client { get; set; } = new ClientInput();

        public IActionResult OnGet() => Page();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var now = DateTime.UtcNow;
            _context.Clients.Add(new Client
            {
                Name = Client.Name.Trim(),
                IndividualTaxNumber = Client.IndividualTaxNumber,
                OrganizationType = Client.OrganizationType,
                CreationDate = now,
                UpdateDate = now
            });
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}