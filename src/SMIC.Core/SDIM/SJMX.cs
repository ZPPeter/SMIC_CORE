using System;
using Abp.Domain.Entities;

namespace SMIC.SDIM
{
    public class SJMX : Entity<long>
    {
        //public int id { get; set; }         //ID - 继承自 Entity
        public string sjdid { get; set; }
        public string qjmc { get; set; }
        public string qjmcbm { get; set; }
        public DateTime djrq { get; set; }
        public string xhggmc { get; set; }     //型号规格 - xhgg
        public string xhggbm { get; set; }     //型号规格编码
        public string ccbh { get; set; }       //出厂编号   
        public string zsbh { get; set; }       //证书编号   
        public string jdzt { get; set; }       // 仪器检定状态100 222
        public string zzc { get; set; }        //制造厂
        //public string img { get; set; }        // 0leica 1topcon 2trimble 3sokkia 4south 5nikkom
        public DateTime yqjcrq { get; set; }   //要求检出日期 
        public string wtdw { get; set; }      //委托单位
        public DateTime jdrq { get; set; }    //jdrq - dbo.DPII_JDRQ
        public DateTime jwrq { get; set; }    //jwrq - dbo.DPII_JDRQ

        //public jbcs jbcs { get; set; }
        public string jdy { get; set; }       //检定员
        public string hyy { get; set; }       //核验员
        public string bzsm { get; set; }      //备注说明        
        public string wtdh { get; set; }      //委托单号
        public string wtdzt { get; set; }      //委托单状态
    }


    public class RecentSJMX : Entity<long>
    {
        public string 委托单号 { get; set; }
        public string 送检单位 { get; set; }
        public string 送检日期 { get; set; }
        public string 仪器件数 { get; set; }
        public string 检定状态 { get; set; }

    }

    public class SJMX_ZSBH : Entity<int>
    {
        public string ZSBH { get; set; }
    }

    public class ZSH_DATA : Entity<int>
    {
        public string Data { get; set; }
    }

    public class USER_NAME : Entity<long>
    {
        public string Surname { get; set; }
    }

    public class VW_CZRZ : Entity<int>
    {
        public int UserID { get; set; }
        public string Surname { get; set; }
        public string CZNR { get; set; }
        public DateTime CZSJ { get; set; }
        public string BZSM { get; set; }
    }
}