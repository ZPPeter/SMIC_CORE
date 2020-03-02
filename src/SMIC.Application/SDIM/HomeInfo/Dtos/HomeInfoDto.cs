
using System;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities.Auditing;
using Abp.AutoMapper;
using Abp.Runtime.Validation;
using SMIC.SDIM;

namespace  SMIC.SDIM.Dtos
{
    [AutoMapTo(typeof(HomeInfo))]
    public class HomeInfoDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public int? Id { get; set; }         
                       
		/// <summary>
		/// title
		/// </summary>
		public string title { get; set; }

		/// <summary>
		/// description
		/// </summary>
		[MaxLength(128, ErrorMessage="信息内容超出最大长度")]
		[Required(ErrorMessage="信息内容不能为空")]
		public string description { get; set; }

        //public DateTime? CreationTime { get; set; }

    }
}