using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ESPL.NG.Entities;
using ESPL.NG.Helpers;
using ESPL.NG.Helpers.Core;
using ESPL.NG.Helpers.Employee;
using ESPL.NG.Models;
using ESPL.NG.Models.Core;
using ESPL.NG.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace KP.Controllers.Employee
{
    [Route("api/employees")]
    [Authorize]
    public class EmployeeController : Controller
    {
        private IAppRepository _appRepository;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        public EmployeeController(IAppRepository appRepository,
            IUrlHelper urlHelper,
            IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService)
        {
            _appRepository = appRepository;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
        }

        [HttpGet(Name = "GetEmployees")]
        [HttpHead]
        public IActionResult GetEmployees(EmployeesResourceParameters employeesResourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<EmployeeDto, ESPL.NG.Entities.Employee>
               (employeesResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<EmployeeDto>
                (employeesResourceParameters.Fields))
            {
                return BadRequest();
            }

            var employeeFromRepo = _appRepository.GetEmployees(employeesResourceParameters);

            var employee = Mapper.Map<IEnumerable<EmployeeDto>>(employeeFromRepo);

            if (mediaType == "application/vnd.marvin.hateoas+json")
            {
                var paginationMetadata = new
                {
                    totalCount = employeeFromRepo.TotalCount,
                    pageSize = employeeFromRepo.PageSize,
                    currentPage = employeeFromRepo.CurrentPage,
                    totalPages = employeeFromRepo.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));
                Response.Headers.Add("Access-Control-Expose-Headers", "X-Pagination");

                var links = CreateLinksForEmployee(employeesResourceParameters,
                    employeeFromRepo.HasNext, employeeFromRepo.HasPrevious);

                var shapedemployee = employee.ShapeData(employeesResourceParameters.Fields);

                var shapedemployeeWithLinks = shapedemployee.Select(occType =>
                {
                    var employeeAsDictionary = occType as IDictionary<string, object>;
                    var employeeLinks = CreateLinksForEmployee(
                        (Guid)employeeAsDictionary["Id"], employeesResourceParameters.Fields);

                    employeeAsDictionary.Add("links", employeeLinks);

                    return employeeAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = shapedemployeeWithLinks,
                    links = links
                };

                return Ok(linkedCollectionResource);
            }
            else
            {
                var previousPageLink = employeeFromRepo.HasPrevious ?
                    CreateEmployeeResourceUri(employeesResourceParameters,
                    ResourceUriType.PreviousPage) : null;

                var nextPageLink = employeeFromRepo.HasNext ?
                    CreateEmployeeResourceUri(employeesResourceParameters,
                    ResourceUriType.NextPage) : null;

                var paginationMetadata = new
                {
                    previousPageLink = previousPageLink,
                    nextPageLink = nextPageLink,
                    totalCount = employeeFromRepo.TotalCount,
                    pageSize = employeeFromRepo.PageSize,
                    currentPage = employeeFromRepo.CurrentPage,
                    totalPages = employeeFromRepo.TotalPages
                };

                Response.Headers.Add("X-Pagination",
                    Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));
                Response.Headers.Add("Access-Control-Expose-Headers", "X-Pagination");

                return Ok(employee.ShapeData(employeesResourceParameters.Fields));
            }
        }

        [HttpGet("LookUp", Name = "GetEmployeeAsLookUp")]        
        public IActionResult GetEmployeeAsLookUp([FromHeader(Name = "Accept")] string mediaType)
        {
            return Ok(_appRepository.GetEmployeeAsLookUp());
        }

        [HttpGet("{id}", Name = "GetEmployee")]
        public IActionResult GetEmployee(Guid id, [FromQuery] string fields)
        {
            if (!_typeHelperService.TypeHasProperties<EmployeeDto>
              (fields))
            {
                return BadRequest();
            }

            var employeeFromRepo = _appRepository.GetEmployee(id);

            if (employeeFromRepo == null)
            {
                return NotFound();
            }

            var employee = Mapper.Map<EmployeeDto>(employeeFromRepo);

            var links = CreateLinksForEmployee(id, fields);

            var linkedResourceToReturn = employee.ShapeData(fields)
                as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return Ok(linkedResourceToReturn);
        }

        [HttpPost(Name = "CreateEmployee")]
        // [RequestHeaderMatchesMediaType("Content-Type",
        //     new[] { "application/vnd.marvin.employee.full+json" })]
        public IActionResult CreateEmployee([FromBody] EmployeeForCreationDto employee)
        {
            if (employee == null)
            {
                return BadRequest();
            }

            var employeeEntity = Mapper.Map<Employee>(employee);

            SetCreationUserData(employeeEntity);

            _appRepository.AddEmployee(employeeEntity);

            if (!_appRepository.Save())
            {
                throw new Exception("Creating an employee failed on save.");
                // return StatusCode(500, "A problem happened with handling your request.");
            }

            if (employee.AreaID != null && employee.AreaID != Guid.Empty)
            {
                AddEmployeeAreaHistory(employeeEntity.EmployeeID, employeeEntity);
            }
            if (employee.DesignationID != null && employee.AreaID != Guid.Empty)
            {
                AddEmployeeDesignationHistory(employeeEntity.EmployeeID, employeeEntity);
            }
            if (employee.DepartmentID != null && employee.AreaID != Guid.Empty)
            {
                AddEmployeeDepartmentHistory(employeeEntity.EmployeeID, employeeEntity);
            }
            if (employee.ShiftID != null && employee.AreaID != Guid.Empty)
            {
                AddEmployeeShiftHistory(employeeEntity.EmployeeID, employeeEntity);
            }


            var employeeToReturn = Mapper.Map<EmployeeDto>(employeeEntity);

            var links = CreateLinksForEmployee(employeeToReturn.EmployeeID, null);

            var linkedResourceToReturn = employeeToReturn.ShapeData(null)
                as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetEmployee",
                new { id = linkedResourceToReturn["EmployeeID"] },
                linkedResourceToReturn);
        }

        [HttpPost("{id}")]
        public IActionResult BlockEmployeeCreation(Guid id)
        {
            if (_appRepository.EmployeeExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        [HttpDelete("{id}", Name = "DeleteEmployee")]
        public IActionResult DeleteEmployee(Guid id)
        {
            var employeeFromRepo = _appRepository.GetEmployee(id);
            if (employeeFromRepo == null)
            {
                return NotFound();
            }

            //_appRepository.DeleteEmployee(employeeFromRepo);
            //....... Soft Delete
            employeeFromRepo.IsDelete = true;

            if (!_appRepository.Save())
            {
                throw new Exception($"Deleting employee {id} failed on save.");
            }

            return NoContent();
        }

        [HttpPut("{id}", Name = "UpdateEmployee")]
        public IActionResult UpdateEmployee(Guid id, [FromBody] EmployeeForUpdationDto employee)
        {
            if (employee == null)
            {
                return BadRequest();
            }
            // if (!_appRepository.EmployeeExists(id))
            // {
            //     return NotFound();
            // }
            //Mapper.Map(source,destination);
            var employeeFromRepo = _appRepository.GetEmployee(id);

            if (employeeFromRepo == null)
            {
                // var employeeAdd = Mapper.Map<Employee>(employee);
                // employeeAdd.EmployeeID = id;

                // _appRepository.AddEmployee(employeeAdd);

                // if (!_appRepository.Save())
                // {
                //     throw new Exception($"Upserting book {id} for author {id} failed on save.");
                // }

                // var employeeReturnVal = Mapper.Map<EmployeeDto>(employeeAdd);

                // return CreatedAtRoute("GetEmployee",
                //     new { OBID = employeeReturnVal.EmployeeID },
                //     employeeReturnVal);
                return NotFound();
            }
            SetItemHistoryData(employee, employeeFromRepo);

            bool isAreaUpdated = false,
                    isDepartmentUpdated = false,
                    isDesignationUpdated = false,
                    isShiftUpdated = false;
            if (employee.AreaID != employeeFromRepo.AreaID)
            {
                isAreaUpdated = true;
            }
            if (employee.DesignationID != employeeFromRepo.DesignationID)
            {
                isDesignationUpdated = true;
            }
            if (employee.DepartmentID != employeeFromRepo.DepartmentID)
            {
                isDepartmentUpdated = true;
            }
            if (employee.ShiftID != employeeFromRepo.ShiftID)
            {
                isShiftUpdated = true;
            }

            Mapper.Map(employee, employeeFromRepo);
            _appRepository.UpdateEmployee(employeeFromRepo);
            if (!_appRepository.Save())
            {
                throw new Exception("Updating an employee failed on save.");
                // return StatusCode(500, "A problem happened with handling your request.");
            }

            if (isAreaUpdated)
            {
                AddEmployeeAreaHistory(id, employeeFromRepo);
            }
            if (isDesignationUpdated)
            {
                AddEmployeeDesignationHistory(id, employeeFromRepo);
            }
            if (isDepartmentUpdated)
            {
                AddEmployeeDepartmentHistory(id, employeeFromRepo);
            }
            if (isShiftUpdated)
            {
                AddEmployeeShiftHistory(id, employeeFromRepo);
            }
            return Ok(employeeFromRepo);
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateEmployee")]
        public IActionResult PartiallyUpdateEmployee(Guid id,
                    [FromBody] JsonPatchDocument<EmployeeForUpdationDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var bookForAuthorFromRepo = _appRepository.GetEmployee(id);

            if (bookForAuthorFromRepo == null)
            {
                // var bookDto = new EmployeeForUpdationDto();
                // patchDoc.ApplyTo(bookDto, ModelState);

                // TryValidateModel(bookDto);

                // if (!ModelState.IsValid)
                // {
                //     return new UnprocessableEntityObjectResult(ModelState);
                // }

                // var bookToAdd = Mapper.Map<Employee>(bookDto);
                // bookToAdd.EmployeeID = id;

                // _appRepository.AddEmployee(bookToAdd);

                // if (!_appRepository.Save())
                // {
                //     throw new Exception($"Upserting in Occurrence Book {id} failed on save.");
                // }

                // var bookToReturn = Mapper.Map<EmployeeDto>(bookToAdd);
                // return CreatedAtRoute("GetEmployee",
                //     new { id = bookToReturn.EmployeeID },
                //     bookToReturn);
                return NotFound();
            }

            var bookToPatch = Mapper.Map<EmployeeForUpdationDto>(bookForAuthorFromRepo);

            patchDoc.ApplyTo(bookToPatch, ModelState);

            // patchDoc.ApplyTo(bookToPatch);

            TryValidateModel(bookToPatch);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            SetItemHistoryData(bookToPatch, bookForAuthorFromRepo);
            Mapper.Map(bookToPatch, bookForAuthorFromRepo);

            _appRepository.UpdateEmployee(bookForAuthorFromRepo);

            if (!_appRepository.Save())
            {
                throw new Exception($"Patching  Occurrence Book {id} failed on save.");
            }

            foreach (var path in patchDoc.Operations)
            {
                if (path.path.ToLowerInvariant().Equals("/areaid"))
                {
                    EmployeeForUpdationDto employeeDto = new EmployeeForUpdationDto();
                    Mapper.Map(bookForAuthorFromRepo, employeeDto);
                    AddEmployeeAreaHistory(id, bookForAuthorFromRepo);
                }
                if (path.path.ToLowerInvariant().Equals("/departmentid"))
                {
                    EmployeeForUpdationDto employeeDto = new EmployeeForUpdationDto();
                    Mapper.Map(bookForAuthorFromRepo, employeeDto);
                    AddEmployeeDepartmentHistory(id, bookForAuthorFromRepo);
                }
                if (path.path.ToLowerInvariant().Equals("/designationid"))
                {
                    EmployeeForUpdationDto employeeDto = new EmployeeForUpdationDto();
                    Mapper.Map(bookForAuthorFromRepo, employeeDto);
                    AddEmployeeDesignationHistory(id, bookForAuthorFromRepo);
                }
                if (path.path.ToLowerInvariant().Equals("/shiftid"))
                {
                    EmployeeForUpdationDto employeeDto = new EmployeeForUpdationDto();
                    Mapper.Map(bookForAuthorFromRepo, employeeDto);
                    AddEmployeeShiftHistory(id, bookForAuthorFromRepo);
                }
            }


            return NoContent();
        }


        [Route("{id}/employeeShifthistory")]
        [HttpGet("{id}/employeeShifthistory", Name = "GetEmployeeShifthistory")]
        public IActionResult GetEmployeeShifthistory(Guid id, EmployeeShiftHistoryResourceParameters employeeShiftHistoryParams,
        [FromHeader(Name = "Accept")]string mediaType)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<EmployeeShiftHistoryDto, CfgEmployeeShift>
               (employeeShiftHistoryParams.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<EmployeeShiftHistoryDto>
                (employeeShiftHistoryParams.Fields))
            {
                return BadRequest();
            }
            var employeeShiftHistoryFromRepo = _appRepository.GetEmployeeShiftHistory(id, employeeShiftHistoryParams);

            var employeeShiftHistory = Mapper.Map<IEnumerable<EmployeeShiftHistoryDto>>(employeeShiftHistoryFromRepo);

            if (mediaType == "application/vnd.marvin.hateoas+json")
            {
                var paginationMetadata = new
                {
                    totalCount = employeeShiftHistoryFromRepo.TotalCount,
                    pageSize = employeeShiftHistoryFromRepo.PageSize,
                    currentPage = employeeShiftHistoryFromRepo.CurrentPage,
                    totalPages = employeeShiftHistoryFromRepo.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));
                Response.Headers.Add("Access-Control-Expose-Headers", "X-Pagination");

                var links = CreateLinksForEmployeeShiftHistory(employeeShiftHistoryParams,
                    employeeShiftHistoryFromRepo.HasNext, employeeShiftHistoryFromRepo.HasPrevious);

                var shapedEmployeeShiftHistory = employeeShiftHistory.ShapeData(employeeShiftHistoryParams.Fields);

                var shapedEmployeeShiftHistoryWithLinks = shapedEmployeeShiftHistory.Select(_employeeShiftHistory =>
                {
                    var employeeShiftAsDictionary = _employeeShiftHistory as IDictionary<string, object>;
                    var employeeShiftLinks = CreateLinksForEmployeeShiftHistory(
                        (Guid)employeeShiftAsDictionary["Id"], employeeShiftHistoryParams.Fields);

                    employeeShiftAsDictionary.Add("links", employeeShiftLinks);

                    return employeeShiftAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = shapedEmployeeShiftHistoryWithLinks,
                    links = links
                };

                return Ok(linkedCollectionResource);
            }
            else
            {
                var previousPageLink = employeeShiftHistoryFromRepo.HasPrevious ?
                    CreateEmployeeShiftHistoryResourceUri(employeeShiftHistoryParams,
                    ResourceUriType.PreviousPage) : null;

                var nextPageLink = employeeShiftHistoryFromRepo.HasNext ?
                    CreateEmployeeShiftHistoryResourceUri(employeeShiftHistoryParams,
                    ResourceUriType.NextPage) : null;

                var paginationMetadata = new
                {
                    previousPageLink = previousPageLink,
                    nextPageLink = nextPageLink,
                    totalCount = employeeShiftHistoryFromRepo.TotalCount,
                    pageSize = employeeShiftHistoryFromRepo.PageSize,
                    currentPage = employeeShiftHistoryFromRepo.CurrentPage,
                    totalPages = employeeShiftHistoryFromRepo.TotalPages
                };

                Response.Headers.Add("X-Pagination",
                    Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));
                Response.Headers.Add("Access-Control-Expose-Headers", "X-Pagination");

                return Ok(employeeShiftHistory.ShapeData(employeeShiftHistoryParams.Fields));
            }
        }

        [Route("{id}/employeeDepartmentHistory")]
        [HttpGet("{id}/employeeDepartmentHistory", Name = "GetEmployeeDepartmentHistory")]
        public IActionResult GetEmployeeDepartmentHistory(Guid id, EmployeeDepartmentHistoryResourceParameters employeeDepartmentHistoryParams,
        [FromHeader(Name = "Accept")]string mediaType)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<EmployeeDepartmentHistoryDto, CfgEmployeeDepartment>
               (employeeDepartmentHistoryParams.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<EmployeeDepartmentHistoryDto>
                (employeeDepartmentHistoryParams.Fields))
            {
                return BadRequest();
            }
            var employeeDepartmentHistoryFromRepo = _appRepository.GetEmployeeDepartmentHistory(id, employeeDepartmentHistoryParams);

            var employeeDepartmentHistory = Mapper.Map<IEnumerable<EmployeeDepartmentHistoryDto>>(employeeDepartmentHistoryFromRepo);

            if (mediaType == "application/vnd.marvin.hateoas+json")
            {
                var paginationMetadata = new
                {
                    totalCount = employeeDepartmentHistoryFromRepo.TotalCount,
                    pageSize = employeeDepartmentHistoryFromRepo.PageSize,
                    currentPage = employeeDepartmentHistoryFromRepo.CurrentPage,
                    totalPages = employeeDepartmentHistoryFromRepo.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));
                Response.Headers.Add("Access-Control-Expose-Headers", "X-Pagination");

                var links = CreateLinksForEmployeeDepartmentHistory(employeeDepartmentHistoryParams,
                    employeeDepartmentHistoryFromRepo.HasNext, employeeDepartmentHistoryFromRepo.HasPrevious);

                var shapedEmployeeDepartmentHistory = employeeDepartmentHistory.ShapeData(employeeDepartmentHistoryParams.Fields);

                var shapedEmployeeDepartmentHistoryWithLinks = shapedEmployeeDepartmentHistory.Select(_employeeDepartmentHistory =>
                {
                    var employeeDepartmentAsDictionary = _employeeDepartmentHistory as IDictionary<string, object>;
                    var employeeDepartmentLinks = CreateLinksForEmployeeDepartmentHistory(
                        (Guid)employeeDepartmentAsDictionary["Id"], employeeDepartmentHistoryParams.Fields);

                    employeeDepartmentAsDictionary.Add("links", employeeDepartmentLinks);

                    return employeeDepartmentAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = shapedEmployeeDepartmentHistoryWithLinks,
                    links = links
                };

                return Ok(linkedCollectionResource);
            }
            else
            {
                var previousPageLink = employeeDepartmentHistoryFromRepo.HasPrevious ?
                    CreateEmployeeDepartmentHistoryResourceUri(employeeDepartmentHistoryParams,
                    ResourceUriType.PreviousPage) : null;

                var nextPageLink = employeeDepartmentHistoryFromRepo.HasNext ?
                    CreateEmployeeDepartmentHistoryResourceUri(employeeDepartmentHistoryParams,
                    ResourceUriType.NextPage) : null;

                var paginationMetadata = new
                {
                    previousPageLink = previousPageLink,
                    nextPageLink = nextPageLink,
                    totalCount = employeeDepartmentHistoryFromRepo.TotalCount,
                    pageSize = employeeDepartmentHistoryFromRepo.PageSize,
                    currentPage = employeeDepartmentHistoryFromRepo.CurrentPage,
                    totalPages = employeeDepartmentHistoryFromRepo.TotalPages
                };

                Response.Headers.Add("X-Pagination",
                    Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));
                Response.Headers.Add("Access-Control-Expose-Headers", "X-Pagination");

                return Ok(employeeDepartmentHistory.ShapeData(employeeDepartmentHistoryParams.Fields));
            }
        }

        [HttpGet("UserLookUp", Name = "GetEmployeeUserLookups")]
        [Authorize(Policy = "IsSuperAdmin")]
        public IActionResult GetEmployeeUserLookups([FromHeader(Name = "Accept")]string mediaType)
        {
            var esplUsersFromRepo = _appRepository.GetUsersWithoutEmployees();
            return Ok(esplUsersFromRepo);
        }

        [HttpOptions]
        public IActionResult GetEmployeeOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok();
        }

        private string CreateEmployeeResourceUri(
            EmployeesResourceParameters employeesResourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetEmployee",
                      new
                      {
                          fields = employeesResourceParameters.Fields,
                          orderBy = employeesResourceParameters.OrderBy,
                          searchQuery = employeesResourceParameters.SearchQuery,
                          pageNumber = employeesResourceParameters.PageNumber - 1,
                          pageSize = employeesResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetEmployee",
                      new
                      {
                          fields = employeesResourceParameters.Fields,
                          orderBy = employeesResourceParameters.OrderBy,
                          searchQuery = employeesResourceParameters.SearchQuery,
                          pageNumber = employeesResourceParameters.PageNumber + 1,
                          pageSize = employeesResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return _urlHelper.Link("GetEmployee",
                    new
                    {
                        fields = employeesResourceParameters.Fields,
                        orderBy = employeesResourceParameters.OrderBy,
                        searchQuery = employeesResourceParameters.SearchQuery,
                        pageNumber = employeesResourceParameters.PageNumber,
                        pageSize = employeesResourceParameters.PageSize
                    });
            }
        }

        private IEnumerable<LinkDto> CreateLinksForEmployee(Guid id, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                  new LinkDto(_urlHelper.Link("GetEmployee", new { id = id }),
                  "self",
                  "GET"));
            }
            else
            {
                links.Add(
                  new LinkDto(_urlHelper.Link("GetEmployee", new { id = id, fields = fields }),
                  "self",
                  "GET"));
            }

            links.Add(
              new LinkDto(_urlHelper.Link("DeleteEmployee", new { id = id }),
              "delete_employee",
              "DELETE"));

            links.Add(
              new LinkDto(_urlHelper.Link("CreateBookForEmployee", new { employeeId = id }),
              "create_book_for_employee",
              "POST"));

            links.Add(
               new LinkDto(_urlHelper.Link("GetBooksForEmployee", new { employeeId = id }),
               "books",
               "GET"));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForEmployee(
            EmployeesResourceParameters employeesResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(
               new LinkDto(CreateEmployeeResourceUri(employeesResourceParameters,
               ResourceUriType.Current)
               , "self", "GET"));

            if (hasNext)
            {
                links.Add(
                  new LinkDto(CreateEmployeeResourceUri(employeesResourceParameters,
                  ResourceUriType.NextPage),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateEmployeeResourceUri(employeesResourceParameters,
                    ResourceUriType.PreviousPage),
                    "previousPage", "GET"));
            }

            return links;
        }

        private void SetItemHistoryData(EmployeeForUpdationDto model, Employee modelRepo)
        {
            model.CreatedOn = modelRepo.CreatedOn;
            if (modelRepo.CreatedBy != null)
                model.CreatedBy = modelRepo.CreatedBy.Value;
            model.UpdatedOn = DateTime.Now;
            var EmployeeID = User.Claims.FirstOrDefault(cl => cl.Type == "EmployeeID");
            model.UpdatedBy = new Guid(EmployeeID.Value);
        }

        private void SetCreationUserData(Employee model)
        {
            var EmployeeID = User.Claims.FirstOrDefault(cl => cl.Type == "EmployeeID");
            model.CreatedBy = new Guid(EmployeeID.Value);
        }

        private void AddEmployeeAreaHistory(Guid employeeId, Employee employee)
        {
            var employeeAreaHistory = Mapper.Map<CfgEmployeeArea>(employee);
            employeeAreaHistory.EmployeeID = employeeId;
            _appRepository.AddEmployeeAreaHistory(employeeAreaHistory);
            if (!_appRepository.Save())
            {
                throw new Exception("Creating an occurrenceBook failed on save.");
                // return StatusCode(500, "Creating an occurrenceBook failed on save.");
            }
        }

        private void AddEmployeeDepartmentHistory(Guid id, Employee employeeDto)
        {
            var employeeDepartmentHistory = Mapper.Map<CfgEmployeeDepartment>(employeeDto);
            employeeDepartmentHistory.EmployeeID = id;
            _appRepository.AddEmployeeDepartmentHistory(employeeDepartmentHistory);
            if (!_appRepository.Save())
            {
                throw new Exception("Creating an occurrenceBook failed on save.");
                // return StatusCode(500, "Creating an occurrenceBook failed on save.");
            }
        }

        private void AddEmployeeDesignationHistory(Guid id, Employee employeeDto)
        {
            var employeeDesignationHistory = Mapper.Map<CfgEmployeeDesignation>(employeeDto);
            employeeDesignationHistory.EmployeeID = id;
            _appRepository.AddEmployeeDesignationHistory(employeeDesignationHistory);
            if (!_appRepository.Save())
            {
                throw new Exception("Creating an occurrenceBook failed on save.");
                // return StatusCode(500, "Creating an occurrenceBook failed on save.");
            }
        }

        private void AddEmployeeShiftHistory(Guid id, Employee employeeDto)
        {
            var employeeDesignationHistory = Mapper.Map<CfgEmployeeShift>(employeeDto);
            employeeDesignationHistory.EmployeeID = id;
            _appRepository.AddEmployeeShiftHistory(employeeDesignationHistory);
            if (!_appRepository.Save())
            {
                throw new Exception("Creating an occurrenceBook failed on save.");
                // return StatusCode(500, "Creating an occurrenceBook failed on save.");
            }
        }

        private IEnumerable<LinkDto> CreateLinksForEmployeeShiftHistory(Guid id, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new LinkDto(_urlHelper.Link("GetEmployeeShiftHistory", new
                    {
                        id = id
                    }),
                        "self",
                        "GET"));
            }
            else
            {
                links.Add(
                    new LinkDto(_urlHelper.Link("GetEmployeeShiftHistory", new
                    {
                        id = id,
                        fields = fields
                    }),
                        "self",
                        "GET"));
            }
            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForEmployeeShiftHistory(
            EmployeeShiftHistoryResourceParameters employeesResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self
            links.Add(
                new LinkDto(CreateEmployeeShiftHistoryResourceUri(employeesResourceParameters,
                        ResourceUriType.Current), "self", "GET"));

            if (hasNext)
            {
                links.Add(
                    new LinkDto(CreateEmployeeShiftHistoryResourceUri(employeesResourceParameters,
                            ResourceUriType.NextPage),
                        "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateEmployeeShiftHistoryResourceUri(employeesResourceParameters,
                            ResourceUriType.PreviousPage),
                        "previousPage", "GET"));
            }

            return links;
        }

        private string CreateEmployeeShiftHistoryResourceUri(
            EmployeeShiftHistoryResourceParameters employeesResourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetEmployeeShiftHistory",
                        new
                        {
                            fields = employeesResourceParameters.Fields,
                            orderBy = employeesResourceParameters.OrderBy,
                            searchQuery = employeesResourceParameters.SearchQuery,
                            pageNumber = employeesResourceParameters.PageNumber - 1,
                            pageSize = employeesResourceParameters.PageSize
                        });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetEmployeeShiftHistory",
                        new
                        {
                            fields = employeesResourceParameters.Fields,
                            orderBy = employeesResourceParameters.OrderBy,
                            searchQuery = employeesResourceParameters.SearchQuery,
                            pageNumber = employeesResourceParameters.PageNumber + 1,
                            pageSize = employeesResourceParameters.PageSize
                        });
                case ResourceUriType.Current:
                default:
                    return _urlHelper.Link("GetEmployeeShiftHistory",
                        new
                        {
                            fields = employeesResourceParameters.Fields,
                            orderBy = employeesResourceParameters.OrderBy,
                            searchQuery = employeesResourceParameters.SearchQuery,
                            pageNumber = employeesResourceParameters.PageNumber,
                            pageSize = employeesResourceParameters.PageSize
                        });
            }
        }

        private IEnumerable<LinkDto> CreateLinksForEmployeeDepartmentHistory(Guid id, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new LinkDto(_urlHelper.Link("GetEmployeeDepartmentHistory", new
                    {
                        id = id
                    }),
                        "self",
                        "GET"));
            }
            else
            {
                links.Add(
                    new LinkDto(_urlHelper.Link("GetEmployeeDepartmentHistory", new
                    {
                        id = id,
                        fields = fields
                    }),
                        "self",
                        "GET"));
            }
            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForEmployeeDepartmentHistory(
            EmployeeDepartmentHistoryResourceParameters employeesResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self
            links.Add(
                new LinkDto(CreateEmployeeDepartmentHistoryResourceUri(employeesResourceParameters,
                        ResourceUriType.Current), "self", "GET"));

            if (hasNext)
            {
                links.Add(
                    new LinkDto(CreateEmployeeDepartmentHistoryResourceUri(employeesResourceParameters,
                            ResourceUriType.NextPage),
                        "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateEmployeeDepartmentHistoryResourceUri(employeesResourceParameters,
                            ResourceUriType.PreviousPage),
                        "previousPage", "GET"));
            }

            return links;
        }

        private string CreateEmployeeDepartmentHistoryResourceUri(
            EmployeeDepartmentHistoryResourceParameters employeesResourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetEmployeeDepartmentHistory",
                        new
                        {
                            fields = employeesResourceParameters.Fields,
                            orderBy = employeesResourceParameters.OrderBy,
                            searchQuery = employeesResourceParameters.SearchQuery,
                            pageNumber = employeesResourceParameters.PageNumber - 1,
                            pageSize = employeesResourceParameters.PageSize
                        });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetEmployeeDepartmentHistory",
                        new
                        {
                            fields = employeesResourceParameters.Fields,
                            orderBy = employeesResourceParameters.OrderBy,
                            searchQuery = employeesResourceParameters.SearchQuery,
                            pageNumber = employeesResourceParameters.PageNumber + 1,
                            pageSize = employeesResourceParameters.PageSize
                        });
                case ResourceUriType.Current:
                default:
                    return _urlHelper.Link("GetEmployeeDepartmentHistory",
                        new
                        {
                            fields = employeesResourceParameters.Fields,
                            orderBy = employeesResourceParameters.OrderBy,
                            searchQuery = employeesResourceParameters.SearchQuery,
                            pageNumber = employeesResourceParameters.PageNumber,
                            pageSize = employeesResourceParameters.PageSize
                        });
            }
        }


    }
}