using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Instagram.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Заполните поле")]
        [Display(Name = "LoginOrEmail")]
        [Remote("CheckLoginOrEmail", "Validation", ErrorMessage = "Такой логин(email) не зарегистрирован")]
        public string LoginOrEmail { get; set; }
        
        
        [Required(ErrorMessage = "Заполните поле")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
        
        [Display(Name = "Запомнить?")] 
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}