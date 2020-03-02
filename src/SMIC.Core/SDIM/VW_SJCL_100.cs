using System;
using Abp.Domain.Entities;

namespace SMIC.SDIM
{
    public class VW_SJCL_100 : Entity<long>
    {
        public int JDZT { get; set; }
        public string QJMCBM { get; set; } 
        public int JDYID { get; set; }
        public int HYYID { get; set; }
        public string HYY { get; set; }
        public int PZRID { get; set; }
        public string PZR { get; set; }
        public string sjdid { get; set; }
        public int xhggbm { get; set; }
        public DateTime djrq { get; set; }
        public DateTime? jdrq { get; set; }
        public DateTime yqjchrq { get; set; }
        
        public string Surname { get; set; }
        public string DWMC { get; set; }

        public string QJMC { get; set; }
        public string xhggmc { get; set; }
        public string ccbh { get; set; }
        public string ZZC { get; set; }
        public string bzsm { get; set; }
        public string hyyj { get; set; }
        public string pzyj { get; set; }

    }

}
