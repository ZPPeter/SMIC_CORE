using SMIC.SJCL;
using SMIC.SJCL.Common;
using System;
using System.Threading.Tasks;
using SMIC.SDIM.SJCL.Dtos;

namespace SMIC.SDIM.SJCL
{
    public class TestAppServices : SMICAppServiceBase
    {
        private readonly CertAppServices _cert;
        public TestAppServices( CertAppServices cert )
        {
            _cert = cert;
        }

        public Task<string[]> Test(string type)
        {
            //string type = "M01";
            int ID = 1000000085;
            string xhgg = "";
            string ccbh = "";
            string zzc = "";

            int[] Signer = { 370340, 370540, 370440 };
            JDJLFM jdjlfm = new JDJLFM(); // 原始记录封面
            RawTemplate template = new RawTemplate(); // 原始记录模板
            Temperature wd = new Temperature();

            jdjlfm.DWMC = "山东省国土测绘院";
            jdjlfm.JDRQ = DateTime.Now;
            template.MBMC = type;

            switch (type)
            {
                case "M01":
                    ID = 1000000087; // 1000000085 1000000086 1000000087
                    xhgg = "NET R9";
                    ccbh = "5213K83647"; // 5146K79805 5146K79846 5213K83647
                    zzc = "美国天宝";

                    jdjlfm.ID = ID;
                    template.QJMC = "GPS接收机";
                    template.QJMCBM = "1030";
                    Signer[0] = 370140;
                    Signer[1] = 370540;
                    Signer[2] = 370440;
                    jdjlfm.JJWD = wd.GetTemperature(ETemperature.OutRoom);// GPS    室外温度
                    break;
                case "M02":
                    ID = 1000000063;
                    xhgg = "TS60";
                    ccbh = "883406";
                    zzc = "瑞士徕卡";

                    jdjlfm.ID = ID;
                    template.QJMC = "全站仪";
                    template.QJMCBM = "1000";
                    Signer[0] = 370340;
                    Signer[1] = 370540;
                    Signer[2] = 370440;
                    jdjlfm.QT05 = new Random().Next(990, 1030).ToString(); // 气压
                    jdjlfm.JJWD = wd.GetTemperature(ETemperature.InOutRoom);// 全站仪 室内外温度
                    jdjlfm.CJJD = "2"; // CJJD = 2 or 5
                    jdjlfm.QT01 = "3"; // 补偿范围            
                    jdjlfm.QT02 = "1.5"; // BCJDA
                    jdjlfm.QT03 = "1"; // BCJDB
                    jdjlfm.QT04 = "2"; // 单双轴
                    break;
                case "M03": // 经纬仪                 
                    ID = 1000000078;
                    xhgg = "";
                    ccbh = "";
                    zzc = "";
                                
                    jdjlfm.ID = ID;
                    template.QJMC = "经纬仪";
                    template.QJMCBM = "1010";
                    Signer[0] = 370260;
                    Signer[1] = 370540;
                    Signer[2] = 370440;
                    jdjlfm.JJWD = wd.GetTemperature(ETemperature.InRoom);
                    break;
                case "M04": // 水准仪 
                    // DSZ05、DSZ1、DSZ3
                    // DS05、DS1、DS3
                    ID = 1000000075;
                    xhgg = "LS15"; // 数字水准仪
                    ccbh = "703849";
                    zzc = "瑞士徕卡";

                    // CJJD 1     2      3    4     5    6
                    // CJJD DS05、DSZ05、DS1、DSZ1、DS3、DSZ3

                    jdjlfm.CJJD = "1"; // 1 3 5 水准管水准仪 - 交叉误差
                    jdjlfm.CJJD = "2"; // 2 4 6 自安平水准仪，电子水准仪2 4

                    jdjlfm.ID = ID;
                    template.QJMC = "水准仪";
                    template.QJMCBM = "1020";
                    Signer[0] = 370260;
                    Signer[1] = 370540;
                    Signer[2] = 370440;
                    jdjlfm.JJWD = wd.GetTemperature(ETemperature.InRoom);
                    break;
                case "M05": // 手持激光测距仪
                    ID = 1000000111;
                    xhgg = "DISTO D510";
                    ccbh = "1073550958";
                    zzc = "瑞士徕卡";
                                
                    jdjlfm.ID = ID;
                    template.QJMC = "手持激光测距仪";
                    template.QJMCBM = "1040";
                    Signer[0] = 370440;
                    Signer[1] = 370540;
                    Signer[2] = 370440;
                    jdjlfm.JJWD = wd.GetTemperature(ETemperature.InRoom);
                    break;
                case "M06":
                    ID = 1000000092;
                    xhgg = "";
                    ccbh = "";
                    zzc = "";

                    jdjlfm.ID = ID;
                    template.QJMC = "电子经纬仪";
                    template.QJMCBM = "1100";
                    Signer[0] = 370260;
                    Signer[1] = 370540;
                    Signer[2] = 370440;
                    jdjlfm.CJJD = "2"; // CJJD = 2 or 5
                    jdjlfm.QT01 = "3"; // 补偿范围            
                    jdjlfm.QT04 = "2"; // 单双轴
                    jdjlfm.JJWD = wd.GetTemperature(ETemperature.InRoom);
                    break;
                default:
                    break;
            }

            jdjlfm.QJMC = template.QJMC; // 可能和 template.QJMC 不一致
                       
            jdjlfm.XHGG = xhgg;
            jdjlfm.CCBH = ccbh;
            jdjlfm.ZZC = zzc;

            CertDto ipt = new CertDto();
            ipt.rawTemplate = template;
            ipt.jdjlfm = jdjlfm;
            ipt.Signer = Signer;

            return _cert.MakeCert(ipt);
            //return _cert.MakeCert(jdjlfm, template, Signer);
        }
    }
}
