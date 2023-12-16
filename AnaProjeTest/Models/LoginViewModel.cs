using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AnaProjeTest.Models
{
	public class LoginViewModel
	{
        [Required(ErrorMessage = "Kullanıcı adı boş bırakılamaz.")]
        [StringLength(30,ErrorMessage ="Bu alan 30 karakterden fazla girilemez.")]
        public string UserName { get; set; }
		[Required(ErrorMessage ="Şifre boş bırakılamaz")]
		[MinLength(6, ErrorMessage = "En az 6 karakter girmelisiniz.")]
		[MaxLength(16, ErrorMessage = "En fazla 16 karakter girebilirsiniz.")]
		public string Password { get; set; }
    }
}
