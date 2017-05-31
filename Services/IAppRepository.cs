using ESPL.NG.Entities;
using ESPL.NG.Helpers;
using System;
using System.Collections.Generic;
using ESPL.NG.Helpers.Core;
using ESPL.NG.Entities.Core;
using ESPL.NG.Helpers.Customer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ESPL.NG.Models;
using naturalgas.Entities.Core;
using naturalgas.Helpers.Core;
using naturalgas.Helpers.Customer;

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

        #region Customer
        PagedList<Customer> GetCustomers(CustomerResourceParameters customersResourceParameters);
        PagedList<Customer> GetCustomers(ExportCustomerResourceParameters customersResourceParameters);
        Customer GetCustomer(Guid customerId);
        IEnumerable<Customer> GetCustomers(IEnumerable<Guid> customerIds);
        void AddCustomer(Customer customer);
        void DeleteCustomer(Customer customer);
        void UpdateCustomer(Customer customer);
        bool CustomerExists(Guid authorId);
        IEnumerable<LookUpItem> GetCustomerAsLookUp();        

        #endregion
    }
}
