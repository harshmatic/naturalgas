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
using System.IO;
using OfficeOpenXml;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.NodeServices;

namespace naturalgas.Controllers.Customers
{
    [Route("api/customers")]
    public class CustomerController : Controller
    {
        private IAppRepository _appRepository;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;
        private readonly IHostingEnvironment _hostingEnvironment;
        public CustomerController(IAppRepository appRepository,
           IUrlHelper urlHelper,
           IPropertyMappingService propertyMappingService,
           ITypeHelperService typeHelperService,
           IHostingEnvironment hostingEnvironment)
        {
            _appRepository = appRepository;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
            _hostingEnvironment = hostingEnvironment;
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

        [HttpGet("ExcelReport", Name = "GetCustomersExcel")]
        public IActionResult GetCustomersExcel(CustomerResourceParameters customerResourceParameters,
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

            var customerFromRepo = _appRepository.GetAllCustomers(customerResourceParameters);

            var customers = Mapper.Map<IEnumerable<CustomerDto>>(customerFromRepo);

            var excelFile = GenerateExcel(customers.ToList());

            return excelFile;
        }

        [HttpGet("PdfReport", Name = "GetCustomersPdf")]
        public async Task<IActionResult> GetCustomersPdf(CustomerResourceParameters customerResourceParameters,
            [FromServices] INodeServices nodeServices)
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

            var customerFromRepo = _appRepository.GetAllCustomers(customerResourceParameters);

            var customers = Mapper.Map<IEnumerable<CustomerDto>>(customerFromRepo);
            
            var htmlContent = CreateHTMLTable(customers.ToList());
            var result = await nodeServices.InvokeAsync<byte[]>("./pdfReport", htmlContent);
            HttpContext.Response.ContentType = "application/pdf";
            string filename = @"report.pdf";
            HttpContext.Response.Headers.Add("x-filename", filename);
            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "x-filename");
            HttpContext.Response.Body.Write(result, 0, result.Length);
            return new ContentResult();
        }

        [HttpOptions]
        public IActionResult GetCustomerOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok();
        }

        private FileStreamResult GenerateExcel(List<CustomerDto> customerList)
        {
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = @"ExportedDocuments/CustomerList.xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
            string localFilePath = Path.Combine(sWebRootFolder, sFileName);
            FileInfo file = new FileInfo(localFilePath);
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Customers");

                PropertyInfo[] allProperties = (new Customer()).GetType().GetProperties();
                int count = 1, iterCount = 2;
                foreach (PropertyInfo property in allProperties)
                {
                    worksheet.Cells[1, count].Value = property.Name;
                    count++;
                }
                foreach (CustomerDto customer in customerList)
                {
                    count = 1;
                    foreach (PropertyInfo property in allProperties)
                    {
                        var obj = customer.GetType().GetProperty(property.Name);
                        if (obj != null)
                        {
                            string columnName = GetExcelColumnName(count);
                            worksheet.Cells[columnName + iterCount].Value = obj.GetValue(customer, null);
                            count++;
                        }
                    }
                    iterCount++;
                }
                package.Save(); //Save the workbook.
            }
            FileStream fs = new FileStream(localFilePath, FileMode.Open);
            FileStreamResult fileStreamResult = new FileStreamResult(fs, "application/vnd.ms-excel");
            fileStreamResult.FileDownloadName = "Customers List.xlsx";
            return fileStreamResult;
        }

        private string CreateHTMLTable(List<CustomerDto> customerList)
        {
            PropertyInfo[] allProperties = (new Customer()).GetType().GetProperties();
            int count = 1, iterCount = 2;
            string table="<table><thead><tr>";
            foreach (PropertyInfo property in allProperties)
            {
                table+="<th>"+ property.Name+"</th>";
                count++;
            }
            table+="</tr></thead><tbody>";
            foreach (CustomerDto customer in customerList)
            {
                table+="<tr>";
                count = 1;
                foreach (PropertyInfo property in allProperties)
                {
                    var obj = customer.GetType().GetProperty(property.Name);
                    if (obj != null)
                    {
                        table+="<td>"+ obj.GetValue(customer, null)+"</td>";
                        count++;
                    }
                }
                table+="</tr>";
                iterCount++;
            }
            table+="</tbody></table>";
            return table;
        }

        private string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
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
