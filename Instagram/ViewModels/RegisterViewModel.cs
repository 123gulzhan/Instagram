using System;
using System.ComponentModel.DataAnnotations;
using Instagram.Enums;

namespace Instagram.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name = "Name")]
        public string Name { get; set; }
        
        [Required]
        [Display(Name = "Login")]
        public string Login { get; set; }
        
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Required]
        [Display(Name = "Avatar")]
        public string Avatar { get; set; }
        
        
        [Display(Name = "PhoneNumber")]
        public string PhoneNumber { get; set; }
        
        
        [Display(Name = "About")]
        public string About { get; set; }
        
        
        [Display(Name = "Sex")]
        public Sex Sex { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
        
        [Required]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        public string ConfirmPassword { get; set; }
    }
}