using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.DTO.Authentication
{
    public class AuthenticateDTO
    {
        [Required(ErrorMessage = "Ошибка :c")]
        public string FingerPrint { get; set; }
        
        [Required(ErrorMessage = "Вас нужно как-то назвать")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Серверу не понравилась длинна имени")]
        public string Name { get; set; }
    }
}
