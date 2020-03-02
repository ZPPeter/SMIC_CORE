using System;
using System.IO;
using System.Reflection;

namespace SMIC.SJCL
{
    public enum ETemperature
    {
        InOutRoom, // 室内外
        InRoom,    // 室内
        OutRoom    // 室外 
    }

    public interface ITemperature
    {
        /// <summary>
        /// -1 室内，0 室内外，1 室外
        /// </summary>
        /// <param name="BZ"></param>
        /// <returns></returns>
        string GetTemperature(ETemperature e);
    }

    public interface IXlsMaker
    {
        //string LoadXls(JDJLFM jdjlfm);
        //string MakeXls(JDJLFM jdjlfm);
        string MakeXls(RawTemplate template, JDJLFM jdjlfm, int[] Signer);
    }


    public class GPSRES
    {
        public string C12 { get; set; }
        public string C19 { get; set; }
    }

    /// <summary>
    /// 检定结果接口
    /// </summary>
    public class RESA17
    {
        public string A01 { get; set; }
        public string A02 { get; set; }
        public string A03 { get; set; }
        public string A04 { get; set; }
        public string A05 { get; set; }
        public string A06 { get; set; }
        public string A07 { get; set; }
        public string A08 { get; set; }
        public string A09 { get; set; }
        public string A10 { get; set; }
        public string A11 { get; set; }
        public string A12 { get; set; }
        public string A13 { get; set; }
        public string A14 { get; set; }
        public string A15 { get; set; }
        public string A16 { get; set; }
        public string A17 { get; set; }
    }

    /// <summary>
    /// 检定结果接口
    /// </summary>
    public class RESULT
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class FindResult
    {
        public RESULT FindByName(RESULT[] res, string Name)
        {
            for (int i = 0; i < res.Length; i++)
            {
                if (res[i].Name == Name)
                {
                    return res[i];
                }
            }
            return null;
        }
    }


    // 检定记录封面
    public class JDJLFM
    {
        public int ID { get; set; }
        public string DWMC { get; set; }
        public string DWDZ { get; set; }
        public string QJMC { get; set; }
        public string XHGG { get; set; }
        public string ZZC { get; set; }
        public string CCBH { get; set; }
        public string ZSBH { get; set; }
        public DateTime JDRQ { get; set; }
        /// <summary>
        /// 检校温度
        /// </summary>
        public string JJWD { get; set; }
        public string CJJD { get; set; } // CCJD 测角精度
        public string QT01 { get; set; } // 电子经纬仪、全站仪补偿范围
        public string QT02 { get; set; } // 全站仪 BCJDA
        public string QT03 { get; set; } // 全站仪 BCJDB
        public string QT04 { get; set; } // 全站仪 Axles
        public string QT05 { get; set; } // 全站仪 气压
        //public string JDY  { get; set; } // 检定
        //public string HYY  { get; set; } // 核验
        //public string PZR  { get; set; } // 批准
    }
        
    public class RawTemplate
    {
        public string QJMCBM { get; set; }   // 1030
        public string QJMC { get; set; }     // GPS接收机
        public string MBMC { get; set; }     // MB01        
    }
}