using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.Models
{
    public class MenuGroup
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int DisplayPosition { get; set; }
        public int? Sequence { get; set; }
        public string Category { get; set; }
        public string Target { get; set; }
        public string Icon { get; set; }
        public bool IsActive { get; set; }
    }
}
