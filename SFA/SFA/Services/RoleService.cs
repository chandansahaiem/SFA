using Microsoft.EntityFrameworkCore;
using SFA.Entities;
using SFA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.Services
{
    public interface IRoleService
    {
        Task<List<Role>> GetAll();
        Task<QueryResult<Role>> Search(RoleQuery query);
        Task<Role> GetById(Guid id);
        Task<bool> Save(Role role);
    }
    public class RoleService : IRoleService
    {
        private readonly RNDDataContext _context = null;

        public RoleService(RNDDataContext context)
        {
            _context = context;
        }

        public async Task<List<Role>> GetAll()
        {
            var roleEntities = await _context.TblRole.OrderBy(m => m.Name).ToListAsync();
            return roleEntities.Select(m => new Role
            {
                Id = m.Id,
                Name = m.Name,
                IsActive = m.IsActive,
                DataAccessCode = m.DataAccessCode
            }).ToList();
        }

        public async Task<QueryResult<Role>> Search(RoleQuery query)
        {
            try
            {
                var skip = (query.Page - 1) * query.Limit;
                var roleQuery = _context.TblRole.AsNoTracking().AsQueryable();

                if (!string.IsNullOrEmpty(query.Name))
                {
                    roleQuery = roleQuery.Where(m => m.Name.Contains(query.Name));
                }
                var count = await roleQuery.CountAsync();

                switch (query.Order.ToLower())
                {
                    default:
                        roleQuery = query.Order.StartsWith("-") ? roleQuery.OrderByDescending(m => m.Name) : roleQuery.OrderBy(m => m.Name);
                        break;
                }
                var roleEntities = await roleQuery.Skip(skip).Take(query.Limit).ToListAsync();
                var roles = roleEntities.Select(m => new Role
                {
                    Id = m.Id,
                    Name = m.Name,
                    IsActive = m.IsActive,
                    DataAccessCode = m.DataAccessCode
                }).ToList();

                return new QueryResult<Role> { Result = roles, Count = count };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Role> GetById(Guid id)
        {
            var roleEntity = await _context.TblRole.FirstOrDefaultAsync(m => m.Id == id);
            return roleEntity == null ? null : new Role
            {
                Id = roleEntity.Id,
                Name = roleEntity.Name,
                IsActive = roleEntity.IsActive,
                DataAccessCode = roleEntity.DataAccessCode
            };
        }
        public async Task<bool> Save(Role role)
        {
            if (role.Id == Guid.Empty)
            {
                var roleEntity = new TblRole
                {
                    Id = Guid.NewGuid(),
                    Name = role.Name,
                    DataAccessCode = role.DataAccessCode,
                    IsActive = role.IsActive
                };
                _context.TblRole.Add(roleEntity);
            }
            else
            {
                var roleEntity = await _context.TblRole.FirstOrDefaultAsync(m => m.Id == role.Id);
                roleEntity.Name = role.Name;
                roleEntity.IsActive = role.IsActive;
                roleEntity.DataAccessCode = role.DataAccessCode;
            }
            try
            {
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
