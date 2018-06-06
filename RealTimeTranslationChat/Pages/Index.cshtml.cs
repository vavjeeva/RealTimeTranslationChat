using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RealTimeTranslationChat.Helper;
using RealTimeTranslationChat.Models;

namespace RealTimeTranslationChat.Pages
{
    public class IndexModel : PageModel
    {
        private CognitiveServiceClient _client;
        public IndexModel(CognitiveServiceClient Client)
        {
            _client = Client;
        }

        [BindProperty]
        public User user { get; set; }

        public SelectList LanguageSL { get; set; }

        public async Task OnGetAsync()
        {
            await GetLanguages();
        }

        private async Task GetLanguages()
        {
            LanguageSL = new SelectList(await _client.GetSupportedLanguages(),nameof(Language.Code),nameof(Language.Name));
        }
    }
}
