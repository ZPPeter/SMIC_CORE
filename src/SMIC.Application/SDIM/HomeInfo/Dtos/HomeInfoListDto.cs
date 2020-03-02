

using System;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;


namespace SMIC.SDIM.Dtos
{
    public class HomeInfoListDto : EntityDto<int> 
    {
        public string User { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime? CreationTime { get; set; }
    }


}