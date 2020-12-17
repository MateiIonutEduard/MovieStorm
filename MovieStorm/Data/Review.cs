using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieStorm.Data
{
    public class Review
    {
        public int Id { get; set; }

        public int rating { get; set; }

        public string content { get; set; }

        public int user_id { get; set; }

        public virtual User User { get; set; }

        public int movie_id { get; set; }

        public virtual Movie Movie { get; set; }
    }
}
