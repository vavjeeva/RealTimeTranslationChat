using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RealTimeTranslationChat.Pages
{
    public class IndexModel : PageModel
    {
        public string Name { get; set; }

        public string Language { get; set; }

        public SelectList LanguageSL { get; set; }

        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {
            return RedirectToPage("./Chat");
        }
    }
}
