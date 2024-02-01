using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public partial class User
    {
        public byte[] Username { get; set; } = null!;
        public long Id { get; set; }
        public byte[] Passhash { get; set; } = null!;
    }
}
