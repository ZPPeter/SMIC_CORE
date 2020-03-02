using System;
using Abp.Domain.Entities;

namespace SMIC.SDIM
{
    public class VW_SJMX : Entity<long>
    {
        public double 送检单ID { get; set; }
        public string 送检单号 { get; set; }
        public string 单位名称 { get; set; }
        public DateTime 送检日期 { get; set; }
        public string 器具名称 { get; set; }
        public string 型号规格 { get; set; }
        public string 出厂编号 { get; set; }
        public string 证书编号 { get; set; }
        public string 检定状态 { get; set; }
    }
}
