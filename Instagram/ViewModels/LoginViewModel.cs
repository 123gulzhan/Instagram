using System.ComponentModel.DataAnnotations;

namespace Instagram.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "LoginOrEmail")]
        public string LoginOrEmail { get; set; }
        
        
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
        
        [Display(Name = "Запомнить?")] 
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}