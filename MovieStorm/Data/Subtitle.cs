using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieStorm.Data
{
    public class Subtitle
    {
        public int Id { get; set; }

        [StringLength(20)]
        public string code { get; set; }

        public string path { get; set; }

        public int movie_id { get; set; }

        public virtual Movie Movie { get; set; }
    }
}
