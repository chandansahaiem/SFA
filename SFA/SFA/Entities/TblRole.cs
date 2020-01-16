using System;
using System.Collections.Generic;

namespace SFA.Entities
{
    public partial class TblRole
    {
        public TblRole()
        {
            TblRoleMenu = new HashSet<TblRoleMenu>();
            TblUser = new HashSet<TblUser>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DataAccessCode { get; set; }
        public bool IsActive { get; set; }

        public ICollection<TblRoleMenu> TblRoleMenu { get; set; }
        public ICollection<TblUser> TblUser { get; set; }
    }
}
