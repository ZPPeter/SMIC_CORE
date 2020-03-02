using System;
using System.IO;
using System.Drawing;
using Spire.Xls;
using Spire.Doc;
using Spire.Doc.Reporting;
using SMIC.SJCL.Common;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SMIC.SJCL
{
    public class PluginsService : BasePluginsService
    {
        Worksheet sheet;
        public override string[] Handle(RawTemplate template, JDJLFM jdjlfm, int[] Signer)
        {
            return MakeXls(jdjlfm, template, Signer);
        }
        public override string[] Handle(RawTemplate template, JDJLFM jdjlfm)
        {
            return MakeXls(jdjlfm, template);
        }
        public override void Handle(string QJMCBM, string ID, int[] Signer)
        {
            SignerCert(QJMCBM, ID, Signer);
        }

        public string[] MakeXls(JDJLFM jdjlfm, RawTemplate template)
        {
            Random rnd = new Random();
            Workbook tempbook = new Workbook();
            tempbook.Version = ExcelVersion.Version2010;
            var baseDirectory = Directory.GetCurrentDirectory();
            var xlsTempPath = Path.Combine(baseDirectory, @"wwwroot\Templates\" + template.MBMC + ".xls"); // 模板
            var xlsPath = Path.Combine(baseDirectory, "wwwroot\\Results\\Xls\\" + DateTime.Now.Year + "\\" + template.QJMCBM);  // 原始记录目录
            var docPath = Path.Combine(baseDirectory, "wwwroot\\Results\\Doc\\" + DateTime.Now.Year + "\\" + template.QJMCBM);  // 检定证书目录

            if (!Directory.Exists(xlsPath))
                Directory.CreateDirectory(xlsPath);

            if (!Directory.Exists(docPath))
                Directory.CreateDirectory(docPath);

            tempbook.LoadFromFile(xlsTempPath);
            Worksheet sheet = tempbook.Worksheets[0];

            sheet.Range["B2"].Text = jdjlfm.DWMC;
            sheet.Range["B4"].Text = jdjlfm.QJMC;
            sheet.Range["I4"].Text = jdjlfm.ZSBH;

            string zzc = jdjlfm.ZZC.ToString().Trim();
            string xhgg = jdjlfm.XHGG.ToString().Trim();
            string ccbh = jdjlfm.CCBH.ToString().Trim();

            if (ccbh == "")
            {
                ccbh = "/";
            }

            string ccbh1, ccbh2 = "";
            string zzc1, zzc2 = "";
            string xhgg1, xhgg2 = "";

            if (zzc.Length > 9)
            {
                zzc1 = zzc.Substring(0, 9);
                zzc2 = zzc.Substring(9, zzc.Length - 9);
            }
            else
                zzc1 = zzc;

            if (xhgg.Length > 12)
            {
                xhgg1 = xhgg.Substring(0, 12);
                xhgg2 = xhgg.Substring(12, xhgg.Length - 12);
            }
            else
                xhgg1 = xhgg;

            if (ccbh.Length > 15)
            {
                ccbh1 = ccbh.Substring(0, 15);
                ccbh2 = ccbh.Substring(15, ccbh.Length - 15);
            }
            else
                ccbh1 = ccbh;

            sheet.Range["B5"].Text = zzc1;
            sheet.Range["F5"].Text = xhgg1;
            sheet.Range["I5"].Text = ccbh1;
            sheet.Range["B6"].Text = zzc2;
            sheet.Range["F6"].Text = xhgg2;
            sheet.Range["I6"].Text = ccbh2;

            //sheet.Range["B14"].DateTimeValue = jdjlfm.JDRQ;
            sheet.Range["B14"].Text = string.Format("{0:D}", Convert.ToDateTime(jdjlfm.JDRQ));
            sheet.Range["G14"].Text = string.Format("{0:D}", Convert.ToDateTime(jdjlfm.JDRQ).AddYears(1).AddDays(-1)); //Convert.ToDateTime(jdjlfm.JDRQ).AddYears(1).AddDays(-1).ToString("D");

            sheet.Range["C13"].Text = jdjlfm.JJWD;

            //原始记录验证码
            sheet.Range["FF1"].Style.Font.Color = Color.White;
            sheet.Range["FF1"].Text = jdjlfm.ID.ToString();

            // ToDo 数据处理
            sheet = tempbook.Worksheets[1]; // 一
                                            // 数据 - 视准轴竖轴垂直度
            double res = 0.0;
            if (jdjlfm.CJJD == "DS05" || jdjlfm.CJJD == "DS1" || jdjlfm.CJJD == "DS3")
            {
                if (jdjlfm.CJJD == "DS3")
                    res = 2 + Math.Round(rnd.NextDouble() * 3, 0);
                else
                    res = 1 + Math.Round(rnd.NextDouble() * 2, 0);
                sheet.Range["D12"].NumberValue = res; // 交叉误差 3 3 5

                res = 7 + Math.Round(rnd.NextDouble() * 3, 0);
                sheet.Range["D13"].NumberValue = res; // i角误差 

                sheet.Range["D15"].Text = "/"; // 自安平的指标
                sheet.Range["D16"].Text = "/";

            }
            else // 自安平
            {
                sheet.Range["D12"].Text = "/"; // 自安平无交叉误差
                res = 7 + Math.Round(rnd.NextDouble() * 3, 0);
                sheet.Range["D13"].NumberValue = res; // i角误差 
                                                      // D14,15 从后面读取
                res = 8 + Math.Round(rnd.NextDouble() * 3, 0);
                sheet.Range["D16"].NumberValue = res; // 补偿范围
            }

            sheet = tempbook.Worksheets[2]; // 二

            double apjd = 0;
            double[] APJD = { 0.4, 0.3, 0.45, 0.35, 1, 0.8 };
            switch(jdjlfm.CJJD){
                case "DS05":
                    apjd = APJD[0];
                    break;
                case "DSZ05":
                    apjd = APJD[1];
                    break;
                case "DS1":
                    apjd = APJD[2];
                    break;
                case "DSZ1":
                    apjd = APJD[3];
                    break;
                case "DS3":
                    apjd = APJD[4];
                    break;
                case "DSZ3":
                    apjd = APJD[5];
                    break;
            }

            for (int i = 9; i <= 27; i = i + 2)
            {
                res = 2 + Math.Round(1 - rnd.NextDouble() * 1, 1) * apjd;
                sheet.Range["B" + i].NumberValue = res;
            }
            var ret3 = sheet.Range["I32"].NumberText;

            sheet = tempbook.Worksheets[3]; // 三 
            double seed = 10;
            if (jdjlfm.CJJD == "DS3" || jdjlfm.CJJD == "DSZ3")
                seed = 15;
            for (int i = 3; i <= 7; i++)  // 数据 D3 - H6
            {
                for (int j = 2; j < 6; j++)
                {
                    res = 100 + Math.Round(15 - rnd.NextDouble() * 15 * 2, 0);
                    sheet.Range[j, i].NumberValue = res;
                }
            }
            var ret2 = sheet.Range["E17"].NumberText;

            // 测微器行差、回程差 没处理
            sheet = tempbook.Worksheets[4]; // 四
            if (jdjlfm.CJJD == "DS05" || jdjlfm.CJJD == "DS1" || jdjlfm.CJJD == "DS3")
            {
                sheet.Range["D27"].Text = "/";
                sheet.Range["I27"].Text = "/";
                sheet.Range["D29"].Text = "/";
                sheet.Range["I29"].Text = "/";
                sheet.Range["I31"].Text = "/";
                sheet.Range["C8:K24"].Text = "/";
            }
            else
            { // 自安平
                for (int i = 8; i < 26; i++)
                {
                    res = 4 + Math.Round(1 - rnd.NextDouble() * 2, 1);
                    sheet.Range["C" + i].NumberValue = res;
                    res = 3 + Math.Round(1 - rnd.NextDouble() * 2, 1);
                    sheet.Range["F" + i].NumberValue = res;
                }
            }
            var ret1 = sheet.Range["I31"].NumberText;

            // 望远镜调焦运行误差,测微器，自安平补偿误差写回P2
            sheet = tempbook.Worksheets[1]; // P2
            if (jdjlfm.CJJD == "DSZ05" || jdjlfm.CJJD == "DSZ1" || jdjlfm.CJJD == "DSZ3")
            {
                //ret1 = x.GetValue(4, "I", 31, 2);
                sheet.Range["D15"].Text = ret1;// .Formula = "=水准仪记录5!I31"; // 来自于 P5         /     ″
            }
            //ret2 = x.GetValue(3, "E", 17, 1);
            sheet.Range["D14"].Text = ret2;//  .Formula = "=水准仪记录4!E17"; // 来自于 P4         mm

            //ret3 = x.GetValue(2, "I", 32, 1);
            sheet.Range["D11"].Text = ret3;// .Formula = "=水准仪记录3!I32"; // 来自于 P3         ″

            //sheet = tempbook.Worksheets[2]; // P3
            //sheet.Range["I32"].NumberValue = ret3;
            //sheet = tempbook.Worksheets[3]; // P4
            //sheet.Range["E17"].NumberValue = ret2;
            //sheet = tempbook.Worksheets[4]; // P5
            //sheet.Range["I31"].NumberValue = ret1;

            /*
                // tempbook.PrintDocument.Print();// 免费版只能打印3页
                Stream stream = new MemoryStream();
                tempbook.SaveToStream(stream);
                XlsPrinter.Print(stream);
            */

            tempbook.SaveToFile(Path.Combine(xlsPath, jdjlfm.ID.ToString() + ".xls"));

            string[] RES = new string[10];
            tempbook.CalculateAllValue();

            RES[0] = jdjlfm.JJWD; // A01 温度
            RES[1] = "";          // A02 未用 

            sheet = tempbook.Worksheets[1];
            RES[2] = sheet.Range["D11"].NumberText.ToString(); // A03 视准线的安平误差
            RES[3] = sheet.Range["D12"].NumberText.ToString(); // A04 交叉误差
            RES[4] = sheet.Range["D13"].NumberText.ToString(); // A05 视准线误差(i角误差)  
            RES[5] = sheet.Range["D14"].NumberText.ToString(); // A06 望远镜调焦运行误差
            RES[6] = sheet.Range["D9"].NumberText.ToString();  // A07 测微器行差
            RES[7] = sheet.Range["E9"].NumberText.ToString();  // A08 测微器回程差
            RES[8] = sheet.Range["D15"].NumberText.ToString(); // A09 自动安平水准仪补偿误差
            RES[9] = sheet.Range["D16"].NumberText.ToString(); // A10 自动安平水准仪补偿工作范围

            MakeCert(jdjlfm, template, RES);

            string[] resA17 = new string[RES.Length + 3];
            resA17[0] = jdjlfm.ID.ToString();
            resA17[1] = jdjlfm.ZSBH;
            for (int i = 2; i < resA17.Length-1; i++)
                resA17[i] = RES[i - 2];
            resA17[resA17.Length - 1] = jdjlfm.CJJD;//精度指标
            return resA17;
        }

        private void MakeCert(JDJLFM jdjlfm, RawTemplate template, string[] A)
        {
            var baseDirectory = Directory.GetCurrentDirectory();
            var imgQRC = Path.Combine(baseDirectory, @"wwwroot\Temp\" + jdjlfm.ID + ".png");
            if (!File.Exists(imgQRC))
            {
                System.DrawingCore.Image image = SJCLQRCode.GetQRCode(jdjlfm.ID.ToString(), 2); // ID 二维码
                image.Save(imgQRC);
            }
            Document doc = new Document();
            doc.LoadFromFileInReadMode(Path.Combine(baseDirectory, @"wwwroot\DocTemp\" + template.QJMCBM + ".docx"), Spire.Doc.FileFormat.Docx);
            string Y1 = jdjlfm.JDRQ.Year.ToString();
            string M1 = jdjlfm.JDRQ.Month.ToString().PadLeft(2, '0');
            string D1 = jdjlfm.JDRQ.Day.ToString().PadLeft(2, '0');
            string Y2 = "";
            string M2 = "";
            string D2 = "";

            var tmpFieldNames = new string[] { "QRC", "ZSBH", "DWMC", "QJMC", "XHGG", "CCBH", "ZZC", "JDRQY", "JDRQM", "JDRQD", "YXQZY", "YXQZM", "YXQZD", "A01", "A02", "A03", "A04", "A05", "A06", "A07", "A08", "A09", "A10" };
            var tmpFieldValues = new string[] { imgQRC, jdjlfm.ZSBH, jdjlfm.DWMC, jdjlfm.QJMC, jdjlfm.XHGG, jdjlfm.CCBH, jdjlfm.ZZC, Y1, M1, D1, Y2, M2, D2, A[0], A[1], A[2], A[3], A[4], A[5], A[6], A[7], A[8], A[9] };

            //创建合并图片自定义事件
            doc.MailMerge.MergeImageField += new MergeImageFieldEventHandler(MailMerge_MergeImageField);

            //合并模板
            doc.MailMerge.Execute(tmpFieldNames, tmpFieldValues);
            doc.SaveToFile(Path.Combine(baseDirectory, "wwwroot\\Results\\Doc\\" + DateTime.Now.Year + "\\" + template.QJMCBM + "\\" + jdjlfm.ID + ".docx"), Spire.Doc.FileFormat.Docx);
            try
            {
                File.Delete(imgQRC);
            }
            catch { }
        }
        public string[] MakeXls(JDJLFM jdjlfm, RawTemplate template, int[] Signer)
        {
            Random rnd = new Random();
            Workbook tempbook = new Workbook();
            tempbook.Version = ExcelVersion.Version2010;
            var baseDirectory = Directory.GetCurrentDirectory();
            var xlsTempPath = Path.Combine(baseDirectory, @"wwwroot\Templates\" + template.MBMC + ".xls"); // 模板
            var xlsPath = Path.Combine(baseDirectory, "wwwroot\\Results\\Xls\\" + DateTime.Now.Year + "\\" + template.QJMCBM);  // 原始记录目录
            var docPath = Path.Combine(baseDirectory, "wwwroot\\Results\\Doc\\" + DateTime.Now.Year + "\\" + template.QJMCBM);  // 检定证书目录

            if (!Directory.Exists(xlsPath))
                Directory.CreateDirectory(xlsPath);

            if (!Directory.Exists(docPath))
                Directory.CreateDirectory(docPath);

            tempbook.LoadFromFile(xlsTempPath);
            Worksheet sheet = tempbook.Worksheets[0];

            sheet.Range["B2"].Text = jdjlfm.DWMC;
            sheet.Range["B4"].Text = jdjlfm.QJMC;
            sheet.Range["I4"].Text = jdjlfm.ZSBH;

            string zzc = jdjlfm.ZZC.ToString().Trim();
            string xhgg = jdjlfm.XHGG.ToString().Trim();
            string ccbh = jdjlfm.CCBH.ToString().Trim();

            if (ccbh == "")
            {
                ccbh = "/";
            }

            string ccbh1, ccbh2 = "";
            string zzc1, zzc2 = "";
            string xhgg1, xhgg2 = "";

            if (zzc.Length > 9)
            {
                zzc1 = zzc.Substring(0, 9);
                zzc2 = zzc.Substring(9, zzc.Length - 9);
            }
            else
                zzc1 = zzc;

            if (xhgg.Length > 12)
            {
                xhgg1 = xhgg.Substring(0, 12);
                xhgg2 = xhgg.Substring(12, xhgg.Length - 12);
            }
            else
                xhgg1 = xhgg;

            if (ccbh.Length > 15)
            {
                ccbh1 = ccbh.Substring(0, 15);
                ccbh2 = ccbh.Substring(15, ccbh.Length - 15);
            }
            else
                ccbh1 = ccbh;

            sheet.Range["B5"].Text = zzc1;
            sheet.Range["F5"].Text = xhgg1;
            sheet.Range["I5"].Text = ccbh1;
            sheet.Range["B6"].Text = zzc2;
            sheet.Range["F6"].Text = xhgg2;
            sheet.Range["I6"].Text = ccbh2;

            //sheet.Range["B14"].DateTimeValue = jdjlfm.JDRQ;
            sheet.Range["B14"].Text = string.Format("{0:D}", Convert.ToDateTime(jdjlfm.JDRQ));
            sheet.Range["G14"].Text = string.Format("{0:D}", Convert.ToDateTime(jdjlfm.JDRQ).AddYears(1).AddDays(-1)); //Convert.ToDateTime(jdjlfm.JDRQ).AddYears(1).AddDays(-1).ToString("D");

            sheet.Range["C13"].Text = jdjlfm.JJWD;

            //原始记录验证码
            sheet.Range["FF1"].Style.Font.Color = Color.White;
            sheet.Range["FF1"].Text = jdjlfm.ID.ToString();

            // ToDo 数据处理
            sheet = tempbook.Worksheets[1]; // 一
                                            // 数据 - 视准轴竖轴垂直度
            double res = 0.0;
            if (jdjlfm.CJJD == "DS05" || jdjlfm.CJJD == "DS1" || jdjlfm.CJJD == "DS3")
            {
                if (jdjlfm.CJJD == "DS3")
                    res = 2 + Math.Round(rnd.NextDouble() * 3, 0);
                else
                    res = 1 + Math.Round(rnd.NextDouble() * 2, 0);
                sheet.Range["D12"].NumberValue = res; // 交叉误差 3 3 5

                res = 7 + Math.Round(rnd.NextDouble() * 3, 0);
                sheet.Range["D13"].NumberValue = res; // i角误差 

                sheet.Range["D15"].Text = "/"; // 自安平的指标
                sheet.Range["D16"].Text = "/";

            }
            else // 自安平
            {
                sheet.Range["D12"].Text = "/"; // 自安平无交叉误差
                res = 7 + Math.Round(rnd.NextDouble() * 3, 0);
                sheet.Range["D13"].NumberValue = res; // i角误差 
                                                      // D14,15 从后面读取
                res = 8 + Math.Round(rnd.NextDouble() * 3, 0);
                sheet.Range["D16"].NumberValue = res; // 补偿范围
            }

            sheet = tempbook.Worksheets[2]; // 二

            double apjd = 0;
            double[] APJD = { 0.4, 0.3, 0.45, 0.35, 1, 0.8 };
            switch (jdjlfm.CJJD)
            {
                case "DS05":
                    apjd = APJD[0];
                    break;
                case "DSZ05":
                    apjd = APJD[1];
                    break;
                case "DS1":
                    apjd = APJD[2];
                    break;
                case "DSZ1":
                    apjd = APJD[3];
                    break;
                case "DS3":
                    apjd = APJD[4];
                    break;
                case "DSZ3":
                    apjd = APJD[5];
                    break;
            }
            for (int i = 9; i <= 27; i = i + 2)
            {
                res = 2 + Math.Round(1 - rnd.NextDouble() * 1, 1) * apjd;
                sheet.Range["B" + i].NumberValue = res;
            }
            var ret3 = sheet.Range["I32"].NumberText;

            sheet = tempbook.Worksheets[3]; // 三 
            double seed = 10;
            if (jdjlfm.CJJD == "DS3" || jdjlfm.CJJD == "DSZ3")
                seed = 15;
            for (int i = 3; i <= 7; i++)  // 数据 D3 - H6
            {
                for (int j = 2; j < 6; j++)
                {
                    res = 100 + Math.Round(15 - rnd.NextDouble() * 15 * 2, 0);
                    sheet.Range[j, i].NumberValue = res;
                }
            }
            var ret2 = sheet.Range["E17"].NumberText;

            // 测微器行差、回程差 没处理
            sheet = tempbook.Worksheets[4]; // 四
            if (jdjlfm.CJJD == "DS05" || jdjlfm.CJJD == "DS1" || jdjlfm.CJJD == "DS3")
            {
                sheet.Range["D27"].Text = "/";
                sheet.Range["I27"].Text = "/";
                sheet.Range["D29"].Text = "/";
                sheet.Range["I29"].Text = "/";
                sheet.Range["I31"].Text = "/";
                sheet.Range["C8:K24"].Text = "/";
            }
            else
            { // 自安平
                for (int i = 8; i < 26; i++)
                {
                    res = 4 + Math.Round(1 - rnd.NextDouble() * 2, 1);
                    sheet.Range["C" + i].NumberValue = res;
                    res = 3 + Math.Round(1 - rnd.NextDouble() * 2, 1);
                    sheet.Range["F" + i].NumberValue = res;
                }
            }
            var ret1 = sheet.Range["I31"].NumberText;

            // 望远镜调焦运行误差,测微器，自安平补偿误差写回P2
            sheet = tempbook.Worksheets[1]; // P2
            if (jdjlfm.CJJD == "DSZ05" || jdjlfm.CJJD == "DSZ1" || jdjlfm.CJJD == "DSZ3")
            {
                //ret1 = x.GetValue(4, "I", 31, 2);
                sheet.Range["D15"].Text = ret1;// .Formula = "=水准仪记录5!I31"; // 来自于 P5         /     ″
            }
            //ret2 = x.GetValue(3, "E", 17, 1);
            sheet.Range["D14"].Text = ret2;//  .Formula = "=水准仪记录4!E17"; // 来自于 P4         mm

            //ret3 = x.GetValue(2, "I", 32, 1);
            sheet.Range["D11"].Text = ret3;// .Formula = "=水准仪记录3!I32"; // 来自于 P3         ″

            //sheet = tempbook.Worksheets[2]; // P3
            //sheet.Range["I32"].NumberValue = ret3;
            //sheet = tempbook.Worksheets[3]; // P4
            //sheet.Range["E17"].NumberValue = ret2;
            //sheet = tempbook.Worksheets[4]; // P5
            //sheet.Range["I31"].NumberValue = ret1;

            /*
                // tempbook.PrintDocument.Print();// 免费版只能打印3页
                Stream stream = new MemoryStream();
                tempbook.SaveToStream(stream);
                XlsPrinter.Print(stream);
            */

            tempbook.SaveToFile(Path.Combine(xlsPath, jdjlfm.ID.ToString() + ".xls"));

            string[] RES = new string[10];
            tempbook.CalculateAllValue();

            RES[0] = jdjlfm.JJWD; // A01 温度
            RES[1] = "";          // A02 未用 

            sheet = tempbook.Worksheets[1];
            RES[2] = sheet.Range["D11"].NumberText.ToString(); // A03 视准线的安平误差
            RES[3] = sheet.Range["D12"].NumberText.ToString(); // A04 交叉误差
            RES[4] = sheet.Range["D13"].NumberText.ToString(); // A05 视准线误差(i角误差)  
            RES[5] = sheet.Range["D14"].NumberText.ToString(); // A06 望远镜调焦运行误差
            RES[6] = sheet.Range["D9"].NumberText.ToString();  // A07 测微器行差
            RES[7] = sheet.Range["E9"].NumberText.ToString();  // A08 测微器回程差
            RES[8] = sheet.Range["D15"].NumberText.ToString(); // A09 自动安平水准仪补偿误差
            RES[9] = sheet.Range["D16"].NumberText.ToString(); // A10 自动安平水准仪补偿工作范围

            MakeCert(jdjlfm, template, Signer, RES);

            string[] resA17 = new string[RES.Length + 3];
            resA17[0] = jdjlfm.ID.ToString();
            resA17[1] = jdjlfm.ZSBH;
            for (int i = 2; i < resA17.Length-1; i++)
                resA17[i] = RES[i - 2];
            resA17[resA17.Length - 1] = jdjlfm.CJJD;//精度指标
            return resA17;
        }

        private void MakeCert(JDJLFM jdjlfm, RawTemplate template, int[] Signer, string[] A)
        {
            var baseDirectory = Directory.GetCurrentDirectory();
            var imgQRC = Path.Combine(baseDirectory, @"wwwroot\Temp\" + jdjlfm.ID + ".png");
            if (!File.Exists(imgQRC))
            {
                System.DrawingCore.Image image = SJCLQRCode.GetQRCode(jdjlfm.ID.ToString(), 2); // ID 二维码
                image.Save(imgQRC);
            }
            Document doc = new Document();
            doc.LoadFromFileInReadMode(Path.Combine(baseDirectory, @"wwwroot\DocTemp\" + template.QJMCBM + ".docx"), Spire.Doc.FileFormat.Docx);
            var imgJDR = Path.Combine(baseDirectory, @"wwwroot\SignImages\" + Signer[0] + ".png");
            var imgHYR = Path.Combine(baseDirectory, @"wwwroot\SignImages\" + Signer[1] + ".png");
            var imgPZR = Path.Combine(baseDirectory, @"wwwroot\SignImages\" + Signer[2] + ".png");
            string Y1 = jdjlfm.JDRQ.Year.ToString();
            string M1 = jdjlfm.JDRQ.Month.ToString().PadLeft(2, '0');
            string D1 = jdjlfm.JDRQ.Day.ToString().PadLeft(2, '0');
            string Y2 = "";
            string M2 = "";
            string D2 = "";

            var tmpFieldNames = new string[] { "QRC", "JDR", "HYR", "PZR", "ZSBH", "DWMC", "QJMC", "XHGG", "CCBH", "ZZC", "JDRQY", "JDRQM", "JDRQD", "YXQZY", "YXQZM", "YXQZD", "A01", "A02", "A03", "A04", "A05", "A06", "A07", "A08", "A09", "A10" };
            var tmpFieldValues = new string[] { imgQRC, imgJDR, imgHYR, imgPZR, jdjlfm.ZSBH, jdjlfm.DWMC, jdjlfm.QJMC, jdjlfm.XHGG, jdjlfm.CCBH, jdjlfm.ZZC, Y1, M1, D1, Y2, M2, D2, A[0], A[1], A[2], A[3], A[4], A[5], A[6], A[7], A[8], A[9] };

            //创建合并图片自定义事件
            doc.MailMerge.MergeImageField += new MergeImageFieldEventHandler(MailMerge_MergeImageField);

            //合并模板
            doc.MailMerge.Execute(tmpFieldNames, tmpFieldValues);
            doc.SaveToFile(Path.Combine(baseDirectory, "wwwroot\\Results\\Doc\\" + DateTime.Now.Year + "\\" + template.QJMCBM + "\\" + jdjlfm.ID + ".docx"), Spire.Doc.FileFormat.Docx);
            try
            {
                File.Delete(imgQRC);
            }
            catch { }
        }

        private void SignerCert(string QJMCBM, string ID, int[] Signer)
        {
            var baseDirectory = Directory.GetCurrentDirectory();
            Document doc = new Document();
            doc.LoadFromFileInReadMode(Path.Combine(baseDirectory, "wwwroot\\Results\\Doc\\" + DateTime.Now.Year + "\\" + QJMCBM + "\\" + ID + ".docx"), Spire.Doc.FileFormat.Docx);
            var imgJDR = Path.Combine(baseDirectory, @"wwwroot\SignImages\" + Signer[0] + ".png");
            var imgHYR = Path.Combine(baseDirectory, @"wwwroot\SignImages\" + Signer[1] + ".png");
            var imgPZR = Path.Combine(baseDirectory, @"wwwroot\SignImages\" + Signer[2] + ".png");

            var tmpFieldNames = new string[] { "JDR", "HYR", "PZR" };
            var tmpFieldValues = new string[] { imgJDR, imgHYR, imgPZR };

            //创建合并图片自定义事件
            doc.MailMerge.MergeImageField += new MergeImageFieldEventHandler(MailMerge_MergeImageField);

            //合并模板
            doc.MailMerge.Execute(tmpFieldNames, tmpFieldValues);
            doc.SaveToFile(Path.Combine(baseDirectory, "wwwroot\\Results\\Doc\\" + DateTime.Now.Year + "\\" + QJMCBM + "\\" + ID + ".docx"), Spire.Doc.FileFormat.Docx);
        }

        //载入图片
        static void MailMerge_MergeImageField(object sender, MergeImageFieldEventArgs field)
        {
            string filePath = field.FieldValue as string;
            if (!string.IsNullOrEmpty(filePath))
            {
                //field.Image = Image.FromStream(newSystem.IO.MemoryStream((byte[])e.FieldValue));
                field.Image = Image.FromFile(filePath);
            }
        }
    }
}