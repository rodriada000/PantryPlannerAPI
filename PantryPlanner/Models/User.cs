using System;
using System.Collections.Generic;

namespace PantryPlanner.Models
{
    public partial class User
    {
        public long UserId { get; set; }
        public Guid UniquePublicGuid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}
