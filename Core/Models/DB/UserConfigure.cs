using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.DB
{
    public class UserConfigure
    {
        public int Id { get; set; }

        public string Base64Ava { get; set; }

        public UserState UserState { get; set; }
    }
}
