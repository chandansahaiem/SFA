using Microsoft.EntityFrameworkCore;
using SFA.Entities;
using SFA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.Services
{
    public interface IUserService
    {
        Task<List<User>> GetAll();
        Task<User> GetById(Guid id);
        Task<List<User>> GetByRoleId(Guid roleId);
        Task<QueryResult<User>> Search(UserQuery query);
        Task<bool> Save(User user);
        Task<User> Validate(string email, string password);
        Task<List<RoleMenu>> GetMenuByUser(Guid userId);
        //Task<List<ApplicationAccess>> GetAccessList(Guid id);
        //Task<List<UserApplication>> GetMenuByUser(Guid userId);
        //Task<bool> SaveLogIn(Guid id);
        //Task<bool> SaveLogOut(Guid id);
    }

    public class UserService : IUserService
    {
        private readonly RNDDataContext _context = null;
        private readonly IMenuService _menuService = null;
        private readonly IMenuGroupService _menuGroupService = null;

        public UserService(RNDDataContext context, IMenuService menuService, IMenuGroupService menuGroupService)
        {
            _context = context;
            _menuService = menuService;
            _menuGroupService = menuGroupService;
        }

        public async Task<List<User>> GetAll()
        {
            var userEntities = await _context.TblUser.Include(m=>m.Role).OrderBy(m => m.Email).ToListAsync();
            return userEntities.Select(m => new User
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                RoleName = m.Role.Name,
                RoleId = m.RoleId,
                IsSuperAdmin = m.IsSuperAdmin,
                IsActive = m.IsActive
            }).ToList();
        }

        public async Task<List<User>> GetByRoleId(Guid roleId)
        {
            var userEntities = await _context.TblUser.Include(m=>m.Role).Where(m => m.RoleId == roleId).ToListAsync();
            return userEntities.Select(m => new User
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                RoleName = m.Role.Name,
                RoleId = m.RoleId,
                IsSuperAdmin = m.IsSuperAdmin,
                IsActive = m.IsActive
            }).ToList();
        }

        public async Task<User> GetById(Guid id)
        {
            var userEntity = await _context.TblUser.Include(m=>m.Role).FirstOrDefaultAsync(m => m.Id == id);
            return new User
            {
                Id = userEntity.Id,
                Name = userEntity.Name,
                Email = userEntity.Email,
                Password = userEntity.Password,
                RoleName = userEntity.Role.Name,
                RoleId = userEntity.RoleId,
                IsSuperAdmin = userEntity.IsSuperAdmin,
                IsActive = userEntity.IsActive
            };
        }

        public async Task<QueryResult<User>> Search(UserQuery query)
        {
            try
            {
                var skip = (query.Page - 1) * query.Limit;
                var userQuery = _context.TblUser.Include(m => m.Role).AsNoTracking().AsQueryable();

                if (!string.IsNullOrEmpty(query.Name))
                {
                    userQuery = userQuery.Where(m => m.Name.Contains(query.Name));
                }
                var count = await userQuery.CountAsync();

                switch (query.Order.ToLower())
                {
                    default:
                        userQuery = query.Order.StartsWith("-") ? userQuery.OrderByDescending(m => m.Name) : userQuery.OrderBy(m => m.Name);
                        break;
                }
                var userEntities = await userQuery.Skip(skip).Take(query.Limit).ToListAsync();
                var users = userEntities.Select(m => new User
                {
                    Id = m.Id,
                    Name = m.Name,
                    Email = m.Email,
                    RoleName = m.Role.Name,
                    RoleId = m.RoleId,
                    IsSuperAdmin = m.IsSuperAdmin,
                    IsActive = m.IsActive,
                    ActiveStatus = m.IsActive ? "Active" : "In-Active",
                    UserStatus = m.IsSuperAdmin ? "Yes" : "No"
                }).ToList();

                return new QueryResult<User> { Result = users, Count = count };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> Save(User user)
        {
            try
            {
                var userEntity = new TblUser();
                if (user.Id == Guid.Empty)
                {
                    userEntity.Id = Guid.NewGuid();
                    userEntity.Name = user.Name;
                    userEntity.Password = user.Password;
                    userEntity.Email = user.Email;
                    userEntity.IsSuperAdmin = user.IsSuperAdmin;
                    userEntity.IsActive = user.IsActive;
                    userEntity.RoleId = user.RoleId;

                    _context.TblUser.Add(userEntity);
                }
                else
                {
                    userEntity = await _context.TblUser.FirstOrDefaultAsync(m => m.Id == user.Id);
                    userEntity.Name = user.Name;
                    userEntity.Email = user.Email;
                    userEntity.IsSuperAdmin = user.IsSuperAdmin;
                    userEntity.IsActive = user.IsActive;
                    userEntity.Password = user.Password;
                    userEntity.RoleId = user.RoleId;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<User> Validate(string email, string password)
        {
            var userEntity = await _context
                .TblUser.Include(m=>m.Role)
                .FirstOrDefaultAsync(m => m.Email.Equals(email) && m.Password.Equals(password) && m.IsActive);
            if(userEntity == null)
            {
                return null;
            }
            var menuGroups = await _menuGroupService.GetAll();

            var userRole = await _menuGroupService.GetRoleMenuAccess(userEntity.Id);

            if (userEntity == null)
            {
                return null;
            }

            return new User
            {
                Name = userEntity.Name,
                Id = userEntity.Id,
                Email = userEntity.Email,
                IsSuperAdmin = userEntity.IsSuperAdmin,
                RoleId = userEntity.RoleId,
                RoleName = userEntity.Role.Name,
                DataAccessCode = userEntity.Role.DataAccessCode,
                Groups = menuGroups.Select(m=> new GroupPermission
                {
                    GroupId = m.Id,
                    GroupName = m.Name,
                    Category = m.Category,
                    GroupPosition = m.DisplayPosition,
                    Icon = m.Icon,
                    Sequence = m.Sequence,
                    Target = m.Target,
                    DefaultCategory = m.Category
                }).ToList(),
                Permissions = userRole.AccessMenus.Select(m => new MenuPermission
                {
                    MenuId = m.MenuId,
                    MenuName = m.MenuName,
                    NameBeng = m.NameBeng,
                    MenuIcon = m.MenuIcon,
                    MenuTarget = m.MenuTarget,
                    MenuPosition = m.MenuPosition,
                    MenuGroupId = m.MenuGroupId,
                    HasReadAccess = m.HasReadAccess,
                    HasWriteAccess = m.HasWriteAccess,
                    HasFullAccess = m.HasFullAccess
                }).ToList()
            };
        }

        public async Task<List<RoleMenu>> GetMenuByUser(Guid userId)
        {
            var user = await _context.TblUser.FirstOrDefaultAsync(m => m.Id == userId);
            var apps = (user.IsSuperAdmin) ? await _menuService.GetAll() : await _menuService.GetByRole(user.RoleId);
            var appGroups = apps.OrderBy(m => m.MenuGroupName).Select(m => m.MenuGroupName).Distinct();
            var allApps = new List<RoleMenu>();
            foreach (var appGroup in appGroups)
            {
                var groupApps = apps.Where(m => m.MenuGroupName.Equals(appGroup)).OrderBy(m => m.Position);
                allApps.Add(new RoleMenu
                {
                    AppName = appGroup,
                    Menus = groupApps.Select(m => new RoleMenu
                    {
                        AppName = m.Name,
                        Path = m.StartingPath
                    }).ToList()
                });
            }

            return allApps;
        }
    }
}
