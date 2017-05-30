using ESPL.NG.Models;
using ESPL.NG.Services;
using Microsoft.AspNetCore.Mvc;
using ESPL.NG.Entities;
using AutoMapper;
using System.Collections.Generic;
using ESPL.NG.Helpers;
using System.Linq;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using ESPL.NG.Helpers.Customer;

namespace naturalgas.Controllers.Customers
{
    [Route("api/customers")]
    public class CustomerController : Controller
    {
        private IAppRepository _appRepository;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;
        public CustomerController(IAppRepository appRepository,
           IUrlHelper urlHelper,
           IPropertyMappingService propertyMappingService,
           ITypeHelperService typeHelperService)
        {
            _appRepository = appRepository;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
        }

        [HttpGet(Name = "GetCustomers")]
        [HttpHead]
        //[Authorize(Policy = Permissions.CustomerRead)]
        public IActionResult GetCustomers(CustomerResourceParameters customerResourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<CustomerDto, Customer>
               (customerResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<CustomerDto>
                (customerResourceParameters.Fields))
            {
                return BadRequest();
            }

            var customerFromRepo = _appRepository.GetCustomers(customerResourceParameters);

            var customer = Mapper.Map<IEnumerable<CustomerDto>>(customerFromRepo);

            if (mediaType == "application/vnd.marvin.hateoas+json")
            {
                var paginationMetadata = new
                {
                    totalCount = customerFromRepo.TotalCount,
                    pageSize = customerFromRepo.PageSize,
                    currentPage = customerFromRepo.CurrentPage,
                    totalPages = customerFromRepo.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));
                Response.Headers.Add("Access-Control-Expose-Headers", "ETag, X-Pagination");

                var links = CreateLinksForCustomer(customerResourceParameters,
                    customerFromRepo.HasNext, customerFromRepo.HasPrevious);

                var shapedcustomer = customer.ShapeData(customerResourceParameters.Fields);

                var shapedcustomerWithLinks = shapedcustomer.Select(occType =>
                {
                    var customerAsDictionary = occType as IDictionary<string, object>;
                    var customerLinks = CreateLinksForCustomer(
                        (Guid)customerAsDictionary["Id"], customerResourceParameters.Fields);

                    customerAsDictionary.Add("links", customerLinks);

                    return customerAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = shapedcustomerWithLinks,
                    links = links
                };

                return Ok(linkedCollectionResource);
            }
            else
            {
                var previousPageLink = customerFromRepo.HasPrevious ?
                    CreateCustomerResourceUri(customerResourceParameters,
                    ResourceUriType.PreviousPage) : null;

                var nextPageLink = customerFromRepo.HasNext ?
                    CreateCustomerResourceUri(customerResourceParameters,
                    ResourceUriType.NextPage) : null;

                var paginationMetadata = new
                {
                    previousPageLink = previousPageLink,
                    nextPageLink = nextPageLink,
                    totalCount = customerFromRepo.TotalCount,
                    pageSize = customerFromRepo.PageSize,
                    currentPage = customerFromRepo.CurrentPage,
                    totalPages = customerFromRepo.TotalPages
                };

                Response.Headers.Add("X-Pagination",
                    Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));
                Response.Headers.Add("Access-Control-Expose-Headers", "ETag, X-Pagination");

                return Ok(customer.ShapeData(customerResourceParameters.Fields));
            }
        }

        [HttpGet("LookUp", Name = "GetCustomerAsLookUp")]
        //[Authorize(Policy = Permissions.CustomerRead)]
        public IActionResult GetCustomerAsLookUp([FromHeader(Name = "Accept")] string mediaType)
        {
            return Ok(_appRepository.GetCustomerAsLookUp());
        }

        [HttpGet("{id}", Name = "GetCustomer")]
        //[Authorize(Policy = Permissions.CustomerRead)]
        public IActionResult GetCustomer(Guid id, [FromQuery] string fields)
        {
            if (!_typeHelperService.TypeHasProperties<CustomerDto>
              (fields))
            {
                return BadRequest();
            }

            var customerFromRepo = _appRepository.GetCustomer(id);

            if (customerFromRepo == null)
            {
                return NotFound();
            }

            var customer = Mapper.Map<CustomerDto>(customerFromRepo);

            var links = CreateLinksForCustomer(id, fields);

            var linkedResourceToReturn = customer.ShapeData(fields)
                as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return Ok(linkedResourceToReturn);
        }

        [HttpPost(Name = "CreateCustomer")]
        //[Authorize(Policy = Permissions.CustomerCreate)]
        // [RequestHeaderMatchesMediaType("Content-Type",
        //     new[] { "application/vnd.marvin.customer.full+json" })]
        public IActionResult CreateCustomer([FromBody] CustomerForCreationDto customer)
        {
            if (customer == null)
            {
                return BadRequest();
            }

            var customerEntity = Mapper.Map<Customer>(customer);

            SetCreationUserData(customerEntity);

            _appRepository.AddCustomer(customerEntity);

            if (!_appRepository.Save())
            {
                throw new Exception("Creating an customer failed on save.");
                // return StatusCode(500, "A problem happened with handling your request.");
            }

            var customerToReturn = Mapper.Map<CustomerDto>(customerEntity);

            var links = CreateLinksForCustomer(customerToReturn.CustomerID, null);

            var linkedResourceToReturn = customerToReturn.ShapeData(null)
                as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetCustomer",
                new { id = linkedResourceToReturn["CustomerID"] },
                linkedResourceToReturn);
        }

        [HttpPost("{id}")]
        public IActionResult BlockCustomerCreation(Guid id)
        {
            if (_appRepository.CustomerExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        [HttpDelete("{id}", Name = "DeleteCustomer")]
        //[Authorize(Policy = Permissions.CustomerDelete)]
        public IActionResult DeleteCustomer(Guid id)
        {
            var customerFromRepo = _appRepository.GetCustomer(id);
            if (customerFromRepo == null)
            {
                return NotFound();
            }

            //_appRepository.DeleteCustomer(customerFromRepo);
            //....... Soft Delete
            customerFromRepo.IsDelete = true;

            if (!_appRepository.Save())
            {
                throw new Exception($"Deleting customer {id} failed on save.");
            }

            return NoContent();
        }

        [HttpPut("{id}", Name = "UpdateCustomer")]
        //[Authorize(Policy = Permissions.CustomerUpdate)]
        public IActionResult UpdateCustomer(Guid id, [FromBody] CustomerForUpdationDto customer)
        {
            if (customer == null)
            {
                return BadRequest();
            }
            var customerFromRepo = _appRepository.GetCustomer(id);

            if (customerFromRepo == null)
            {
                return NotFound();
            }
            SetItemHistoryData(customer, customerFromRepo);

            Mapper.Map(customer, customerFromRepo);
            _appRepository.UpdateCustomer(customerFromRepo);
            if (!_appRepository.Save())
            {
                throw new Exception("Updating an customer failed on save.");
                // return StatusCode(500, "A problem happened with handling your request.");
            }
            return Ok(customerFromRepo);
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateCustomer")]
        //[Authorize(Policy = Permissions.CustomerUpdate)]
        public IActionResult PartiallyUpdateCustomer(Guid id,
                    [FromBody] JsonPatchDocument<CustomerForUpdationDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var bookForAuthorFromRepo = _appRepository.GetCustomer(id);

            if (bookForAuthorFromRepo == null)
            {
                return NotFound();
            }

            var bookToPatch = Mapper.Map<CustomerForUpdationDto>(bookForAuthorFromRepo);

            patchDoc.ApplyTo(bookToPatch, ModelState);

            // patchDoc.ApplyTo(bookToPatch);

            TryValidateModel(bookToPatch);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            SetItemHistoryData(bookToPatch, bookForAuthorFromRepo);
            Mapper.Map(bookToPatch, bookForAuthorFromRepo);

            _appRepository.UpdateCustomer(bookForAuthorFromRepo);

            if (!_appRepository.Save())
            {
                throw new Exception($"Patching  Occurrence Book {id} failed on save.");
            }
            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetCustomerOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok();
        }

        private string CreateCustomerResourceUri(
            CustomerResourceParameters customerResourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetCustomer",
                      new
                      {
                          fields = customerResourceParameters.Fields,
                          orderBy = customerResourceParameters.OrderBy,
                          searchQuery = customerResourceParameters.SearchQuery,
                          pageNumber = customerResourceParameters.PageNumber - 1,
                          pageSize = customerResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetCustomer",
                      new
                      {
                          fields = customerResourceParameters.Fields,
                          orderBy = customerResourceParameters.OrderBy,
                          searchQuery = customerResourceParameters.SearchQuery,
                          pageNumber = customerResourceParameters.PageNumber + 1,
                          pageSize = customerResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return _urlHelper.Link("GetCustomer",
                    new
                    {
                        fields = customerResourceParameters.Fields,
                        orderBy = customerResourceParameters.OrderBy,
                        searchQuery = customerResourceParameters.SearchQuery,
                        pageNumber = customerResourceParameters.PageNumber,
                        pageSize = customerResourceParameters.PageSize
                    });
            }
        }

        private IEnumerable<LinkDto> CreateLinksForCustomer(Guid id, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                  new LinkDto(_urlHelper.Link("GetCustomer", new { id = id }),
                  "self",
                  "GET"));
            }
            else
            {
                links.Add(
                  new LinkDto(_urlHelper.Link("GetCustomer", new { id = id, fields = fields }),
                  "self",
                  "GET"));
            }

            links.Add(
              new LinkDto(_urlHelper.Link("DeleteCustomer", new { id = id }),
              "delete_customer",
              "DELETE"));

            links.Add(
              new LinkDto(_urlHelper.Link("CreateBookForCustomer", new { customerId = id }),
              "create_book_for_customer",
              "POST"));

            links.Add(
               new LinkDto(_urlHelper.Link("GetBooksForCustomer", new { customerId = id }),
               "books",
               "GET"));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForCustomer(
            CustomerResourceParameters customerResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(
               new LinkDto(CreateCustomerResourceUri(customerResourceParameters,
               ResourceUriType.Current)
               , "self", "GET"));

            if (hasNext)
            {
                links.Add(
                  new LinkDto(CreateCustomerResourceUri(customerResourceParameters,
                  ResourceUriType.NextPage),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateCustomerResourceUri(customerResourceParameters,
                    ResourceUriType.PreviousPage),
                    "previousPage", "GET"));
            }

            return links;
        }

        private void SetItemHistoryData(CustomerForUpdationDto model, Customer modelRepo)
        {
            // model.CreatedOn = modelRepo.CreatedOn;
            // if (modelRepo.CreatedBy != null)
            //     model.CreatedBy = modelRepo.CreatedBy.Value;
            // model.UpdatedOn = DateTime.Now;
            // var CustomerID = User.Claims.FirstOrDefault(cl => cl.Type == "CustomerID");
            // model.UpdatedBy = new Guid(CustomerID.Value);
        }

        private void SetCreationUserData(Customer model)
        {
            // var CustomerID = User.Claims.FirstOrDefault(cl => cl.Type == "CustomerID");
            // model.CreatedBy = new Guid(CustomerID.Value);
        }

    }
}