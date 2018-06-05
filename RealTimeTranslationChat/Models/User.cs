using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealTimeTranslationChat.Models
{
    public class User
    {
        public string Name { get; set; }
        public string LanguagePreference { get; set; }
        public string ConnectionId { get; set; }
    }
}
