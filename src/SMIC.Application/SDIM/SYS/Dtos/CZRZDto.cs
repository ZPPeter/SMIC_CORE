using System;
using System.Collections.Generic;
using System.Text;
using SMIC.SJCL;
using SMIC.Dtos;
using Abp.Runtime.Validation;

namespace SMIC.SDIM.Dtos
{
    public class CZRZDto
    {
        //public int UserId { get; set; }
        public string CZNR { get; set; }
        public string BZSM { get; set; }
    }

    public class GetCzrzInput : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 模糊搜索使用的关键字
        /// </summary>
        public string FilterText { get; set; }
        public bool isAdmin { get; set; }
        /// <summary>
        /// 正常化排序使用
        /// </summary>
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Id";
            }
            if (string.IsNullOrEmpty(Order))
            {
                Order = "asc";
            }
        }

    }


}
