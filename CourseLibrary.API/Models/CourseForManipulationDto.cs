using CourseLibrary.API.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.API.Models
{
    [CourseTitleMustBeDifferentFromDescription(ErrorMessage = "Title must be different from description.")]
    //an abstract modifier in a class declaration means that this class is only meant to be used as a base class 
    //it cannot be used on its own - it can only be derived from other classes
    public abstract class CourseForManipulationDto
    {
        [Required(ErrorMessage = "You should fill out a title.")]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(1500, ErrorMessage = "The description shouldn't have more than 1,500 characters.")]
        public virtual string Description { get; set; } //virtual is used when we have implementation on the base class but we want to allow overriding 
        //the overriding logic is in the courseForUpdateDto
    }
}
