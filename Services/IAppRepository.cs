using ESPL.NG.Entities;
using ESPL.NG.Helpers;
using System;
using System.Collections.Generic;
using ESPL.NG.Helpers.Core;
using ESPL.NG.Entities.Core;
using ESPL.NG.Helpers.Employee;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ESPL.NG.Models;
using naturalgas.Entities.Core;

namespace ESPL.NG.Services
{
    public interface IAppRepository
    {
        bool Save();

        #region AppUser

        PagedList<AppUser> GetAppUsers(AppUsersResourceParameters esplUserResourceParameters);
        AppUser GetAppUser(Guid esplUserId);
        IEnumerable<AppUser> GetAppUsers(IEnumerable<Guid> esplUserIds);
        void AddAppUser(AppUser esplUser);
        void DeleteAppUser(AppUser esplUser);
        void UpdateAppUser(AppUser esplUser);
        bool AppUserExists(Guid esplUserId);

        #endregion AppUser

        #region Employee
        PagedList<Employee> GetEmployees(EmployeesResourceParameters employeesResourceParameters);
        Employee GetEmployee(Guid employeeId);
        Employee GetEmployeeByUserID(Guid userId);
        IEnumerable<Employee> GetEmployees(IEnumerable<Guid> employeeIds);
        void AddEmployee(Employee employee);
        void DeleteEmployee(Employee employee);
        void UpdateEmployee(Employee employee);
        bool EmployeeExists(Guid authorId);
        IEnumerable<LookUpItem> GetUsersWithoutEmployees();
        IEnumerable<LookUpItem> GetEmployeeAsLookUp();


        #endregion
    }
}