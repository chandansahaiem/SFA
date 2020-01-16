using Microsoft.EntityFrameworkCore;
using SFA.Entities;
using SFA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.Services
{
    public interface IRoleMenuService
    {
        Task<List<Menu>> GetMenuByRole(Guid roleId);
        Task<bool> SaveRoleMenu(RoleWiseMenu roleMenu); 
    }
    public class RoleMenuService : IRoleMenuService
    {
        private readonly RNDDataContext _context = null;

        public RoleMenuService(RNDDataContext context)
        {
            _context = context;
        }

        public async Task<List<Menu>> GetMenuByRole(Guid roleId)
        {
            var menuEntities = await _context.TblMenu.Include(m => m.TblRoleMenu).OrderBy(m => m.Name).ToListAsync();
            var roleMenuEntities = await _context.TblRoleMenu.Where(n => n.RoleId == roleId).ToListAsync();
            var menuList = new List<Menu>();

            foreach (var item in menuEntities)
            {
                menuList.Add(new Menu
                {
                    Id = item.Id,
                    Name = item.Name,
                    HasReadAccess = roleMenuEntities.Where(n => n.RoleId == roleId && n.MenuId == item.Id && n.HasReadAccess).Count() > 0 ? true : false,
                    HasWriteAccess = roleMenuEntities.Where(n => n.RoleId == roleId && n.MenuId == item.Id && n.HasWriteAccess).Count() > 0 ? true : false,
                    HasFullAccess = roleMenuEntities.Where(n => n.RoleId == roleId && n.MenuId == item.Id && n.HasFullAccess).Count() > 0 ? true : false
                });
            }

            return menuList;
        }

        public async Task<bool> SaveRoleMenu(RoleWiseMenu roleMenu)
        {
            var existRoleMenuEntities = await _context.TblRoleMenu.Where(m => m.RoleId == roleMenu.RoleId).ToListAsync();

            if (existRoleMenuEntities.Count > 0)
            {
                _context.TblRoleMenu.RemoveRange(existRoleMenuEntities);
                _context.SaveChanges();
            }

            var newRoleMenuEntities = new List<TblRoleMenu>();

            foreach (var item in roleMenu.Menus)
            {
                if (item.HasReadAccess || item.HasWriteAccess || item.HasFullAccess)
                {
                    var roleMenuEntity = new TblRoleMenu
                    {
                        RoleId = roleMenu.RoleId,
                        MenuId = item.Id,
                        HasFullAccess = item.HasFullAccess,
                        HasReadAccess = item.HasReadAccess,
                        HasWriteAccess = item.HasWriteAccess
                    };
                    newRoleMenuEntities.Add(roleMenuEntity);
                }
            }

            try
            {
                _context.TblRoleMenu.AddRange(newRoleMenuEntities);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
