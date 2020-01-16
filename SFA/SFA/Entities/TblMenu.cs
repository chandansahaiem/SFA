using System;
using System.Collections.Generic;

namespace SFA.Entities
{
    public partial class TblMenu
    {
        public TblMenu()
        {
            TblRoleMenu = new HashSet<TblRoleMenu>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Target { get; set; }
        public Guid MenuGroupId { get; set; }
        public string StartingPath { get; set; }
        public int Position { get; set; }
        public bool IsActive { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public TblUser CreatedByNavigation { get; set; }
        public TblUser LastModifiedByNavigation { get; set; }
        public TblMenuGroup MenuGroup { get; set; }
        public ICollection<TblRoleMenu> TblRoleMenu { get; set; }
    }
}
