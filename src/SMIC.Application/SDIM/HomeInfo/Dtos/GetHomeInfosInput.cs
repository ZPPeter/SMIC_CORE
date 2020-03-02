
using Abp.Runtime.Validation;

using System;
using SMIC.Dtos;
namespace SMIC.SDIM.Dtos
{
    public class GetHomeInfosInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        /// <summary>
        /// 正常化排序使用
        /// </summary>
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Id";
            }
        }
        public DateTime? LastDate { get; set; }
    }
}
