using System;
using System.Collections.Generic;

namespace CRUDServices.DataAccess.Models
{
    public partial class Users
    {
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
