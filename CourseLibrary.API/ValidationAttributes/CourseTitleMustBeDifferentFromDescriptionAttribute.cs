using CourseLibrary.API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.ValidationAttributes
{
    public class CourseTitleMustBeDifferentFromDescriptionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var course = (CourseForManipulationDto)validationContext.ObjectInstance;
            if (course.Title == course.Description)
            {       
                //use when there will be a customer error message passed through the dto
                return new ValidationResult(ErrorMessage, new[] { nameof(CourseForManipulationDto) });
            }
            return ValidationResult.Success;
        }

        #region Region -The original code before creating the CourseforManipulationDto base class 
        //protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        //{
        //    var course = (CourseForCreationDto)validationContext.ObjectInstance;
        //    if (course.Title == course.Description)
        //    {
        //        /* -- use when no custom attribute is passed in through the dto --
        //        return new ValidationResult("The provided description should be different from the title",new[] { nameof(CourseForCreationDto) });
        //        */

        //        //use when there will be a customer error message passed through the dto
        //        return new ValidationResult(ErrorMessage, new[] { nameof(CourseForCreationDto) });
        //    }
        //    return ValidationResult.Success;
        //}
        #endregion
    }
}
