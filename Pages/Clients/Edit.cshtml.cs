using System;
using System.Threading.Tasks;
using EntityEditor.Data;
using EntityEditor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EntityEditor.Pages.Clients
{
    public class EditModel : PageModel
    {
        private readonly EntityEditorContext _context;
        public EditModel(EntityEditorContext context) => _context = context;

        [BindProperty]
        public ClientInput Client { get; set; } = new ClientInput();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();
            var client = await _context.Clients.FindAsync(id.Value);
            if (client == null)
                return NotFound();
            Client = new ClientInput
            {
                Name = client.Name,
                IndividualTaxNumber = client.IndividualTaxNumber,
                OrganizationType = client.OrganizationType
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
                return NotFound();
            var client = await _context.Clients.FindAsync(id.Value);
            if (client == null)
                return NotFound();
            if (!ModelState.IsValid)
                return Page();

            client.Name = Client.Name.Trim();
            client.IndividualTaxNumber = Client.IndividualTaxNumber;
            client.OrganizationType = Client.OrganizationType;
            client.UpdateDate = DateTime.UtcNow;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Clients.AnyAsync(c => c.ID == id.Value))
                    return NotFound();
                throw;
            }
            return RedirectToPage("./Index");
        }
    }
}