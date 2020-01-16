using Microsoft.EntityFrameworkCore;
using SFA.Entities;
using SFA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.Services
{
    public interface IMenuService
    {
        Task<List<Menu>> GetAll();
        Task<Menu> GetById(Guid id);
        Task<bool> Save(Menu menu);
        Task<QueryResult<Menu>> Search(MenuQuery query);
        Task<List<Menu>> GetByRole(Guid id);
        //Task<List<Menu>> GetByGroupId(Guid id);
    }
    public class MenuService : IMenuService
    {
        private readonly RNDDataContext _context = null;

        public MenuService(RNDDataContext context)
        {
            _context = context;
        }

        public async Task<List<Menu>> GetAll()
        {
            var menuEntities = await _context.TblMenu.Include(m=>m.MenuGroup).ToListAsync();
            return menuEntities.Select(m => new Menu
            {
                Id = m.Id,
                Name = m.Name,
                MenuGroupId = m.MenuGroupId,
                MenuGroupName = m.MenuGroup.Name,
                Position = m.Position,
                StartingPath = m.StartingPath,
                IsActive = m.IsActive
            }).ToList();
        }

        public async Task<Menu> GetById(Guid id)
        {
            var menuEntity = await _context.TblMenu.Include(m=>m.MenuGroup).FirstOrDefaultAsync(m => m.Id == id);
            return new Menu
            {
                Id = menuEntity.Id,
                Name = menuEntity.Name,
                MenuGroupId = menuEntity.MenuGroupId,
                MenuGroupName = menuEntity.MenuGroup.Name,
                Position = menuEntity.Position,
                Icon = menuEntity.Icon,
                StartingPath = menuEntity.StartingPath,
                IsActive = menuEntity.IsActive,
                Category = menuEntity.MenuGroup.Category
            };
        }

        public async Task<List<Menu>> GetByRole(Guid id)
        {
            var menuEntities = await _context.TblMenu.Include(m=>m.MenuGroup).Include(m => m.TblRoleMenu).Where(m => m.TblRoleMenu
                                     .Select(n => n.RoleId).Contains(id)).OrderBy(m => m.MenuGroup.Name).ThenBy(m => m.Position).ToListAsync();
            return menuEntities.Select(m => new Menu
            {
                Id = m.Id,
                Name = m.Name,
                MenuGroupName = m.MenuGroup.Name,
                StartingPath = m.StartingPath,
                Position = m.Position,
                Icon = m.Icon,
                MenuGroupId = m.MenuGroupId,
                Target = m.Target,
            }).ToList();
        }

        public async Task<bool> Save(Menu menu)
        {
            try
            {
                var menuEntity = new TblMenu();
                if (menu.Id == Guid.Empty)
                {
                    menuEntity.Id = Guid.NewGuid();
                    menuEntity.Name = menu.Name;
                    menuEntity.MenuGroupId = menu.MenuGroupId;
                    menuEntity.Position = menu.Position;
                    menuEntity.Icon = menu.Icon;
                    menuEntity.StartingPath = menu.StartingPath;
                    menuEntity.Target = menu.StartingPath;
                    menuEntity.CreatedBy = menu.CreatedBy;
                    menuEntity.CreatedOn = DateTime.Now;
                    menuEntity.IsActive = menu.IsActive;
                    _context.TblMenu.Add(menuEntity);
                }
                else
                {
                    menuEntity = await _context.TblMenu.FirstOrDefaultAsync(m => m.Id == menu.Id);
                    menuEntity.Name = menu.Name;
                    menuEntity.MenuGroupId = menu.MenuGroupId;
                    menuEntity.Position = menu.Position;
                    menuEntity.Icon = menu.Icon;
                    menuEntity.StartingPath = menu.StartingPath;
                    menuEntity.Target = menu.StartingPath;
                    menuEntity.LastModifiedBy = menu.LastModifiedBy;
                    menuEntity.LastModifiedOn = DateTime.Now;
                    menuEntity.IsActive = menu.IsActive;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<QueryResult<Menu>> Search(MenuQuery query)
        {
            var skip = (query.Page - 1) * query.Limit;
            var menuQuery = _context.TblMenu.Include(m=>m.MenuGroup).OrderBy(m => m.MenuGroup.Name).ThenBy(m => m.Position).AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(query.Name))
            {
                menuQuery = menuQuery.Where(m => m.Name.Contains(query.Name));
            }
            if (!string.IsNullOrEmpty(query.Group))
            {
                menuQuery = menuQuery.Where(m => m.MenuGroup.Name.Contains(query.Group));
            }
            var count = await menuQuery.CountAsync();

            switch (query.Order.ToLower())
            {
                case "group":
                    menuQuery = query.Order.StartsWith("-") ? menuQuery.OrderByDescending(m => m.MenuGroup.Name) : menuQuery.OrderBy(m => m.MenuGroup.Name);
                    break;
                default:
                    menuQuery = query.Order.StartsWith("-") ? menuQuery.OrderByDescending(m => m.Name) : menuQuery.OrderBy(m => m.Name);
                    break;
            }
            var menuEntities = await menuQuery.Skip(skip).Take(query.Limit).ToListAsync();
            var menus = menuEntities.Select(m => new Menu
            {
                Id = m.Id,
                Name = m.Name,
                MenuGroupName = m.MenuGroup.Name,
                StartingPath = m.StartingPath,
                Position = m.Position,
                Icon = m.Icon
            }).ToList();

            return new QueryResult<Menu> { Result = menus, Count = count };
        }

        
    }
}
