using System;
using Abp.Domain.Entities;

namespace SMIC.SDIM
{
    public class WTD : Entity<long>
    {
        //public int id { get; set; }         //送检单ID - 继承自 Entity
        public string sjdid { get; set; }     //委托单号 - wtdh
        public string dwmc { get; set; }      //送检单位 - wtdw
        public DateTime sjrq { get; set; }    //送检日期 - wtrq
        
        public int yqjs { get; set; }         //仪器件数

        //public int q z yjs { get; set; }         //全站仪件数

        public string jdzt { get; set; }      //检定状态 - 登记 正在检定 检完
        //public string q z yjdzt { get; set; }   //全站仪检定状态
        public DateTime yqjcrq { get; set; }  //无 - 默认10个工作日，AddDays(14)       
                                              //public bool ssjj { get; set; }        //无 - 是否加急
                                              //public List<sjmx> sjmx { get; set; }  //送检明细
    }
}
