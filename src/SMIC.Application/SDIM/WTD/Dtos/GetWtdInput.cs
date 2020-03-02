using System;
using Abp.Runtime.Validation;
using SMIC.Dtos;

namespace SMIC.SDIM.Dtos
{
    public class GetWtdInput : PagedAndSortedInputDto, IShouldNormalize
    {
        ////BCC/ BEGIN CUSTOM CODE SECTION
        ////ECC/ END CUSTOM CODE SECTION
        /// <summary>
        /// 模糊搜索使用的关键字
        /// </summary>
        public string WTDH { get; set; }
        public string WTDW { get; set; }
        public string FilterText { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

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
