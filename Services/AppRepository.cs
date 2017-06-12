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
using naturalgas.Helpers.Core;
using naturalgas.Helpers.Customer;
using naturalgas.Models.Customer;
using Newtonsoft.Json;
using NaturalGas.Models.Customer;
using NaturalGas.Helpers.Customer;

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
                _context.Customer.Where(c => !c.IsDelete)
                .ApplySort(CustomersResourceParameters.OrderBy,
                _propertyMappingService.GetPropertyMapping<CustomerDto, Customer>());

            if (!string.IsNullOrEmpty(CustomersResourceParameters.SearchQuery))
            {
                // trim & ignore casing
                var searchQueryForWhereClause = CustomersResourceParameters.SearchQuery
                    .Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(a => a.Firstname.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.Surname.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.NationalID.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.Mobile.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || Convert.ToString(a.DateOfBirth).ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.Email.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.DistributorName.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || Convert.ToString(a.DistributorContact).ToLowerInvariant().Contains(searchQueryForWhereClause));

            }

            return PagedList<Customer>.Create(collectionBeforePaging,
                CustomersResourceParameters.PageNumber,
                CustomersResourceParameters.PageSize);
        }

        public PagedList<Customer> GetCustomers(ExportCustomerResourceParameters CustomersResourceParameters)
        {
            var collectionBeforePaging =
                _context.Customer.Where(c => !c.IsDelete)
                .ApplySort(CustomersResourceParameters.OrderBy,
                _propertyMappingService.GetPropertyMapping<CustomerDto, Customer>());

            if (!string.IsNullOrEmpty(CustomersResourceParameters.SearchQuery))
            {
                // trim & ignore casing
                var searchQueryForWhereClause = CustomersResourceParameters.SearchQuery
                    .Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(a => a.Firstname.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.Surname.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.NationalID.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.Mobile.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || Convert.ToString(a.DateOfBirth).ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.Email.ToLowerInvariant().Contains(searchQueryForWhereClause)
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
                        Name = a.Firstname + " " + a.Surname
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
                .OrderBy(a => a.Firstname)
                .ToList();
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

        public NationalIDResponse ValidateNationalId(string nationalId)
        {
            string response = GetCustomerByNationalID(nationalId);
            NationalIDResponse customerObj = GetCustomerFromResponse(response);
            return customerObj;
        }

        private NationalIDResponse GetCustomerFromResponse(string response)
        {
            NationalIDResponse objResponse = JsonConvert.DeserializeObject<NationalIDResponse>(response);

            return objResponse;
        }

        private string GetCustomerByNationalID(string nationalId)
        {
            if (nationalId.Equals("1234567890"))
            {
                return @"{
	                    	'ErrorCode' : '',
	                    	'ErrorMessage' : '',
	                    	'ErrorOcurred' : false,
	                    	'Citizenship' : null,
	                    	'Clan' : null,
	                    	'Date_of_Birth' : '10\/7\/1981 12:00:00 AM',
	                    	'Date_of_Death' : null,
	                    	'Ethnic_Group' : null,
	                    	'Family' : null,
	                    	'Fingerprint' : null,
	                    	'First_Name' : 'Nick',
	                    	'Gender' : 'M',
	                    	'ID_Number' : null,
	                    	'Occupation' : null,
	                    	'Other_Name' : 'Nicky',
	                    	'Photo' : null,
	                    	'Pin' : '12345',
	                    	'Place_of_Birth' : null,
	                    	'Place_of_Death' : null,
	                    	'Place_of_Live' : 'test address for national id 1234567890',
	                    	'Signature' : null,
	                    	'Surname' : 'Jones',
	                    	'Date_of_Issue' : null,
	                    	'RegOffice' : null,
	                    	'Serial_Number' : '1234567890'
	                    }";
            }
            else if (nationalId.Equals("2345678901"))
            {
                return @"{
	                	'ErrorCode' : '',
	                	'ErrorMessage' : '',
	                	'ErrorOcurred' : false,
	                	'Citizenship' : null,
	                	'Clan' : null,
	                	'Date_of_Birth' : '1\/12\/1980 12:00:00 AM',
	                	'Date_of_Death' : null,
	                	'Ethnic_Group' : null,
	                	'Family' : null,
	                	'Fingerprint' : null,
	                	'First_Name' : 'Angelina',
	                	'Gender' : 'F',
	                	'ID_Number' : null,
	                	'Occupation' : null,
	                	'Other_Name' : 'Angel',
	                	'Photo' : null,
	                	'Pin' : '12345',
	                	'Place_of_Birth' : null,
	                	'Place_of_Death' : null,
	                	'Place_of_Live' : 'test address for national id 2345678901',
	                	'Signature' : null,
	                	'Surname' : 'Jolie',
	                	'Date_of_Issue' : null,
	                	'RegOffice' : null,
	                	'Serial_Number' : '2345678901'
	                }";
            }
            else if (nationalId.Equals("3456789012"))
            {
                return @"{
	                    	'ErrorCode' : '',
	                    	'ErrorMessage' : '',
	                    	'ErrorOcurred' : false,
	                    	'Citizenship' : null,
	                    	'Clan' : null,
	                    	'Date_of_Birth' : '5\/24\/1980 12:00:00 AM',
	                    	'Date_of_Death' : null,
	                    	'Ethnic_Group' : null,
	                    	'Family' : null,
	                    	'Fingerprint' : null,
	                    	'First_Name' : 'John',
	                    	'Gender' : 'M',
	                    	'ID_Number' : null,
	                    	'Occupation' : null,
	                    	'Other_Name' : 'Johnny',
	                    	'Photo' : null,
	                    	'Pin' : '12345',
	                    	'Place_of_Birth' : null,
	                    	'Place_of_Death' : null,
	                    	'Place_of_Live' : 'test address for national id 3456789012',
	                    	'Signature' : null,
	                    	'Surname' : 'Doe',
	                    	'Date_of_Issue' : null,
	                    	'RegOffice' : null,
	                    	'Serial_Number' : '3456789012'
	                    }";
            }
            else if (nationalId.Equals("4567890123"))
            {
                return @"{
	                    	'ErrorCode' : '',
	                    	'ErrorMessage' : '',
	                    	'ErrorOcurred' : false,
	                    	'Citizenship' : null,
	                    	'Clan' : null,
	                    	'Date_of_Birth' : '10\/15\/1983 12:00:00 AM',
	                    	'Date_of_Death' : null,
	                    	'Ethnic_Group' : null,
	                    	'Family' : null,
	                    	'Fingerprint' : null,
	                    	'First_Name' : 'Jack',
	                    	'Gender' : 'M',
	                    	'ID_Number' : null,
	                    	'Occupation' : null,
	                    	'Other_Name' : 'Jacky',
	                    	'Photo' : null,
	                    	'Pin' : '12345',
	                    	'Place_of_Birth' : null,
	                    	'Place_of_Death' : null,
	                    	'Place_of_Live' : 'test address for national id 4567890123',
	                    	'Signature' : null,
	                    	'Surname' : 'Sparrow',
	                    	'Date_of_Issue' : null,
	                    	'RegOffice' : null,
	                    	'Serial_Number' : '4567890123'
	                    }";
            }
            else if (nationalId.Equals("5678901234"))
            {
                return @"{
	                    	'ErrorCode' : '',
	                    	'ErrorMessage' : '',
	                    	'ErrorOcurred' : false,
	                    	'Citizenship' : null,
	                    	'Clan' : null,
	                    	'Date_of_Birth' : '10\/15\/1983 12:00:00 AM',
	                    	'Date_of_Death' : null,
	                    	'Ethnic_Group' : null,
	                    	'Family' : null,
	                    	'Fingerprint' : null,
	                    	'First_Name' : 'Brad',
	                    	'Gender' : 'M',
	                    	'ID_Number' : null,
	                    	'Occupation' : null,
	                    	'Other_Name' : 'Jacky',
	                    	'Photo' : null,
	                    	'Pin' : '23456',
	                    	'Place_of_Birth' : null,
	                    	'Place_of_Death' : null,
	                    	'Place_of_Live' : 'test address for national id 5678901234',
	                    	'Signature' : null,
	                    	'Surname' : 'Pitt',
	                    	'Date_of_Issue' : null,
	                    	'RegOffice' : null,
	                    	'Serial_Number' : '5678901234'
	                    }";
            }
            else if (nationalId.Equals("6789012345"))
            {
                return @"{
	                    	'ErrorCode' : '',
	                    	'ErrorMessage' : '',
	                    	'ErrorOcurred' : false,
	                    	'Citizenship' : null,
	                    	'Clan' : null,
	                    	'Date_of_Birth' : '10\/15\/1983 12:00:00 AM',
	                    	'Date_of_Death' : null,
	                    	'Ethnic_Group' : null,
	                    	'Family' : null,
	                    	'Fingerprint' : null,
	                    	'First_Name' : 'Steve',
	                    	'Gender' : 'M',
	                    	'ID_Number' : null,
	                    	'Occupation' : null,
	                    	'Other_Name' : 'Steve',
	                    	'Photo' : null,
	                    	'Pin' : '23456',
	                    	'Place_of_Birth' : null,
	                    	'Place_of_Death' : null,
	                    	'Place_of_Live' : 'test address for national id 6789012345',
	                    	'Signature' : null,
	                    	'Surname' : 'Rogers',
	                    	'Date_of_Issue' : null,
	                    	'RegOffice' : null,
	                    	'Serial_Number' : '6789012345'
	                    }";
            }
            else if (nationalId.Equals("7890123456"))
            {
                return @"{
	                    	'ErrorCode' : '',
	                    	'ErrorMessage' : '',
	                    	'ErrorOcurred' : false,
	                    	'Citizenship' : null,
	                    	'Clan' : null,
	                    	'Date_of_Birth' : '10\/15\/1983 12:00:00 AM',
	                    	'Date_of_Death' : null,
	                    	'Ethnic_Group' : null,
	                    	'Family' : null,
	                    	'Fingerprint' : null,
	                    	'First_Name' : 'Tony',
	                    	'Gender' : 'M',
	                    	'ID_Number' : null,
	                    	'Occupation' : null,
	                    	'Other_Name' : null,
	                    	'Photo' : null,
	                    	'Pin' : '23456',
	                    	'Place_of_Birth' : null,
	                    	'Place_of_Death' : null,
	                    	'Place_of_Live' : 'test address for national id 7890123456',
	                    	'Signature' : null,
	                    	'Surname' : 'Stark',
	                    	'Date_of_Issue' : null,
	                    	'RegOffice' : null,
	                    	'Serial_Number' : '7890123456'
	                    }";
            }
            else if (nationalId.Equals("8901234567"))
            {
                return @"{
	                    	'ErrorCode' : '',
	                    	'ErrorMessage' : '',
	                    	'ErrorOcurred' : false,
	                    	'Citizenship' : null,
	                    	'Clan' : null,
	                    	'Date_of_Birth' : '11\/18\/1983 12:00:00 AM',
	                    	'Date_of_Death' : null,
	                    	'Ethnic_Group' : null,
	                    	'Family' : null,
	                    	'Fingerprint' : null,
	                    	'First_Name' : 'Johnny',
	                    	'Gender' : 'M',
	                    	'ID_Number' : null,
	                    	'Occupation' : null,
	                    	'Other_Name' : null,
	                    	'Photo' : null,
	                    	'Pin' : '23456',
	                    	'Place_of_Birth' : null,
	                    	'Place_of_Death' : null,
	                    	'Place_of_Live' : 'test address for national id 8901234567',
	                    	'Signature' : null,
	                    	'Surname' : 'Depp',
	                    	'Date_of_Issue' : null,
	                    	'RegOffice' : null,
	                    	'Serial_Number' : '8901234567'
	                    }";
            }
            else if (nationalId.Equals("9012345678"))
            {
                return @"{
	                    	'ErrorCode' : '',
	                    	'ErrorMessage' : '',
	                    	'ErrorOcurred' : false,
	                    	'Citizenship' : null,
	                    	'Clan' : null,
	                    	'Date_of_Birth' : '11\/18\/1983 12:00:00 AM',
	                    	'Date_of_Death' : null,
	                    	'Ethnic_Group' : null,
	                    	'Family' : null,
	                    	'Fingerprint' : null,
	                    	'First_Name' : 'Tom',
	                    	'Gender' : 'M',
	                    	'ID_Number' : null,
	                    	'Occupation' : null,
	                    	'Other_Name' : 'Tommy',
	                    	'Photo' : null,
	                    	'Pin' : '23456',
	                    	'Place_of_Birth' : null,
	                    	'Place_of_Death' : null,
	                    	'Place_of_Live' : '9012345678',
	                    	'Signature' : null,
	                    	'Surname' : 'Cruise',
	                    	'Date_of_Issue' : null,
	                    	'RegOffice' : null,
	                    	'Serial_Number' : '9012345678'
	                    }";
            }

            return "{'ErrorOcurred':true,'ErrorCode':'ISB-105','ErrorMessage':'There is no information for requested search parameters'}";

        }

        public IEnumerable<CustomerRegistrationReportDto> GetCustomerRegistrationReport(
            CustomerRegistrationReportParameters CustomerRegistrationReportParameters)
        {



            if (CustomerRegistrationReportParameters.Year != 0)
            {
                return _context.Customer
                        .Where(c => c.CreatedOn.Year == CustomerRegistrationReportParameters.Year)
                        .GroupBy(c => new { c.CreatedOn.Month })
                        .Select(g => new CustomerRegistrationReportDto()
                        {
                            CustomerCount = g.Count(),
                            MaleCount = g.Where(c => c.Gender == "M").Count(),
                            FemaleCount = g.Where(c => c.Gender == "F").Count(),
                            Month = g.Key.Month
                        })
                        .OrderBy(c => c.Month);



            }
            else
            {
				return _context.Customer
                        .Where(c => c.CreatedOn.Year >= DateTime.Now.AddYears(-5).Year)
                        .GroupBy(c => new { c.CreatedOn.Year })
                        .Select(g => new CustomerRegistrationReportDto()
                        {
                            CustomerCount = g.Count(),
                            MaleCount = g.Where(c => c.Gender == "M").Count(),
                            FemaleCount = g.Where(c => c.Gender == "F").Count(),
                            Year = g.Key.Year
                        })
                       .OrderBy(c => c.Year);
            }

        }


        #endregion Customer
    }
}
