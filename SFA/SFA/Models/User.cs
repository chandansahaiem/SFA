using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public string Password { get; set; }
        public bool IsSuperAdmin { get; set; }
        public bool IsActive { get; set; }
        public string DataAccessCode { get; set; }
        //public List<MenuAccess> Menus { get; set; }
        public List<GroupPermission> Groups { get; set; }
        public List<MenuPermission> Permissions { get; set; }

        public string ActiveStatus { get; set; }
        public string UserStatus { get; set; } 
    }

    public class UserQuery : Query
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public Guid UserId { get; set; }
    }

    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class GroupPermission
    {
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public int GroupPosition { get; set; }
        public int? Sequence { get; set; }
        public string GroupNameBeng { get; set; }
        public string DefaultCategory { get; set; }
        public string Category { get; set; }
        public string CategoryBeng { get; set; }
        public string Target { get; set; }
        public string Icon { get; set; }
    }

    public class MenuPermission
    {
        public Guid MenuId { get; set; }
        public string MenuName { get; set; }
        public string NameBeng { get; set; }
        public string MenuIcon { get; set; }
        public string MenuTarget { get; set; }
        public int MenuPosition { get; set; }
        public Guid MenuGroupId { get; set; }
        public bool HasReadAccess { get; set; }
        public bool HasWriteAccess { get; set; }
        public bool HasFullAccess { get; set; }
    }
}
