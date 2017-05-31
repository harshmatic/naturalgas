using ESPL.NG.Entities;
using ESPL.NG.Helpers;
using ESPL.NG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using ESPL.NG.Helpers.Core;
using ESPL.NG.Entities.Core;
using ESPL.NG.Models.Core;
using ESPL.NG.Helpers.Customer;
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

        #region Customer
        public PagedList<Customer> GetCustomers(CustomerResourceParameters CustomersResourceParameters)
        {
            var collectionBeforePaging =
                _context.Customer.Where(c=>!c.IsDelete)
                .ApplySort(CustomersResourceParameters.OrderBy,
                _propertyMappingService.GetPropertyMapping<CustomerDto, Customer>());

            if (!string.IsNullOrEmpty(CustomersResourceParameters.SearchQuery))
            {
                // trim & ignore casing
                var searchQueryForWhereClause = CustomersResourceParameters.SearchQuery
                    .Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(a => a.CustomerName.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.Mobile.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.Landline.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || Convert.ToString(a.DateOfBirth).ToLowerInvariant().Contains(searchQueryForWhereClause)                    
                    || a.CustomerEmail.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.DistributorName.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.DistributorContact.ToLowerInvariant().Contains(searchQueryForWhereClause));

            }

            return PagedList<Customer>.Create(collectionBeforePaging,
                CustomersResourceParameters.PageNumber,
                CustomersResourceParameters.PageSize);
        }

        public IEnumerable<LookUpItem> GetCustomerAsLookUp()
        {
            return (from a in _context.Customer
                    where a.IsDelete == false
                    select new LookUpItem
                    {
                        ID = a.CustomerID,
                        Name = a.CustomerName
                    }).ToList();
        }

        public Customer GetCustomer(Guid CustomerId)
        {
            return _context.Customer
            .Where(a => a.IsDelete == false)
            .FirstOrDefault(a => a.CustomerID == CustomerId);
        }       


        public IEnumerable<Customer> GetCustomers(IEnumerable<Guid> CustomerIds)
        {
            return _context.Customer
                .Where(a => a.IsDelete == false)
                .Where(a => CustomerIds.Contains(a.CustomerID))
                .OrderBy(a => a.CustomerName)
                .ToList();
        }

        public IEnumerable<Customer> GetAllCustomers(CustomerResourceParameters CustomersResourceParameters)
        {
            var collectionBeforePaging =
                _context.Customer.Where(c=>!c.IsDelete)
                .ApplySort(CustomersResourceParameters.OrderBy,
                _propertyMappingService.GetPropertyMapping<CustomerDto, Customer>());

            if (!string.IsNullOrEmpty(CustomersResourceParameters.SearchQuery))
            {
                // trim & ignore casing
                var searchQueryForWhereClause = CustomersResourceParameters.SearchQuery
                    .Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(a => a.CustomerName.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.Mobile.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.Landline.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || Convert.ToString(a.DateOfBirth).ToLowerInvariant().Contains(searchQueryForWhereClause)                    
                    || a.CustomerEmail.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.DistributorName.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.DistributorContact.ToLowerInvariant().Contains(searchQueryForWhereClause));
            }
            return collectionBeforePaging;
        }

        public void AddCustomer(Customer Customer)
        {
            Customer.CustomerID = Guid.NewGuid();
            _context.Customer.Add(Customer);
        }

        public void DeleteCustomer(Customer Customer)
        {
            _context.Customer.Remove(Customer);
        }

        public void UpdateCustomer(Customer Customer)
        {
            // no code in this implementation
        }

        public bool CustomerExists(Guid CustomerId)
        {
            return _context.Customer.Any(a => a.CustomerID == CustomerId && a.IsDelete == false);
        }
        #endregion Customer
    }
}
