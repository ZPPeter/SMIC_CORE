

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SMIC.SDIM;

namespace SMIC.SDIM.Dtos
{
    public class CreateOrUpdateHomeInfoInput
    {
        [Required]
        public HomeInfoDto HomeInfo { get; set; }

    }
}