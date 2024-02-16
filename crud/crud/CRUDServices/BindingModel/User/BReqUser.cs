using System.ComponentModel.DataAnnotations;

namespace CRUDServices.BindingModel.User
{
    public class UserDto
    {
      
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }


    public class UserDtoList
    {
        [Required]
        public int page { get; set; } = 1;

        [Required]
        public int size { get; set; } = 1;
    }
}
