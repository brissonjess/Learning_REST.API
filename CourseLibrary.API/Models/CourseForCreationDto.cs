using CourseLibrary.API.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Models
{
    public class CourseForCreationDto : CourseForManipulationDto
    { }


    #region Region -The original code before creating the CourseforManipulationDto base class
    /*--- 
     * added the base class to reduce the amount of duplicated code related to handling title and description response validation


    //the property validations will still be the first to display to the user when implementing custom attribute annotationss
    //custom attributes are executed before the ivalidate method is called and this comes in handy for property level validation 
    //yet at class level this same rules still apply: if property-level validation fails then class level validation will not occur even when using custom attributes
    [CourseTitleMustBeDifferentFromDescription(ErrorMessage ="Title must be ddifferent from description.")]//<-new validation attribute takes over from IValidationObject
    public class CourseForCreationDto //: IValidatableObject
    {
        [Required(ErrorMessage ="You should fill out a title.")]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(1500)]
        public string Description { get; set; }

        //the IValidatableObject and the following code is one way to handle input validation
        //however, if the rules defined in the property annotations (ie required and max length) return an error first then this code will not be immediately displayed to the user
        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (Title == Description)
        //    {
        //        yield return new ValidationResult("The provided description should be different from the title.", new[] { "CourseForCreationDto" });
        //    }
        //}


    }
    */
    #endregion

}
