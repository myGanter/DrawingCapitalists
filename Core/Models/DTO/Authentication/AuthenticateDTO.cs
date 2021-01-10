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
        public string Name { get; set; }
    }
}
