using ESPL.NG.Entities;
using ESPL.NG.Helpers;
using ESPL.NG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using ESPL.NG.Helpers.Core;
using ESPL.NG.Entities.Core;
using ESPL.NG.Models.Core;
using ESPL.NG.Helpers.Employee;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using naturalgas.Entities.Core;

namespace ESPL.NG.Services
{
    public class AppRepository : IAppRepository
    {
        private Entities.ApplicationContext _context;
        private IPropertyMappingService _propertyMappingService;
        private RoleManager<IdentityRole> _roleMgr;
        private UserManager<AppUser> _userMgr;

        public AppRepository(Entities.ApplicationContext context,
            IPropertyMappingService propertyMappingService,
            UserManager<AppUser> userMgr,
            RoleManager<IdentityRole> roleMgr)
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
            _userMgr = userMgr;
            _roleMgr = roleMgr;
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        #region AppUser

        public PagedList<AppUser> GetAppUsers(AppUsersResourceParameters esplUserResourceParameters)
        {
            var collectionBeforePaging =
               _userMgr.Users.ApplySort(esplUserResourceParameters.OrderBy,
                _propertyMappingService.GetPropertyMapping<AppUserDto, AppUser>());

            if (!string.IsNullOrEmpty(esplUserResourceParameters.SearchQuery))
            {
                // trim & ignore casing
                var searchQueryForWhereClause = esplUserResourceParameters.SearchQuery
                    .Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(a => a.FirstName.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.LastName.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.Email.ToLowerInvariant().Contains(searchQueryForWhereClause));
            }

            return PagedList<AppUser>.Create(collectionBeforePaging,
                esplUserResourceParameters.PageNumber,
                esplUserResourceParameters.PageSize);
        }

        public AppUser GetAppUser(Guid esplUserId)
        {
            return _userMgr.Users.FirstOrDefault(a => a.Id == esplUserId.ToString());
        }

        public IEnumerable<AppUser> GetAppUsers(IEnumerable<Guid> esplUserIds)
        {
            return _userMgr.Users.Where(a => esplUserIds.Contains(new Guid(a.Id)))
                .OrderBy(a => a.FirstName)
                .OrderBy(a => a.LastName)
                .ToList();
        }

        public void AddAppUser(AppUser esplUser)
        {
            _userMgr.CreateAsync(esplUser);
        }

        public async void DeleteAppUser(AppUser esplUser)
        {
            await _userMgr.DeleteAsync(esplUser);
        }

        public void UpdateAppUser(AppUser esplUser)
        {
            // no code in this implementation
        }

        public bool AppUserExists(Guid esplUserId)
        {
            return _userMgr.Users.Any(a => a.Id == esplUserId.ToString());
        }

        #endregion AppUser

        #region Employee
        public PagedList<Employee> GetEmployees(EmployeesResourceParameters employeesResourceParameters)
        {
            var collectionBeforePaging =
                _context.Employee
                .Where(a => a.IsDelete == false)
                .Include(e => e.AppUser)
                .ApplySort(employeesResourceParameters.OrderBy,
                _propertyMappingService.GetPropertyMapping<EmployeeDto, Employee>());

            if (!string.IsNullOrEmpty(employeesResourceParameters.SearchQuery))
            {
                // trim & ignore casing
                var searchQueryForWhereClause = employeesResourceParameters.SearchQuery
                    .Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(a => a.FirstName.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.LastName.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.EmployeeCode.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || Convert.ToString(a.DateOfBirth).ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.Gender.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.Mobile.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.Email.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.AppUser.UserName.ToLowerInvariant().Contains(searchQueryForWhereClause));

            }

            return PagedList<Employee>.Create(collectionBeforePaging,
                employeesResourceParameters.PageNumber,
                employeesResourceParameters.PageSize);
        }

        public IEnumerable<LookUpItem> GetEmployeeAsLookUp()
        {
            return (from a in _context.Employee
                    where a.IsDelete == false
                    select new LookUpItem
                    {
                        ID = a.EmployeeID,
                        Name = a.FirstName + " " + a.LastName
                    }).ToList();
        }

        public Employee GetEmployee(Guid employeeId)
        {
            return _context.Employee
                .Where(a => a.IsDelete == false)
                .Include(e => e.AppUser)
                .FirstOrDefault(a => a.EmployeeID == employeeId);
        }

        public Employee GetEmployeeByUserID(Guid userId)
        {
            return _context.Employee
                .Where(a => a.IsDelete == false)
                .Include(e => e.AppUser)
                .FirstOrDefault(a => a.UserID == userId.ToString());
        }


        public IEnumerable<Employee> GetEmployees(IEnumerable<Guid> employeeIds)
        {
            return _context.Employee
                .Where(a => a.IsDelete == false)
                .Include(e => e.AppUser)
                .Where(a => employeeIds.Contains(a.EmployeeID))
                .OrderBy(a => a.FirstName)
                .ToList();
        }

        public void AddEmployee(Employee employee)
        {
            employee.EmployeeID = Guid.NewGuid();
            _context.Employee.Add(employee);
        }

        public void DeleteEmployee(Employee employee)
        {
            _context.Employee.Remove(employee);
        }

        public void UpdateEmployee(Employee employee)
        {
            // no code in this implementation
        }

        public bool EmployeeExists(Guid employeeId)
        {
            return _context.Employee.Any(a => a.EmployeeID == employeeId && a.IsDelete == false);
        }

        public IEnumerable<LookUpItem> GetUsersWithoutEmployees()
        {
            return _userMgr.Users
                .Where(u => !_context.Employee.Any(e => e.UserID == u.Id))
                .Select(u => new LookUpItem() { ID = new Guid(u.Id), Name = u.UserName })
                .ToList();
        }

        #endregion Employee
    }
}
