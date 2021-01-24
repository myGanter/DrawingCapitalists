using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Core.Expansions;

namespace Core.Models.Hubs
{
    public class CreateLobbyInfo : IValidatableObject
    {
        [Required(ErrorMessage = "Лобби нужно как-то назвать")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Серверу не понравилась длинна имени")]
        public string Name { get; set; }

        public bool IsPrivate { get; set; }

        public string Password { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();

            if (IsPrivate && (Password.IsNullOrEmpty() || Password.Length < 3 || Password.Length > 20))
                errors.Add(new ValidationResult("Пароль должен состоять минимум из 3 символов"));

            return errors;
        }
    }
}
