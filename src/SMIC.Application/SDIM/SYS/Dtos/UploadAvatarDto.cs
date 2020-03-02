using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;

namespace SMIC.Common.Dto
{
    public class UploadAvatarDto
    {
        [Required]
        public long UserId { get; set; }
        [Required]
        public IFormFile File { get; set; }

    }
}