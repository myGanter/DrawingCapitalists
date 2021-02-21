using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.DB
{
    public class WordCollection
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public UserState UserState { get; set; }

        public List<Word> Words { get; set; }

        public WordCollection()
        {
            Words = new List<Word>();
        }
    }
}
