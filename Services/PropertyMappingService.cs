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

        private Dictionary<string, PropertyMappingValue> _employeePropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
        {
            { "EmployeeID", new PropertyMappingValue(new List<string>() { "EmployeeID" } ) },
            { "FirstName", new PropertyMappingValue(new List<string>() { "FirstName" } ) },
            { "LastName", new PropertyMappingValue(new List<string>() { "LastName" } ) },
            { "EmployeeCode", new PropertyMappingValue(new List<string>() { "EmployeeCode" } ) },
            { "DateofBirth", new PropertyMappingValue(new List<string>() { "DateofBirth" } ) },
            { "Gender", new PropertyMappingValue(new List<string>() { "Gender" } ) },
            { "Mobile", new PropertyMappingValue(new List<string>() { "Mobile" } ) },
            { "Email", new PropertyMappingValue(new List<string>() { "Email" } ) },
            { "ResidencePhone1", new PropertyMappingValue(new List<string>() { "ResidencePhone1" } ) },
            { "OrganizationJoiningDate", new PropertyMappingValue(new List<string>() { "OrganizationJoiningDate" } ) },
            { "ServiceJoiningDate", new PropertyMappingValue(new List<string>() { "ServiceJoiningDate" } ) },
            { "Address1", new PropertyMappingValue(new List<string>() { "Mobile" } ) },
            { "Address2", new PropertyMappingValue(new List<string>() { "Email" } ) },
            { "AreaID", new PropertyMappingValue(new List<string>() { "ResidencePhone1" } ) },
            { "DepartmentID", new PropertyMappingValue(new List<string>() { "DepartmentID" } ) },
            { "DesignationID", new PropertyMappingValue(new List<string>() { "DesignationID" } ) },
            { "ShiftID", new PropertyMappingValue(new List<string>() { "ShiftID" } ) },
            { "UserID", new PropertyMappingValue(new List<string>() { "UserID" } ) },
            { "MstArea.AreaName", new PropertyMappingValue(new List<string>() { "MstArea.AreaName" } ) },
            { "MstDesignation.DesignationName", new PropertyMappingValue(new List<string>() { "MstDesignation.DesignationName" } ) },
            { "MstDepartment.DepartmentName", new PropertyMappingValue(new List<string>() { "MstDepartment.DepartmentName" } ) },
            { "MstShift.ShiftName", new PropertyMappingValue(new List<string>() { "MstShift.ShiftName" } ) },
            { "MstOccurrenceBooks.MstStatus.StatusName", new PropertyMappingValue(new List<string>() { "MstOccurrenceBooks.MstStatus.StatusName" } ) },
            { "MstOccurrenceBooks.MstOccurrenceType.OBTypeName", new PropertyMappingValue(new List<string>() { "MstOccurrenceBooks.MstOccurrenceType.OBTypeName" } ) },
            { "AppUser.UserName", new PropertyMappingValue(new List<string>() { "AppUser.UserName" } ) }
        };

        private IList<IPropertyMapping> propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            propertyMappings.Add(new PropertyMapping<AppUserDto, AppUser>(_esplUserPropertyMapping));
            propertyMappings.Add(new PropertyMapping<EmployeeDto, Employee>(_employeePropertyMapping));
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
