using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace GrupaEka.Models
{

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Obecne hasło")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} musi mieć dułogość conajmniej {2} znaków.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nowe hasło")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź nowe hasło")]
        [Compare("NewPassword", ErrorMessage = "Nowe hasło i potwierdzenie hasła nie pasują do siebie.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangeEmailModel
    {
        [Required(ErrorMessage="Podaj nowy adres e-mail.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Nowy adres e-mail")]
        [RegularExpression("^[0-9a-z_.-]+@([0-9a-z-]+\\.)+[a-z]{2,6}$", ErrorMessage="Podaj poprawny adres e-mail.")]
        public string NewEmail { get; set; }

        [Required(ErrorMessage = "Potwierdź nowy adres e-mail.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Potwierdź nowy adres e-mail")]
        [Compare("NewEmail", ErrorMessage = "Nowy e-mail i potwierdzenie nie pasują do siebie.")]
        [RegularExpression("^[0-9a-z_.-]+@([0-9a-z-]+\\.)+[a-z]{2,6}$", ErrorMessage="Podaj poprawny adres e-mail.")]
        public string ConfirmEmail { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [Display(Name = "Użytkownik")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [Display(Name = "Pamiętaj?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} musi mieć dułogość conajmniej {2} znaków.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź hasło")]
        [Compare("Password", ErrorMessage = "Nowe hasło i potwierdzenie hasła nie pasują do siebie.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Profil")]
        public Profile Profile { get; set; }
    }
}
