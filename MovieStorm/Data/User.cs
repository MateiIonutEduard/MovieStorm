using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MovieStorm.Data
{
    public class User
    {
        public int Id { get; set; }

        [StringLength(80)]
        public string username { get; set; }

        [StringLength(64)]
        public string password { get; set; }

        public string address { get; set; }

        public string logo { get; set; }

        public string token { get; set; }

        [ForeignKey("user_id")]
        public virtual ICollection<Movie> Movies { get; set; }

        [ForeignKey("user_id")]
        public virtual ICollection<Review> Reviews { get; set; }

        public bool admin { get; set; }
    }
}
