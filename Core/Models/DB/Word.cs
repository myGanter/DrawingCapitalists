using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.DB
{
    public class Word
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public int WordCollectionId { get; set; }
        public WordCollection WordCollection { get; set; }
    }
}
