
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barangay.Pages.Admin
{
    public class ListPermissionsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ListPermissionsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Permission> Permissions { get;set; }

        public async Task OnGetAsync()
        {
            Permissions = await _context.Permissions.ToListAsync();
        }
    }
}
