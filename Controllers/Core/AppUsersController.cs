using ESPL.NG.Models;
using ESPL.NG.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESPL.NG.Helpers;
using AutoMapper;
using ESPL.NG.Entities;
using Microsoft.AspNetCore.Http;
using ESPL.NG.Models.Core;
using ESPL.NG.Helpers.Core;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ESPL.NG.Controllers.Core
{
    [Route("api/appusers")]
    [Authorize(Policy = "IsSuperAdmin")]
    public class AppUserController : Controller
    {
        // private IAppRepository _appRepository;
        // private IUrlHelper _urlHelper;
        // private IPropertyMappingService _propertyMappingService;
        // private ITypeHelperService _typeHelperService;
        // // private RoleManager<IdentityRole> _roleMgr;
        // // private UserManager<AppUser> _userMgr;


        // public AppUserController(IAppRepository appRepository,
        //     IUrlHelper urlHelper,
        //     IPropertyMappingService propertyMappingService,
        //     ITypeHelperService typeHelperService)
        //     // UserManager<AppUser> userMgr,
        //     // RoleManager<IdentityRole> roleMgr)
        // {
        //     _appRepository = appRepository;
        //     _urlHelper = urlHelper;
        //     _propertyMappingService = propertyMappingService;
        //     _typeHelperService = typeHelperService;
        //     // _userMgr = userMgr;
        //     // _roleMgr = roleMgr;
        // }

        // [HttpGet(Name = "GetAppUsers")]
        // public IActionResult GetAppUsers(AppUsersResourceParameters esplUsersResourceParameters,
        //     [FromHeader(Name = "Accept")] string mediaType)
        // {
        //     if (!_propertyMappingService.ValidMappingExistsFor<AppUserDto, AppUser>
        //        (esplUsersResourceParameters.OrderBy))
        //     {
        //         return BadRequest();
        //     }

        //     if (!_typeHelperService.TypeHasProperties<AppUserDto>
        //         (esplUsersResourceParameters.Fields))
        //     {
        //         return BadRequest();
        //     }

        //     var esplUsersFromRepo = _appRepository.GetAppUsers(esplUsersResourceParameters);

        //     var esplUsers = new List<AppUserDto>();
        //     esplUsersFromRepo.ForEach(esplUser =>
        //     {
        //         esplUsers.Add(
        //         new AppUserDto()
        //         {
        //             Id = esplUser.UserId,
        //             FirstName = esplUser.FirstName,
        //             LastName = esplUser.LastName,
        //             UserName = esplUser.UserName
        //         });
        //     });

        //     if (mediaType == "application/vnd.marvin.hateoas+json")
        //     {
        //         var paginationMetadata = new
        //         {
        //             totalCount = esplUsersFromRepo.TotalCount,
        //             pageSize = esplUsersFromRepo.PageSize,
        //             currentPage = esplUsersFromRepo.CurrentPage,
        //             totalPages = esplUsersFromRepo.TotalPages,
        //         };

        //         Response.Headers.Add("X-Pagination",
        //             Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));
        //         Response.Headers.Add("Access-Control-Expose-Headers", "X-Pagination");

        //         var links = CreateLinksForAppUsers(esplUsersResourceParameters,
        //             esplUsersFromRepo.HasNext, esplUsersFromRepo.HasPrevious);

        //         var shapedAppUsers = esplUsers.ShapeData(esplUsersResourceParameters.Fields);
        //         var linkedCollectionResource = new
        //         {
        //             value = shapedAppUsers,
        //             links = links
        //         };

        //         return Ok(linkedCollectionResource);
        //     }
        //     else
        //     {
        //         var previousPageLink = esplUsersFromRepo.HasPrevious ?
        //             CreateAppUsersResourceUri(esplUsersResourceParameters,
        //             ResourceUriType.PreviousPage) : null;

        //         var nextPageLink = esplUsersFromRepo.HasNext ?
        //             CreateAppUsersResourceUri(esplUsersResourceParameters,
        //             ResourceUriType.NextPage) : null;

        //         var paginationMetadata = new
        //         {
        //             previousPageLink = previousPageLink,
        //             nextPageLink = nextPageLink,
        //             totalCount = esplUsersFromRepo.TotalCount,
        //             pageSize = esplUsersFromRepo.PageSize,
        //             currentPage = esplUsersFromRepo.CurrentPage,
        //             totalPages = esplUsersFromRepo.TotalPages
        //         };

        //         Response.Headers.Add("X-Pagination",
        //             Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));
        //         Response.Headers.Add("Access-Control-Expose-Headers", "X-Pagination");
                
        //         return Ok(esplUsers);
        //     }
        // }

        // private string CreateAppUsersResourceUri(
        //     AppUsersResourceParameters esplUsersResourceParameters,
        //     ResourceUriType type)
        // {
        //     switch (type)
        //     {
        //         case ResourceUriType.PreviousPage:
        //             return _urlHelper.Link("GetAppUsers",
        //               new
        //               {
        //                   fields = esplUsersResourceParameters.Fields,
        //                   orderBy = esplUsersResourceParameters.OrderBy,
        //                   searchQuery = esplUsersResourceParameters.SearchQuery,
        //                   pageNumber = esplUsersResourceParameters.PageNumber - 1,
        //                   pageSize = esplUsersResourceParameters.PageSize
        //               });
        //         case ResourceUriType.NextPage:
        //             return _urlHelper.Link("GetAppUsers",
        //               new
        //               {
        //                   fields = esplUsersResourceParameters.Fields,
        //                   orderBy = esplUsersResourceParameters.OrderBy,
        //                   searchQuery = esplUsersResourceParameters.SearchQuery,
        //                   pageNumber = esplUsersResourceParameters.PageNumber + 1,
        //                   pageSize = esplUsersResourceParameters.PageSize
        //               });
        //         case ResourceUriType.Current:
        //         default:
        //             return _urlHelper.Link("GetAppUsers",
        //             new
        //             {
        //                 fields = esplUsersResourceParameters.Fields,
        //                 orderBy = esplUsersResourceParameters.OrderBy,
        //                 searchQuery = esplUsersResourceParameters.SearchQuery,
        //                 pageNumber = esplUsersResourceParameters.PageNumber,
        //                 pageSize = esplUsersResourceParameters.PageSize
        //             });
        //     }
        // }

        // [HttpGet("{id}", Name = "GetAppUser")]
        // public IActionResult GetAppUser(Guid id, [FromQuery] string fields)
        // {
        //     if (!_typeHelperService.TypeHasProperties<AppUserDto>
        //       (fields))
        //     {
        //         return BadRequest();
        //     }

        //     var esplUserFromRepo = _appRepository.GetAppUser(id);

        //     if (esplUserFromRepo == null)
        //     {
        //         return NotFound();
        //     }

        //     var esplUser = Mapper.Map<AppUserDto>(esplUserFromRepo);

        //     var links = CreateLinksForAppUser(id, fields);

        //     var linkedResourceToReturn = esplUser.ShapeData(fields)
        //         as IDictionary<string, object>;

        //     linkedResourceToReturn.Add("links", links);

        //     return Ok(linkedResourceToReturn);
        // }

        // [HttpPost(Name = "CreateAppUser")]
        // public IActionResult CreateAppUser([FromBody] AppUserForCreationDto esplUser)
        // {
        //     if (esplUser == null)
        //     {
        //         return BadRequest();
        //     }

        //     var esplUserEntity = Mapper.Map<AppUser>(esplUser);

        //     _appRepository.AddAppUser(esplUserEntity);

        //     if (!_appRepository.Save())
        //     {
        //         throw new Exception("Creating an esplUser failed on save.");
        //         // return StatusCode(500, "A problem happened with handling your request.");
        //     }

        //     var esplUserToReturn = Mapper.Map<AppUserDto>(esplUserEntity);

        //     var links = CreateLinksForAppUser(esplUserToReturn.Id, null);

        //     var linkedResourceToReturn = esplUserToReturn.ShapeData(null)
        //         as IDictionary<string, object>;

        //     linkedResourceToReturn.Add("links", links);

        //     return CreatedAtRoute("GetAppUser",
        //         new { id = linkedResourceToReturn["Id"] },
        //         linkedResourceToReturn);
        // }


        // [HttpPost("{id}")]
        // public IActionResult BlockAppUserCreation(Guid id)
        // {
        //     if (_appRepository.AppUserExists(id))
        //     {
        //         return new StatusCodeResult(StatusCodes.Status409Conflict);
        //     }

        //     return NotFound();
        // }

        // [HttpDelete("{id}", Name = "DeleteAppUser")]
        // public IActionResult DeleteAppUser(Guid id)
        // {
        //     var esplUserFromRepo = _appRepository.GetAppUser(id);
        //     if (esplUserFromRepo == null)
        //     {
        //         return NotFound();
        //     }

        //     _appRepository.DeleteAppUser(esplUserFromRepo);

        //     if (!_appRepository.Save())
        //     {
        //         throw new Exception($"Deleting esplUser {id} failed on save.");
        //     }

        //     return NoContent();
        // }

        // private IEnumerable<LinkDto> CreateLinksForAppUser(Guid id, string fields)
        // {
        //     var links = new List<LinkDto>();

        //     if (string.IsNullOrWhiteSpace(fields))
        //     {
        //         links.Add(
        //           new LinkDto(_urlHelper.Link("GetAppUser", new { id = id }),
        //           "self",
        //           "GET"));
        //     }
        //     else
        //     {
        //         links.Add(
        //           new LinkDto(_urlHelper.Link("GetAppUser", new { id = id, fields = fields }),
        //           "self",
        //           "GET"));
        //     }

        //     links.Add(
        //       new LinkDto(_urlHelper.Link("DeleteAppUser", new { id = id }),
        //       "delete_esplUser",
        //       "DELETE"));

        //     links.Add(
        //       new LinkDto(_urlHelper.Link("CreateBookForAppUser", new { esplUserId = id }),
        //       "create_book_for_esplUser",
        //       "POST"));

        //     links.Add(
        //        new LinkDto(_urlHelper.Link("GetBooksForAppUser", new { esplUserId = id }),
        //        "books",
        //        "GET"));

        //     return links;
        // }

        // private IEnumerable<LinkDto> CreateLinksForAppUsers(
        //     AppUsersResourceParameters esplUsersResourceParameters,
        //     bool hasNext, bool hasPrevious)
        // {
        //     var links = new List<LinkDto>();

        //     // self 
        //     links.Add(
        //        new LinkDto(CreateAppUsersResourceUri(esplUsersResourceParameters,
        //        ResourceUriType.Current)
        //        , "self", "GET"));

        //     if (hasNext)
        //     {
        //         links.Add(
        //           new LinkDto(CreateAppUsersResourceUri(esplUsersResourceParameters,
        //           ResourceUriType.NextPage),
        //           "nextPage", "GET"));
        //     }

        //     if (hasPrevious)
        //     {
        //         links.Add(
        //             new LinkDto(CreateAppUsersResourceUri(esplUsersResourceParameters,
        //             ResourceUriType.PreviousPage),
        //             "previousPage", "GET"));
        //     }

        //     return links;
        // }

        // [HttpOptions]
        // public IActionResult GetAppUsersOptions()
        // {
        //     Response.Headers.Add("Allow", "GET,OPTIONS,POST");
        //     return Ok();
        // }
    }
}