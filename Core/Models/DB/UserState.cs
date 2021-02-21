using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.DB
{
    public class UserState
    {
        public string FingerPrint { get; set; }

        public string Name { get; set; }

        public string Ip { get; set; }

        public DateTime LoginTime { get; set; }

        public DateTime? LogoutTime { get; set; }

        public bool IsActive { get; set; }

        public int? UserConfigureId { get; set; }

        public UserConfigure UserConfigure { get; set; }

        public List<WordCollection> WordCollections { get; set; }

        public UserState()
        {
            WordCollections = new List<WordCollection>();
        }
    }
}
