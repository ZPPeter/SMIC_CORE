using System;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;


namespace SMIC.SDIM.Dtos
{
    public class SJMXListDto : EntityDto<int>
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime? CreationTime { get; set; }
    }

    public class UpdateCcbhDto
    {
        [Required]
        public string ID { get; set; }

        [Required]
        public string Ccbh { get; set; }

        public string OCcbh { get; set; }

        public string BZSM { get; set; }

    }

    public class AddJbcsDto
    {
        [Required]
        public string ID { get; set; } // XHGGBM
        [Required]
        public double BCJDA { get; set; }
        [Required]
        public double BCJDB { get; set; }
        [Required]
        public double CJJD { get; set; }
        [Required]
        public double BCFW { get; set; }
        [Required]
        public int Axles { get; set; }

    }

}


