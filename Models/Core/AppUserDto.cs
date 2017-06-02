using System;

namespace ESPL.NG.Models.Core
{
    public class AppUserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
    }
}