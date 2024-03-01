using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public partial class User
    {
        public string Username { get; set; } = null!;
        public long Id { get; set; }
        public string Passhash { get; set; } = null!;
    }
}
