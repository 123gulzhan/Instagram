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
        [Required(ErrorMessage = "Заполните поле")]
        [RegularExpression(@"^([0-9a-zA-Z_.]){1,12}", ErrorMessage = "Логин должен содержать только буквы, цифры, точку и _")]
        [Display(Name = "Login")]
        [Remote("CheckLogin", "Validation", ErrorMessage = "Такой логин зарегистрирован")]
        public string Login { get; set; }
        
        [Required(ErrorMessage = "Заполните поле")]
        [Display(Name = "Email")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес эл.почты")]
        [Remote("CheckEmail", "Validation", ErrorMessage = "Такой email зарегистрирован")]
        public string Email { get; set; }
        
        public string Avatar { get; set; }
        [NotMapped]
        [Required(ErrorMessage = "Заполните поле")]
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
        
        [Required(ErrorMessage = "Заполните поле")]
        [DataType(DataType.Password)]
        [MinLength(5, ErrorMessage = "Минимальная длина 5 знаков")]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "Заполните поле")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        public string ConfirmPassword { get; set; }
    }
}