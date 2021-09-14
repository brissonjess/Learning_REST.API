using AutoMapper;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Models;
using CourseLibrary.API.ResourceParameters;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;
        public AuthorsController(ICourseLibraryRepository courseLibraryRespoitory, IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRespoitory ?? throw new ArgumentNullException(nameof(courseLibraryRespoitory));
            _mapper = mapper ?? throw new ArgumentException(nameof(mapper));
        }

        [HttpGet(Name = "GetAuthors")]
        [HttpHead]
        //public ActionResult<IEnumerable<AuthorDto>> GetAuthors([FromQuery(Name ="")] string mainCategory) // Add [FromQuery(Name ="")] to method signature to assign an attribute to the param for readability. the Name="" portion is how you assign the db column name if it doesn't match the col value that is stored in your database.
        public ActionResult<IEnumerable<AuthorDto>> GetAuthors([FromQuery] AuthorsResourceParameters authorsResourceParameters)
        {
            var authorsFromRepo = _courseLibraryRepository.GetAuthors(authorsResourceParameters);
            #region region: how to map values without using AutoMapper library
            //throw new Exception("Testexception");//this line tests exception handling messages; need development environment to prod to see full effect

            /*--------mapping resources using a regular foreach loop
             * okay if you dont have a lot of child collections, and other complex data structures
            var authors = new List<AuthorDto>();
            foreach (var author in authorsFromRepo)
            {
                authors.Add(new AuthorDto()
                {
                    Id = author.Id,
                    Name = $"{author.FirstName} {author.LastName}",
                    MainCategory = author.MainCategory,
                    DateOfBirth = author.DateOfBirth,
                    Age = author.DateOfBirth.GetCurrentAge()
                });
            }
            return Ok(authors);
            */

            /*-------mapping resources using automapper
             a profile needs to be created (see profile folder) to map the entity*/
            #endregion

            //API Pagination: check if there is a previous or next link
            var previousPageLink = authorsFromRepo.HasPrevious ? CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.PreviousPage) : null;
            var nextPageLink = authorsFromRepo.HasNext ? CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.NextPage) : null;

            ///Pagination: Create metadata by init a new object with all the potential values
            var paginationMetaData = new
            {
                totalCount = authorsFromRepo.TotalCount,
                pageSize = authorsFromRepo.PageSize,
                currentPage = authorsFromRepo.CurrentPage,
                totalPages = authorsFromRepo.TotalPages,
                previousPageLink,
                nextPageLink
            };
            ///Pagination: add the metadata as a custom header in the response
            ///Second parameter is the value, and in this ccase we formatted to Json. It can be any format.
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetaData));

            return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo));
        }

        [HttpGet("{authorId}", Name = "GetAuthor")]
        public IActionResult GetAuthor(Guid authorId)
        {
            var authorFromRepo = _courseLibraryRepository.GetAuthor(authorId);
            if (authorFromRepo == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<AuthorDto>(authorFromRepo));
        }

        [HttpPost]
        public ActionResult<AuthorDto> CreateAuthor(AuthorForCreationDto author)
        {
            var authorEntity = _mapper.Map<Entities.Author>(author);
            _courseLibraryRepository.AddAuthor(authorEntity);
            _courseLibraryRepository.Save();

            var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);
            return CreatedAtRoute("GetAuthor", new { authorId = authorToReturn.Id }, authorToReturn);
        }

        [HttpOptions]
        public IActionResult GetAuthorsOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok("You can write a message in the response body heres");
        }

        [HttpDelete("{authorId}")]
        public ActionResult DeleteAuthor(Guid authorId)
        {
            var authorFromRepo = _courseLibraryRepository.GetAuthor(authorId);
            if(authorFromRepo == null)
            {
                return NotFound();
            }
            //cascade delete is on by default, so any authors that are deleted will also delete their courses
            _courseLibraryRepository.DeleteAuthor(authorFromRepo);
            _courseLibraryRepository.Save();
            return NoContent();
        }

        private string CreateAuthorsResourceUri(AuthorsResourceParameters authorsResourceParameters, ResourceUriType type)
        {
            ///Use a switch on the ResourceUriType to set to the previous or next page
            ///and pass in the Url.Link using the Route Name and a set of values
            ///these values are the query string parameters (like pageNumber, pageSize, etc.)
            ///Immportant: pass in any other potential query string values, otherwise the
            ///paging will not display the correct set of data
            ///Default: we return the current page link
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetAuthors",
                        new { 
                            pageNumber = authorsResourceParameters.PageNumber -1,
                            pageSize = authorsResourceParameters.PageSize, 
                            mainCategory = authorsResourceParameters.MainCategory,
                            searchQuery = authorsResourceParameters.SearchQuery
                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetAuthors", new {
                        pageNumber = authorsResourceParameters.PageNumber + 1,
                        pageSize = authorsResourceParameters.PageSize,
                        mainCategory = authorsResourceParameters.MainCategory,
                        searchQuery = authorsResourceParameters.SearchQuery
                    });
                default:
                    return Url.Link("GetAuthors",
                        new {
                            pageNumber = authorsResourceParameters.PageNumber,
                            pageSize = authorsResourceParameters.PageSize,
                            mainCategory = authorsResourceParameters.MainCategory,
                            searchQuery = authorsResourceParameters.SearchQuery
                        });
            }
        }
    }
}
