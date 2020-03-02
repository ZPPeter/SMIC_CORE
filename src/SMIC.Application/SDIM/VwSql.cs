using System;
using System.Collections.Generic;
using System.Text;

namespace SMIC.SDIM
{
    public class VwSql
    {
        public static string Vw_SJMX = @"
SELECT dbo.YQSF_SJMX.ID, dbo.YQSF_SJD.sjdid AS 送检单号, 
dbo.YQSF_KH.dwmc AS 单位名称, CONVERT(varchar(100), 
dbo.YQSF_SJD.sjrq, 23) AS 送检日期, 
dbo.JCXX_QJMC_BM.QJMC AS 器具名称,
dbo.JCXX_XHGG_BM.XHGGMC AS 型号规格, 
dbo.YQSF_SJMX.ccbh AS 出厂编号, 
dbo.YQSF_SJMX.zsbh AS 证书编号, 
dbo.YQSF_SJMX.jdzt AS 检定状态,
dbo.JCXX_XHGG_BM.QJMCBM
FROM dbo.YQSF_SJMX LEFT JOIN dbo.YQSF_SJD ON dbo.YQSF_SJD.ID = dbo.YQSF_SJMX.sjdid 
LEFT JOIN  dbo.YQSF_KH ON dbo.YQSF_SJD.khid = dbo.YQSF_KH.khid 
LEFT JOIN  dbo.JCXX_XHGG_BM ON dbo.YQSF_SJMX.XHGGBM = dbo.JCXX_XHGG_BM.XHGGBM 
LEFT JOIN  dbo.JCXX_QJMC_BM ON dbo.JCXX_XHGG_BM.QJMCBM = dbo.JCXX_QJMC_BM.QJMCBM
";

        public static string Vw_DPII_SJD = @"
SELECT a.ID, a.sjdid, f.dwmc, a.sjrq, a.qzyjs, a.qzyjdzt
FROM dbo.YQSF_SJD AS a LEFT JOIN dbo.YQSF_KH AS f ON f.khid = a.khid
where a.sjrq>'2019-04-25' and a.jdzt <>'检完' and a.qzyjs>0
ORDER BY a.ID DESC";

        public static string Vw_DPII_SJMX = @"
SELECT a.ID, a.sjdid as 委托单号,e.sjdid as 委托单号码,f.dwmc as 委托单位,e.djrq as 登记日期,d.QJMC AS 器具名称, b.XHGGMC AS 型号规格, 
a.ccbh AS 出厂编号,c.ZZCNR as 制造厂,b.XHGGBM,g.jdrq,g.jwrq, g.jdzt,a.bzsm,e.sjdid as WTDH,a.jdzt as 检定状态,g.jdy
FROM dbo.YQSF_SJMX AS a LEFT JOIN
dbo.YQSF_SJD as e on a.sjdid = e.id LEFT JOIN
dbo.YQSF_KH as f on e.khid = f.khid LEFT JOIN
dbo.JCXX_XHGG_BM AS b ON a.XHGGBM = b.XHGGBM LEFT JOIN
dbo.JCXX_ZZC_BM AS c ON b.ZZCBM = c.ZZCBM LEFT JOIN
dbo.JCXX_QJMC_BM AS d ON b.QJMCBM = d.QJMCBM LEFT JOIN
dbo.YQSF_DPII_JDRQ as g on g.id = a.id 											
where d.QJMCBM = 1000 and e.djrq>'2019-04-21'
";
    }
}