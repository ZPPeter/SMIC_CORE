using System.ComponentModel.DataAnnotations;

namespace SMIC.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}