using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RealTimeTranslationChat.Models;

namespace RealTimeTranslationChat.Pages
{    
    public class IndexModel : PageModel
    {
        [BindProperty]
        public User user { get; set; }

        public SelectList LanguageSL { get; set; }        
    }
}
