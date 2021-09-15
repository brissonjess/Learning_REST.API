using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CourseLibrary.API.Services
{
    public class CourseLibraryRepository : ICourseLibraryRepository, IDisposable
    {
        private readonly CourseLibraryContext _context;

        public CourseLibraryRepository(CourseLibraryContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void AddCourse(Guid authorId, Course course)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            if (course == null)
            {
                throw new ArgumentNullException(nameof(course));
            }
            // always set the AuthorId to the passed-in authorId
            course.AuthorId = authorId;
            _context.Courses.Add(course);
        }

        public void DeleteCourse(Course course)
        {
            _context.Courses.Remove(course);
        }

        public Course GetCourse(Guid authorId, Guid courseId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            if (courseId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(courseId));
            }

            return _context.Courses
              .Where(c => c.AuthorId == authorId && c.Id == courseId).FirstOrDefault();
        }

        public IEnumerable<Course> GetCourses(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return _context.Courses
                        .Where(c => c.AuthorId == authorId)
                        .OrderBy(c => c.Title).ToList();
        }

        public void UpdateCourse(Course course)
        {
            // no code in this implementation
            //since there is no underlying logic behind the update
            //the conext in the _courserepository object takes care 
            //of committing the changes to the database
            //by keeping this method here it is easy to implment 
            //future changes to the update logic
        }

        public void AddAuthor(Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            // the repository fills the id (instead of using identity columns)
            author.Id = Guid.NewGuid();

            foreach (var course in author.Courses)
            {
                course.Id = Guid.NewGuid();
            }

            _context.Authors.Add(author);
        }

        public bool AuthorExists(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return _context.Authors.Any(a => a.Id == authorId);
        }

        public void DeleteAuthor(Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            _context.Authors.Remove(author);
        }

        public Author GetAuthor(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return _context.Authors.FirstOrDefault(a => a.Id == authorId);
        }

        public IEnumerable<Author> GetAuthors()
        {
            return _context.Authors.ToList<Author>();
        }
        public PagedList<Author> GetAuthors(AuthorsResourceParameters authorsResourceParameters)
        {
            if(authorsResourceParameters == null)
            {
                throw new Exception(nameof(authorsResourceParameters));
            }

            #region region: remove whitespace check 
            //now we will return a page of results, even if a main category or search category is not provided
            /*if (string.IsNullOrWhiteSpace(authorsResourceParameters.MainCategory) 
                && string.IsNullOrWhiteSpace(authorsResourceParameters.SearchQuery))
            {
                return GetAuthors();
            }
            */
            #endregion
            
            //we use iqueryable to create a collection object that can be filtered using lambda expressions
            var collection = _context.Authors as IQueryable<Author>;
            
            //by handling the query string behavior here we build the db select statement before hitting the database, improving performance and reducing the amount of resources that are held in memory
            if (!string.IsNullOrWhiteSpace(authorsResourceParameters.MainCategory))
            {
                var mainCategory = authorsResourceParameters.MainCategory.Trim();
                collection = collection.Where(x => x.MainCategory == mainCategory);
            }

            if (!string.IsNullOrWhiteSpace(authorsResourceParameters.SearchQuery))
            {
                var searchQuery = authorsResourceParameters.SearchQuery.Trim();
                collection = collection.Where(y => y.MainCategory.Contains(searchQuery)
                || y.FirstName.Contains(searchQuery)
                || y.LastName.Contains(searchQuery));
            }

            if (!string.IsNullOrWhiteSpace(authorsResourceParameters.OrderBy))
            {
                if(authorsResourceParameters.OrderBy.ToLowerInvariant() == "name")
                {
                    collection = collection.OrderBy(a => a.FirstName).ThenBy(a => a.LastName);
                }

                //collection.ApplySort(authorsResourceParameters.OrderBy, _mappingDictionary);

            }
            //by the time the code reaches here all the business logic has been applied and the return will simply list the results
            return PagedList<Author>.Create(collection, authorsResourceParameters.PageNumber, authorsResourceParameters.PageSize);
        }

        public IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds)
        {
            if (authorIds == null)
            {
                throw new ArgumentNullException(nameof(authorIds));
            }

            return _context.Authors.Where(a => authorIds.Contains(a.Id))
                .OrderBy(a => a.FirstName)
                .OrderBy(a => a.LastName)
                .ToList();
        }

        public void UpdateAuthor(Author author)
        {
            // no code in this implementation
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose resources when needed
            }
        }
    }
}
