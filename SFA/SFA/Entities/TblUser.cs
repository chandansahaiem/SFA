using System;
using System.Collections.Generic;

namespace SFA.Entities
{
    public partial class TblUser
    {
        public TblUser()
        {
            TblMenuCreatedByNavigation = new HashSet<TblMenu>();
            TblMenuLastModifiedByNavigation = new HashSet<TblMenu>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Guid RoleId { get; set; }
        public bool IsSuperAdmin { get; set; }
        public bool IsActive { get; set; }

        public TblRole Role { get; set; }
        public ICollection<TblMenu> TblMenuCreatedByNavigation { get; set; }
        public ICollection<TblMenu> TblMenuLastModifiedByNavigation { get; set; }
    }
}
