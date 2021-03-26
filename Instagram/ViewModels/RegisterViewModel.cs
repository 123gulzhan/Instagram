using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Instagram.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Instagram.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Логин вроде как нужен..")]
        [Display(Name = "Login")]
        public string Login { get; set; }
        
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        public string Avatar { get; set; }
        [NotMapped]
        [Required(ErrorMessage = "Фото тоже нужно..")]
        [Display(Name = "FormFile")]
        public IFormFile FormFile { get; set; }
        
        
        [Display(Name = "Name")]
        public string Name { get; set; }
        
        
        [Display(Name = "PhoneNumber")]
        public string PhoneNumber { get; set; }
        
        
        [Display(Name = "Description")]
        public string Description { get; set; }
        
        
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