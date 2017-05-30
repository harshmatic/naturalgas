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
        List<Customer> GetCustomers(CustomerResourceParameters CustomersResourceParameters);
        Customer GetCustomer(Guid CustomerId);
        IEnumerable<Customer> GetCustomers(IEnumerable<Guid> CustomerIds);
        void AddCustomer(Customer Customer);
        void DeleteCustomer(Customer Customer);
        void UpdateCustomer(Customer Customer);
        bool CustomerExists(Guid authorId);
        IEnumerable<LookUpItem> GetCustomerAsLookUp();


        #endregion
    }
}