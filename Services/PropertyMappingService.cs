using ESPL.NG.Entities;
using ESPL.NG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESPL.NG.Entities.Core;
using ESPL.NG.Models.Core;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ESPL.NG.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _esplUserPropertyMapping =
          new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
          {
               { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
               { "FirstName", new PropertyMappingValue(new List<string>() { "FirstName" } )},
               { "LastName", new PropertyMappingValue(new List<string>() { "LastName" } )},
               { "Email", new PropertyMappingValue(new List<string>() { "Email" } )},
               { "UserName", new PropertyMappingValue(new List<string>() { "UserName" } )}
          };

        private Dictionary<string, PropertyMappingValue> _customerPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
        {
            { "CustomerID", new PropertyMappingValue(new List<string>() { "CustomerID" } ) },
            { "NationalID", new PropertyMappingValue(new List<string>() { "NationalID" } ) },
            { "SerialNumber", new PropertyMappingValue(new List<string>() { "SerialNumber" } ) },
            { "Firstname", new PropertyMappingValue(new List<string>() { "Firstname" } ) },
            { "Surname", new PropertyMappingValue(new List<string>() { "Surname" } ) },
            { "Othername", new PropertyMappingValue(new List<string>() { "Othername" } ) },
            { "Mobile", new PropertyMappingValue(new List<string>() { "Mobile" } ) },
            { "Email", new PropertyMappingValue(new List<string>() { "Email" } ) },
            { "Gender", new PropertyMappingValue(new List<string>() { "Gender" } ) },
            { "DateOfBirth", new PropertyMappingValue(new List<string>() { "DateOfBirth" } ) },
            { "Citizenship", new PropertyMappingValue(new List<string>() { "Citizenship" } ) },
            { "Occupation", new PropertyMappingValue(new List<string>() { "Occupation" } ) },
            { "Pin", new PropertyMappingValue(new List<string>() { "Pin" } ) },
            { "Address", new PropertyMappingValue(new List<string>() { "Address" } ) },
            { "Status", new PropertyMappingValue(new List<string>() { "Status" } ) },
            { "DistributorName", new PropertyMappingValue(new List<string>() { "DistributorName" } ) },
            { "DistributorAddress", new PropertyMappingValue(new List<string>() { "DistributorAddress" } ) },
            { "DistributorContact", new PropertyMappingValue(new List<string>() { "DistributorContact" } ) },
        };

        private IList<IPropertyMapping> propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            propertyMappings.Add(new PropertyMapping<AppUserDto, AppUser>(_esplUserPropertyMapping));
            propertyMappings.Add(new PropertyMapping<CustomerDto, Customer>(_customerPropertyMapping));
        }
        public Dictionary<string, PropertyMappingValue> GetPropertyMapping
            <TSource, TDestination>()
        {
            // get matching mapping
            var matchingMapping = propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance for <{typeof(TSource)},{typeof(TDestination)}");
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            // the string is separated by ",", so we split it.
            var fieldsAfterSplit = fields.Split(',');

            // run through the fields clauses
            foreach (var field in fieldsAfterSplit)
            {
                // trim
                var trimmedField = field.Trim();

                // remove everything after the first " " - if the fields 
                // are coming from an orderBy string, this part must be 
                // ignored
                var indexOfFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexOfFirstSpace);

                // find the matching property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;

        }

    }
}
