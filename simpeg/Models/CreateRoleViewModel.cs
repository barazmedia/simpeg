using System.ComponentModel.DataAnnotations;

namespace simpeg.Models
{
    public class CreateRoleViewModel
    {
        [Required]
        [Display(Name = "Role")]
        public string RoleName { get; set; }
    }
}
