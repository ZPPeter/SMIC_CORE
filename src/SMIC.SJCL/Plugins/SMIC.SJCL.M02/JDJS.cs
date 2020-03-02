using System;
using System.Collections.Generic;
using System.Text;

namespace SMIC.SJCL
{
    public class JDJS
    {
        public static double[] DataProcess(List<Double> JXZHS, List<Double> RawDatas)
        {
            /*
            List<Double> JXZHS = new List<Double>(); 
            JXZHS.Add(48.04679);
            JXZHS.Add(144.17496);
            JXZHS.Add(384.47354);
            JXZHS.Add(576.50659);
            JXZHS.Add(816.29461);
            JXZHS.Add(1080.64004);
            JXZHS.Add(96.12817);
            JXZHS.Add(336.42675);
            JXZHS.Add(528.4598);
            JXZHS.Add(768.24782);
            JXZHS.Add(1032.59325);
            JXZHS.Add(240.29858);
            JXZHS.Add(432.33163);
            JXZHS.Add(672.11965);
            JXZHS.Add(936.46508);
            JXZHS.Add(192.03305);
            JXZHS.Add(431.82107);
            JXZHS.Add(696.1665);
            JXZHS.Add(239.78802);
            JXZHS.Add(504.13345);
            JXZHS.Add(264.34543);
            */
            /*
            JXZHS.Add(48.002);
            JXZHS.Add(120.0126);
            JXZHS.Add(191.9977);
            JXZHS.Add(263.9794);
            JXZHS.Add(407.9775);
            JXZHS.Add(551.9996);
            JXZHS.Add(503.9976);
            JXZHS.Add(791.949);
            JXZHS.Add(359.9755);
            JXZHS.Add(215.9774);
            JXZHS.Add(143.9958);
            JXZHS.Add(72.0106);
            JXZHS.Add(71.9851);
            JXZHS.Add(143.9667);
            JXZHS.Add(431.987);
            JXZHS.Add(719.9384);
            JXZHS.Add(647.9533);
            JXZHS.Add(360.0018);
            JXZHS.Add(215.9798);
            JXZHS.Add(71.9816);
            JXZHS.Add(287.9649);            
            JXZHS.Add(288.0277);
            JXZHS.Add(575.9792);
            JXZHS.Add(720.0012);
            JXZHS.Add(863.9994);
            JXZHS.Add(935.981);
            JXZHS.Add(1007.9661);
            JXZHS.Add(1079.9767);
            JXZHS.Add(1127.9787);
            JXZHS.Add(839.951);
            JXZHS.Add(791.949);
            JXZHS.Add(719.9384);
            JXZHS.Add(647.9533);
            JXZHS.Add(575.9717);
            JXZHS.Add(431.9732);
            JXZHS.Add(287.9514);
            */
            return new JDJS().DoWork(JXZHS, RawDatas);
        }
        public Double[] DoWork(List<Double> JXZHS, List<Double> RawDatas)
        {
            var Xsum = 0.0;
            var X2sum = 0.0;
            var Ysum = 0.0;
            var XY = 0.0;
            var n = JXZHS.Count;
            Double[] diffValues = new Double[n];
            for (var i = 0; i < n; i++)
            {
                diffValues[i] = Math.Round((JXZHS[i] - RawDatas[i]) * 1000,1); //mm
                //Console.WriteLine(diffValues[i]);
                Xsum += JXZHS[i];
                Ysum += diffValues[i];
                XY += JXZHS[i] * diffValues[i];
                X2sum += JXZHS[i] * JXZHS[i];
            }

            var R = (Xsum * Ysum / n - XY) / (Xsum * Xsum / n - X2sum);
            var K = (Ysum - R * Xsum) / n;
            var jcs = K;
            var ccs = R / 1000;

            // 加乘常数改正 - 修正后的观测值
            // 规程验证不需要此改正，已经修正
            for (var j = 0; j < n; j++)
            {
                RawDatas[j] = RawDatas[j] + RawDatas[j] * ccs / 1000000 + jcs / 1000;
            }

            var dhe = 0.0;
            var dlhe = 0.0;
            var d2he = 0.0;
            var lhe = 0.0;

            for (var j = 0; j < n; j++)
            {
                dhe = dhe + RawDatas[j]; // 修改后
                dlhe = dlhe + RawDatas[j] * Math.Abs(JXZHS[j] - RawDatas[j]) * 1000;
                d2he = d2he + Math.Pow(RawDatas[j], 2);
                lhe = lhe + Math.Abs(JXZHS[j] - RawDatas[j]) * 1000;
            }

            var YQJDA = Math.Round(Math.Abs((dhe * dlhe - d2he * lhe) / (Math.Pow(dhe, 2) - n * d2he)),1);
            var YQJDB = Math.Round(Math.Abs((dhe * lhe - n * dlhe) / (Math.Pow(dhe, 2) - n * d2he) * 1000),1);
            Double[] ret = new Double[] { Math.Round(K, 1), Math.Round((R * 1000), 1), YQJDA, YQJDB };
            return ret;
        }
    }
}