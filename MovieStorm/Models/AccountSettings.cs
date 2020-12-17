using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieStorm.Models
{
    public class AccountSettings : IAccountSettings
    {
        public string host { get; set; }

        public string port { get; set; }

        public string client { get; set; }

        public string secret { get; set; }
    }

    public interface IAccountSettings
    {
        string host { get; set; }

        string port { get; set; }

        string client { get; set; }

        string secret { get; set; }
    }
}
