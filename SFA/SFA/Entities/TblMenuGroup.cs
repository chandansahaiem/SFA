using System;
using System.Collections.Generic;

namespace SFA.Entities
{
    public partial class TblMenuGroup
    {
        public TblMenuGroup()
        {
            TblMenu = new HashSet<TblMenu>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public int DisplayPosition { get; set; }
        public int? Sequence { get; set; }
        public string Category { get; set; }
        public string Target { get; set; }
        public string Icon { get; set; }
        public bool IsActive { get; set; }

        public ICollection<TblMenu> TblMenu { get; set; }
    }
}
