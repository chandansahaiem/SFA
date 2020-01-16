using System;
using System.Collections.Generic;

namespace SFA.Entities
{
    public partial class TblRoleMenu
    {
        public Guid RoleId { get; set; }
        public Guid MenuId { get; set; }
        public bool HasReadAccess { get; set; }
        public bool HasWriteAccess { get; set; }
        public bool HasFullAccess { get; set; }

        public TblMenu Menu { get; set; }
        public TblRole Role { get; set; }
    }
}
