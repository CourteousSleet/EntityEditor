using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EntityEditor.Data;
using EntityEditor.Models;

namespace EntityEditor.Pages.Clients
{
    public class IndexModel : PageModel
    {
        private readonly EntityEditor.Data.EntityEditorContext _context;

        public IndexModel(EntityEditor.Data.EntityEditorContext context)
        {
            _context = context;
        }

        public IList<Client> Client { get;set; }

        public async Task OnGetAsync()
        {
            Client = await _context.Clients.ToListAsync();
        }
    }
}
