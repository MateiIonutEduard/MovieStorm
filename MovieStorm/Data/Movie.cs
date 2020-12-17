using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MovieStorm.Data
{
    public class Movie
    {
        public int Id { get; set; }

        [StringLength(1024)]
        public string name { get; set; }

        [StringLength(20)]
        public string genre { get; set; }

        public DateTime released { get; set; }

        public string description { get; set; }

        public string preview { get; set; }

        public int views { get; set; }

        public int user_id { get; set; }

        public virtual User User { get; set; }

        [ForeignKey("movie_id")]
        public ICollection<Review> Reviews { get; set; }

        public string path { get; set; }
    }
}
