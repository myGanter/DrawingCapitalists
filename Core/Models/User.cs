using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models
{
    [Owned]
    public class User
    {
        public string FingerPrint { get; set; }

        public string Name { get; set; }

        public string Ip { get; set; }
    }
}
