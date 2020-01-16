using Microsoft.EntityFrameworkCore;
using SFA.Entities;
using SFA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.Services
{
    public interface IMenuGroupService
    {
        Task<List<MenuGroup>> GetAll();
        Task<List<MenuGroup>> GetAllActive();
        Task<MenuGroup> GetById(Guid id);
        Task<bool> Save(MenuGroup menuGroup);
        Task<Role> GetRoleMenuAccess(Guid userId);
        Task<List<String>> GetAllCategory();
        Task<List<MenuGroup>> GetByCategory(string category);
    }
    public class MenuGroupService : IMenuGroupService
    {
        private readonly RNDDataContext _context = null;

        public MenuGroupService(RNDDataContext context)
        {
            _context = context;
        }

        public async Task<List<MenuGroup>> GetAll()
        {
            var menuGroupEntities = await _context.TblMenuGroup.ToListAsync();
            return menuGroupEntities.Select(m => new MenuGroup
            {
                Id = m.Id,
                Name = m.Name,
                Category = m.Category,
                DisplayPosition = m.DisplayPosition,
                Icon = m.Icon,
                Sequence = m.Sequence,
                Target = m.Target,
                IsActive = m.IsActive
            }).ToList();
        }

        public async Task<List<MenuGroup>> GetAllActive()
        {
            var menuGroupEntities = await _context.TblMenuGroup.Where(m=>m.IsActive).ToListAsync();
            return menuGroupEntities.Select(m => new MenuGroup
            {
                Id = m.Id,
                Name = m.Name,
                IsActive = m.IsActive
            }).ToList();
        }

        public async Task<List<String>> GetAllCategory()
        {
            var menuGroupEntities = await _context.TblMenuGroup.Select(m => m.Category).Distinct().ToListAsync();
            return menuGroupEntities.ToList();
        }

        public async Task<List<MenuGroup>> GetByCategory(string category)
        {
            var menuGroupEntities = await _context.TblMenuGroup.Where(m => m.Category.Trim() == category.Trim()).OrderBy(m => m.DisplayPosition).ToListAsync();
            return menuGroupEntities.Select(m => new MenuGroup
            {
                Id = m.Id,
                Name = m.Name,
                DisplayPosition = m.DisplayPosition,
                Sequence = m.Sequence,
                Category = m.Category,
                Target = m.Target,
                Icon = m.Icon
            }).ToList();
        }


        public async Task<MenuGroup> GetById(Guid id)
        {
            var menuGroupEntity = await _context.TblMenuGroup.FirstOrDefaultAsync(m => m.Id == id);
            return new MenuGroup
            {
                Id = menuGroupEntity.Id,
                Name = menuGroupEntity.Name,
                IsActive = menuGroupEntity.IsActive
            };
        }

        public async Task<Role> GetRoleMenuAccess(Guid userId)
        {
            var userRoleEntity = await _context.TblUser.Include(m => m.Role).FirstOrDefaultAsync(m => m.Id == userId);
            if (userRoleEntity == null)
            {
                return null;
            }
            var roleMenuEntities = await _context.TblRoleMenu.Include(m => m.Menu).ThenInclude(m => m.MenuGroup).Where(m => m.RoleId == userRoleEntity.RoleId).ToListAsync();
            if (userRoleEntity == null)
            {
                return null;
            }
            return new Role
            {
                Id = userRoleEntity.RoleId,
                Name = userRoleEntity.Role.Name,
                DataAccessCode = userRoleEntity.Role.DataAccessCode,
                AccessMenus = roleMenuEntities.Select(m => new AccessMenu
                {
                    MenuId = m.MenuId,
                    MenuName = m.Menu.Name,
                    MenuIcon = m.Menu.Icon,
                    MenuTarget = m.Menu.Target,
                    MenuPosition = m.Menu.Position,
                    MenuGroupId = m.Menu.MenuGroupId,
                    HasReadAccess = m.HasReadAccess,
                    HasWriteAccess = m.HasWriteAccess,
                    HasFullAccess = m.HasFullAccess
                }).ToList()
            };
        }

        public async Task<bool> Save(MenuGroup menuGroup)
        {
            try
            {
                var menuGroupEntity = new TblMenuGroup();
                if (menuGroup.Id == Guid.Empty)
                {
                    menuGroupEntity.Id = Guid.NewGuid();
                    menuGroupEntity.Name = menuGroup.Name;
                    menuGroupEntity.IsActive = menuGroup.IsActive;
                    _context.TblMenuGroup.Add(menuGroupEntity);
                }
                else
                {
                    menuGroupEntity = await _context.TblMenuGroup.FirstOrDefaultAsync(m => m.Id == menuGroup.Id);
                    menuGroupEntity.Name = menuGroup.Name;
                    menuGroupEntity.IsActive = menuGroup.IsActive;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
