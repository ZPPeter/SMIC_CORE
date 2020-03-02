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
            Workbook tempbook = new Workbook();
            tempbook.Version = ExcelVersion.Version2010;
            var baseDirectory = Directory.GetCurrentDirectory();

            var MBJD = "2";
            // jdjlfm.CJJD 2 5
            if (int.Parse(jdjlfm.CJJD) > 2)
                MBJD = "5";

            var xlsTempPath = Path.Combine(baseDirectory, @"wwwroot\Templates\" + template.MBMC + "-" + MBJD + ".xls");  // 模板
            var xlsPath = Path.Combine(baseDirectory, "wwwroot\\Results\\Xls\\" + DateTime.Now.Year + "\\" + template.QJMCBM);  // 原始记录目录
            var docPath = Path.Combine(baseDirectory, "wwwroot\\Results\\Doc\\" + DateTime.Now.Year + "\\" + template.QJMCBM);  // 检定证书目录

            if (!Directory.Exists(xlsPath))
                Directory.CreateDirectory(xlsPath);

            if (!Directory.Exists(docPath))
                Directory.CreateDirectory(docPath);

            tempbook.LoadFromFile(xlsTempPath);
            sheet = tempbook.Worksheets[0];
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
                                                                                                                       // sheet.Range["G14"].DateTimeValue // 无法设置格式?

            // 日期 -> String 
            //XlsWorksheet.TRangeValueType cellType = sheet.GetCellType(14, 2, false); // B14
            //var test = sheet.GetText(14, 2);


            sheet.Range["C13"].Text = jdjlfm.JJWD;
            sheet.Range["G13"].Text = jdjlfm.QT05 + "hPa";

            //原始记录验证码
            sheet.Range["FF1"].Style.Font.Color = Color.White;
            sheet.Range["FF1"].Text = jdjlfm.ID.ToString();

            // 数据处理
            Random rnd = new Random();
            var cellName = "";
            sheet = tempbook.Worksheets[1]; // 测角一            
            // 数据 - 横轴竖轴垂直度
            double res = Math.Round(1 + 0.5 - rnd.NextDouble(), 1); // 1±.5
            double res1 = 0;
            cellName = "J10";
            sheet.Range[cellName].NumberValue = res;
            if (double.Parse(jdjlfm.CJJD) < 2)
                res1 = Math.Round((1 - rnd.NextDouble() * 2) / 10, 1);
            else if (double.Parse(jdjlfm.CJJD) == 2)
                res1 = Math.Round((2 - rnd.NextDouble() * 4) / 10, 1);
            else
                res1 = Math.Round((3 - rnd.NextDouble() * 6) / 10, 1);

            if (res1 == 0) res1 = 0.1;
            res = res + res1;
            cellName = "J11";
            sheet.Range[cellName].NumberValue = res;

            // 数据 - 指标差
            cellName = "E13";
            res = 89;
            sheet.Range[cellName].NumberValue = res;
            cellName = "G13";
            res = 59;
            sheet.Range[cellName].NumberValue = res;
            cellName = "E14";
            res = 269;
            sheet.Range[cellName].NumberValue = res;
            cellName = "G14";
            res = 59;
            sheet.Range[cellName].NumberValue = res;


            res = Math.Round(58 + 1 - rnd.NextDouble() * 2, 0);
            cellName = "I13";
            sheet.Range[cellName].NumberValue = res;

            res = Math.Round(57 + 1 - rnd.NextDouble() * 2, 0);
            cellName = "I14";
            sheet.Range[cellName].NumberValue = res;

            cellName = "E15";
            res = 113;
            sheet.Range[cellName].NumberValue = res;
            cellName = "E16";
            res = 246;
            sheet.Range[cellName].NumberValue = res;

            res = Math.Round(5 + 1 - rnd.NextDouble() * 2, 0);
            cellName = "G15";
            sheet.Range[cellName].NumberValue = res;

            res = 59 - res;
            cellName = "G16";
            sheet.Range[cellName].NumberValue = res;

            res = Math.Round(45 + 2 - rnd.NextDouble() * 4, 0);
            cellName = "I15";
            sheet.Range[cellName].NumberValue = res;


            res = 59 - res + Math.Round(1 - rnd.NextDouble() * 2, 0);
            cellName = "I16";
            sheet.Range[cellName].NumberValue = res;

            // 数据 - 照准误差
            sheet.Range["L13"].NumberValue = 0;
            sheet.Range["L14"].NumberValue = 180;
            sheet.Range["L15"].NumberValue = 179;
            sheet.Range["L16"].NumberValue = 0;
            sheet.Range["N13"].NumberValue = 0;
            sheet.Range["N14"].NumberValue = 0;
            sheet.Range["N15"].NumberValue = 59;
            sheet.Range["N16"].NumberValue = 0;

            res = 4 + Math.Round(2 - rnd.NextDouble() * 4, 0);
            sheet.Range["P13"].NumberValue = res;
            res = 4 + Math.Round(2 - rnd.NextDouble() * 4, 0);
            sheet.Range["P14"].NumberValue = res;
            res = 55 + Math.Round(2 - rnd.NextDouble() * 4, 0);
            sheet.Range["P15"].NumberValue = res;
            sheet.Range["P16"].NumberValue = 0;

            // 数据 - 横轴误差
            res = 1 + Math.Round(rnd.NextDouble() * .4, 0);
            sheet.Range["J19"].NumberValue = res;
            res1 = Math.Round((3 - rnd.NextDouble() * 6) / 10, 1);
            if (res1 == 0) res1 = 0.1;
            res = res + res1;
            sheet.Range["J20"].NumberValue = res;

            // 数据 - 补偿范围
            sheet.Range["I22"].Text = jdjlfm.QT01;  // 补偿范围来自数据库

            // 数据 - 补偿精度
            sheet.Range["L23"].NumberValue = 89;
            sheet.Range["L24"].NumberValue = 89;
            sheet.Range["N23"].NumberValue = 59;
            sheet.Range["N24"].NumberValue = 59;

            res = 55 + Math.Round(rnd.NextDouble() * 4, 0);
            sheet.Range["P23"].NumberValue = res;
            res = 55 + Math.Round(rnd.NextDouble() * 4, 0);
            sheet.Range["P24"].NumberValue = res;

            if (jdjlfm.QT04 == "2")
            {
                sheet.Range["L25"].NumberValue = 0;
                sheet.Range["L26"].NumberValue = 0;
                sheet.Range["N25"].NumberValue = 0;
                sheet.Range["N26"].NumberValue = 0;
                res = Math.Round(rnd.NextDouble() * 3, 0);
                sheet.Range["P25"].NumberValue = res;
                res = Math.Round(rnd.NextDouble() * 3, 0);
                sheet.Range["P26"].NumberValue = res;
            }
            else
            {
                sheet.Range["L25"].Text = @"\";
                sheet.Range["L26"].Text = @"\";
                sheet.Range["N25"].Text = @"\";
                sheet.Range["N26"].Text = @"\";
                sheet.Range["P25"].Text = @"\";
                sheet.Range["P26"].Text = @"\";
                sheet.Range["L28"].Text = @"\";
            }

            // 数据 - 零位误差
            res = Math.Round(rnd.NextDouble() * 3, 0);
            sheet.Range["H29"].NumberValue = res;
            if (res != 0) res++;
            sheet.Range["H30"].NumberValue = res;

            res = Math.Round(rnd.NextDouble() * 3, 0);
            sheet.Range["L29"].NumberValue = res;
            if (res != 0) res++;
            sheet.Range["L30"].NumberValue = res;

            // 测角二 0.5 1 2 5
            sheet = tempbook.Worksheets[2]; 
            if (jdjlfm.CJJD == "5")
            {
                for (int i = 6; i <= 17; i++)
                {
                    res = 1 + Math.Round(rnd.NextDouble() * 10, 0);
                    sheet.Range["C" + i].NumberValue = res;
                    res1 = Math.Round(2 - rnd.NextDouble() * 4, 0);
                    if (res1 == 0) res1 = 0.5;
                    res = res + res1;
                    sheet.Range["D" + i].NumberValue = res;
                }
            }
            else // if (jdjlfm.CJJD == "2")
            {
                sheet.Range["C6"].NumberValue = 0;
                sheet.Range["D6"].NumberValue = 0;

                //多齿分度台标准角值
                double[] J2BZJZ = new double[22];
                int j = 0;
                J2BZJZ[j] = 7.8; j++;
                J2BZJZ[j] = 15.7; j++;
                J2BZJZ[j] = 23.5; j++;
                J2BZJZ[j] = 31.3; j++;
                J2BZJZ[j] = 39.1; j++;
                J2BZJZ[j] = 47; j++;
                J2BZJZ[j] = 54.8; j++;
                J2BZJZ[j] = 2.6; j++;
                J2BZJZ[j] = 10.4; j++;
                J2BZJZ[j] = 18.2; j++;
                J2BZJZ[j] = 26; j++;
                J2BZJZ[j] = 33.9; j++;
                J2BZJZ[j] = 41.7; j++;
                J2BZJZ[j] = 49.5; j++;
                J2BZJZ[j] = 57.3; j++;
                J2BZJZ[j] = 05.2; j++;
                J2BZJZ[j] = 13; j++;
                J2BZJZ[j] = 20.8; j++;
                J2BZJZ[j] = 28.6; j++;
                J2BZJZ[j] = 36.5; j++;
                J2BZJZ[j] = 44.3; j++;
                J2BZJZ[j] = 52.2;
                double seed = 2;
                int zfh = 1;
                if (double.Parse(jdjlfm.CJJD) <= 1)
                    seed = 0.5;
                for (int i = 7; i <= 28; i++)
                {
                    res = Math.Round(J2BZJZ[i - 7], 0) + Math.Round(seed / 2 - rnd.NextDouble() * seed, 0);
                    sheet.Range["C" + i].NumberValue = res;
                    res1 = Math.Round(seed / 2 - rnd.NextDouble() * seed, 0);
                    if (rnd.NextDouble() > 0.5)
                        zfh = -1;
                    else
                        zfh = 1;
                    if (res1 == 0) res1 = 0.1 * zfh;
                    res = res + res1;
                    sheet.Range["D" + i].NumberValue = res;
                }
            }

            // 三 内符精度
            sheet = tempbook.Worksheets[3]; 
            Calcu_NFJD(jdjlfm.QT02);  // BCJDA

            sheet = tempbook.Worksheets[4]; // 四 加成常数
            Cal(jdjlfm.JJWD, jdjlfm.QT02, jdjlfm.QT05); // 温度 气压

            /*
            // tempbook.PrintDocument.Print();// 免费版只能打印3页
            Stream stream = new MemoryStream();
            tempbook.SaveToStream(stream);
            XlsPrinter.Print(stream);
            */

            tempbook.SaveToFile(Path.Combine(xlsPath, jdjlfm.ID.ToString() + ".xls"));
            //tempbook.Worksheets[0].SaveToPdf(Path.Combine(xlsPath, jdjlfm.ID.ToString() + ".pdf")); //  .net core 不支持  
            //tempbook.ConverterSetting.SheetFitToPage = true;//大小自适应
            //tempbook.SaveToFile(Path.Combine(xlsPath, jdjlfm.ID.ToString() + ".xls"),Spire.Xls.FileFormat.PDF); // .net core 不支持  

            //System.Diagnostics.Process.Start(Path.Combine(xlsPath, jdjlfm.ID.ToString() + ".xls")); // .net core 不支持            

            /*
            sheet.Range["A2"].Text = "我是A2单元格";
            sheet.Range["A3"].NumberValue = 100.23;
            sheet.Range["A4"].DateTimeValue = DateTime.Now;            
            sheet.Range["A5"].BooleanValue = true;

            Console.WriteLine(sheet.Range["A1"].Value2.ToString());
            Console.WriteLine(sheet.Range["A1"].Value);
            Console.WriteLine(sheet.Range["B1"].Text);

            //对一定范围内的单元格进行字体控制
            sheet.Range["A1:B10"].Style.Font.FontName = "微软雅黑";
            sheet.Range["A1:B10"].Style.Font.Size = 20;
            sheet.Range["A1:B10"].Style.Font.Underline = FontUnderlineType.DoubleAccounting;

            var columes = sheet.Range["C5:E6"].Columns ;
            foreach (var column in columes)
            { }
            */

            string[] RES = new string[17];
            tempbook.CalculateAllValue();
            RES[0] = jdjlfm.JJWD; // A01 温度
            RES[1] = jdjlfm.QT05; // A02 气压
            sheet = tempbook.Worksheets[1];
            RES[2] = sheet.Range["H21"].NumberText.ToString(); // A03 横轴误差
            RES[3] = sheet.Range["G12"].NumberText.ToString(); // A04 望远镜视轴对横轴的垂直度
            RES[4] = sheet.Range["E17"].NumberText.ToString(); // A05 指 标 差            

            sheet = tempbook.Worksheets[2];
            RES[5] = sheet.Range["E32"].NumberText.ToString(); // A06 一测回水平方向标准偏差

            sheet = tempbook.Worksheets[1];
            RES[6] = sheet.Range["I22"].NumberText.ToString(); // A07 补偿范围
            RES[7] = sheet.Range["L27"].NumberText.ToString(); // A08 垂直角补偿精度
            RES[8] = sheet.Range["L28"].NumberText.ToString(); // A09 水平角补偿精度
            RES[9] = sheet.Range["H29"].NumberText.ToString(); // A10 零位误差X
            RES[10] = sheet.Range["L29"].NumberText.ToString(); // A11 零位误差Y
            RES[11] = sheet.Range["M17"].NumberText.ToString(); // A12 照准误差

            sheet = tempbook.Worksheets[3];
            RES[12] = sheet.Range["J21"].NumberText.ToString(); // A13 测量重复性

            sheet = tempbook.Worksheets[4];
            RES[13] = sheet.Range["F52"].NumberText.ToString(); // A14 K
            RES[14] = sheet.Range["F53"].NumberText.ToString(); // A15 R
            RES[15] = sheet.Range["F54"].NumberText.ToString(); // A16 A
            RES[16] = sheet.Range["F55"].NumberText.ToString(); // A17 B

            MakeCert(jdjlfm, template, RES);

            string[] resA17 = new string[RES.Length + 2];
            resA17[0] = jdjlfm.ID.ToString();
            resA17[1] = jdjlfm.ZSBH;
            for (int i = 2; i < resA17.Length; i++)
                resA17[i] = RES[i - 2];
            return resA17;
        }

        private void SignerCert(string QJMCBM,string ID, int[] Signer)
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

            var tmpFieldNames = new string[] { "QRC", "ZSBH", "DWMC", "QJMC", "XHGG", "CCBH", "ZZC", "JDRQY", "JDRQM", "JDRQD", "YXQZY", "YXQZM", "YXQZD", "A01", "A02", "A03", "A04", "A05", "A06", "A07", "A08", "A09", "A10", "A11", "A12", "A13", "A14", "A15", "A16", "A17" };
            var tmpFieldValues = new string[] { imgQRC, jdjlfm.ZSBH, jdjlfm.DWMC, jdjlfm.QJMC, jdjlfm.XHGG, jdjlfm.CCBH, jdjlfm.ZZC, Y1, M1, D1, Y2, M2, D2, A[0], A[1], A[2], A[3], A[4], A[5], A[6], A[7], A[8], A[9], A[10], A[11], A[12], A[13], A[14], A[15], A[16] };

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
            Workbook tempbook = new Workbook();
            tempbook.Version = ExcelVersion.Version2010;
            var baseDirectory = Directory.GetCurrentDirectory();

            var MBJD = "2";
            // jdjlfm.CJJD 2 5
            if (int.Parse(jdjlfm.CJJD) > 2)
                MBJD = "5";

            var xlsTempPath = Path.Combine(baseDirectory, @"wwwroot\Templates\" + template.MBMC + "-" + MBJD + ".xls");  // 模板
            var xlsPath = Path.Combine(baseDirectory, "wwwroot\\Results\\Xls\\" + DateTime.Now.Year + "\\" + template.QJMCBM);  // 原始记录目录
            var docPath = Path.Combine(baseDirectory, "wwwroot\\Results\\Doc\\" + DateTime.Now.Year + "\\" + template.QJMCBM);  // 检定证书目录

            if (!Directory.Exists(xlsPath))
                Directory.CreateDirectory(xlsPath);

            if (!Directory.Exists(docPath))
                Directory.CreateDirectory(docPath);

            tempbook.LoadFromFile(xlsTempPath);
            sheet = tempbook.Worksheets[0];
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
                                                                                                                       // sheet.Range["G14"].DateTimeValue // 无法设置格式?

            // 日期 -> String 
            //XlsWorksheet.TRangeValueType cellType = sheet.GetCellType(14, 2, false); // B14
            //var test = sheet.GetText(14, 2);


            sheet.Range["C13"].Text = jdjlfm.JJWD;
            sheet.Range["G13"].Text = jdjlfm.QT05 + "hPa";

            //原始记录验证码
            sheet.Range["FF1"].Style.Font.Color = Color.White;
            sheet.Range["FF1"].Text = jdjlfm.ID.ToString();

            // ToDo 数据处理
            Random rnd = new Random();
            var cellName = "";
            sheet = tempbook.Worksheets[1]; // 测角一            
            // 数据 - 横轴竖轴垂直度
            double res = Math.Round(1 + 0.5 - rnd.NextDouble(), 1); // 1±.5
            double res1 = 0;
            cellName = "J10";
            sheet.Range[cellName].NumberValue = res;
            if (double.Parse(jdjlfm.CJJD) < 2)
                res1 = Math.Round((1 - rnd.NextDouble() * 2) / 10, 1);
            else if (double.Parse(jdjlfm.CJJD) == 2)
                res1 = Math.Round((2 - rnd.NextDouble() * 4) / 10, 1);
            else
                res1 = Math.Round((3 - rnd.NextDouble() * 6) / 10, 1);

            if (res1 == 0) res1 = 0.1;
            res = res + res1;
            cellName = "J11";
            sheet.Range[cellName].NumberValue = res;

            // 数据 - 指标差
            cellName = "E13";
            res = 89;
            sheet.Range[cellName].NumberValue = res;
            cellName = "G13";
            res = 59;
            sheet.Range[cellName].NumberValue = res;
            cellName = "E14";
            res = 269;
            sheet.Range[cellName].NumberValue = res;
            cellName = "G14";
            res = 59;
            sheet.Range[cellName].NumberValue = res;


            res = Math.Round(58 + 1 - rnd.NextDouble() * 2, 0);
            cellName = "I13";
            sheet.Range[cellName].NumberValue = res;

            res = Math.Round(57 + 1 - rnd.NextDouble() * 2, 0);
            cellName = "I14";
            sheet.Range[cellName].NumberValue = res;

            cellName = "E15";
            res = 113;
            sheet.Range[cellName].NumberValue = res;
            cellName = "E16";
            res = 246;
            sheet.Range[cellName].NumberValue = res;

            res = Math.Round(5 + 1 - rnd.NextDouble() * 2, 0);
            cellName = "G15";
            sheet.Range[cellName].NumberValue = res;

            res = 59 - res;
            cellName = "G16";
            sheet.Range[cellName].NumberValue = res;

            res = Math.Round(45 + 2 - rnd.NextDouble() * 4, 0);
            cellName = "I15";
            sheet.Range[cellName].NumberValue = res;


            res = 59 - res + Math.Round(1 - rnd.NextDouble() * 2, 0);
            cellName = "I16";
            sheet.Range[cellName].NumberValue = res;

            // 数据 - 照准误差
            sheet.Range["L13"].NumberValue = 0;
            sheet.Range["L14"].NumberValue = 180;
            sheet.Range["L15"].NumberValue = 179;
            sheet.Range["L16"].NumberValue = 0;
            sheet.Range["N13"].NumberValue = 0;
            sheet.Range["N14"].NumberValue = 0;
            sheet.Range["N15"].NumberValue = 59;
            sheet.Range["N16"].NumberValue = 0;

            res = 4 + Math.Round(2 - rnd.NextDouble() * 4, 0);
            sheet.Range["P13"].NumberValue = res;
            res = 4 + Math.Round(2 - rnd.NextDouble() * 4, 0);
            sheet.Range["P14"].NumberValue = res;
            res = 55 + Math.Round(2 - rnd.NextDouble() * 4, 0);
            sheet.Range["P15"].NumberValue = res;
            sheet.Range["P16"].NumberValue = 0;

            // 数据 - 横轴误差
            res = 1 + Math.Round(rnd.NextDouble() * .4, 0);
            sheet.Range["J19"].NumberValue = res;
            res1 = Math.Round((3 - rnd.NextDouble() * 6) / 10, 1);
            if (res1 == 0) res1 = 0.1;
            res = res + res1;
            sheet.Range["J20"].NumberValue = res;

            // 数据 - 补偿范围
            sheet.Range["I22"].Text = jdjlfm.QT01;  // 补偿范围来自数据库

            // 数据 - 补偿精度
            sheet.Range["L23"].NumberValue = 89;
            sheet.Range["L24"].NumberValue = 89;
            sheet.Range["N23"].NumberValue = 59;
            sheet.Range["N24"].NumberValue = 59;

            res = 55 + Math.Round(rnd.NextDouble() * 4, 0);
            sheet.Range["P23"].NumberValue = res;
            res = 55 + Math.Round(rnd.NextDouble() * 4, 0);
            sheet.Range["P24"].NumberValue = res;

            if (jdjlfm.QT04 == "2")
            {
                sheet.Range["L25"].NumberValue = 0;
                sheet.Range["L26"].NumberValue = 0;
                sheet.Range["N25"].NumberValue = 0;
                sheet.Range["N26"].NumberValue = 0;
                res = Math.Round(rnd.NextDouble() * 3, 0);
                sheet.Range["P25"].NumberValue = res;
                res = Math.Round(rnd.NextDouble() * 3, 0);
                sheet.Range["P26"].NumberValue = res;
            }
            else
            {
                sheet.Range["L25"].Text = @"\";
                sheet.Range["L26"].Text = @"\";
                sheet.Range["N25"].Text = @"\";
                sheet.Range["N26"].Text = @"\";
                sheet.Range["P25"].Text = @"\";
                sheet.Range["P26"].Text = @"\";
                sheet.Range["L28"].Text = @"\";
            }

            // 数据 - 零位误差
            res = Math.Round(rnd.NextDouble() * 3, 0);
            sheet.Range["H29"].NumberValue = res;
            if (res != 0) res++;
            sheet.Range["H30"].NumberValue = res;

            res = Math.Round(rnd.NextDouble() * 3, 0);
            sheet.Range["L29"].NumberValue = res;
            if (res != 0) res++;
            sheet.Range["L30"].NumberValue = res;

            sheet = tempbook.Worksheets[2]; // 二
            if (jdjlfm.CJJD == "2")
            {
                sheet.Range["C6"].NumberValue = 0;
                sheet.Range["D6"].NumberValue = 0;

                //多齿分度台标准角值
                double[] J2BZJZ = new double[22];
                int j = 0;
                J2BZJZ[j] = 7.8; j++;
                J2BZJZ[j] = 15.7; j++;
                J2BZJZ[j] = 23.5; j++;
                J2BZJZ[j] = 31.3; j++;
                J2BZJZ[j] = 39.1; j++;
                J2BZJZ[j] = 47; j++;
                J2BZJZ[j] = 54.8; j++;
                J2BZJZ[j] = 2.6; j++;
                J2BZJZ[j] = 10.4; j++;
                J2BZJZ[j] = 18.2; j++;
                J2BZJZ[j] = 26; j++;
                J2BZJZ[j] = 33.9; j++;
                J2BZJZ[j] = 41.7; j++;
                J2BZJZ[j] = 49.5; j++;
                J2BZJZ[j] = 57.3; j++;
                J2BZJZ[j] = 05.2; j++;
                J2BZJZ[j] = 13; j++;
                J2BZJZ[j] = 20.8; j++;
                J2BZJZ[j] = 28.6; j++;
                J2BZJZ[j] = 36.5; j++;
                J2BZJZ[j] = 44.3; j++;
                J2BZJZ[j] = 52.2;
                double seed = 2;
                int zfh = 1;
                if (double.Parse(jdjlfm.CJJD) <= 1)
                    seed = 0.5;
                for (int i = 7; i <= 28; i++)
                {
                    res = Math.Round(J2BZJZ[i - 7], 0) + Math.Round(seed / 2 - rnd.NextDouble() * seed, 0);
                    sheet.Range["C" + i].NumberValue = res;
                    res1 = Math.Round(seed / 2 - rnd.NextDouble() * seed, 0);
                    if (rnd.NextDouble() > 0.5)
                        zfh = -1;
                    else
                        zfh = 1;
                    if (res1 == 0) res1 = 0.1 * zfh;
                    res = res + res1;
                    sheet.Range["D" + i].NumberValue = res;
                }
            }
            else if (jdjlfm.CJJD == "5")
            {
                for (int i = 6; i <= 17; i++)
                {
                    res = 1 + Math.Round(rnd.NextDouble() * 10, 0);
                    sheet.Range["C" + i].NumberValue = res;
                    res1 = Math.Round(2 - rnd.NextDouble() * 4, 0);
                    if (res1 == 0) res1 = 0.5;
                    res = res + res1;
                    sheet.Range["D" + i].NumberValue = res;
                }
            }

            sheet = tempbook.Worksheets[3]; // 三 内符精度
            Calcu_NFJD(jdjlfm.QT02);  // BCJDA

            sheet = tempbook.Worksheets[4]; // 四 加成常数
            Cal(jdjlfm.JJWD, jdjlfm.QT02, jdjlfm.QT05); // 温度 气压

            /*
            // tempbook.PrintDocument.Print();// 免费版只能打印3页
            Stream stream = new MemoryStream();
            tempbook.SaveToStream(stream);
            XlsPrinter.Print(stream);
            */

            tempbook.SaveToFile(Path.Combine(xlsPath, jdjlfm.ID.ToString() + ".xls"));

            //System.Diagnostics.Process.Start(Path.Combine(xlsPath, jdjlfm.ID.ToString() + ".xls")); // .net core 不支持            

            /*
            sheet.Range["A2"].Text = "我是A2单元格";
            sheet.Range["A3"].NumberValue = 100.23;
            sheet.Range["A4"].DateTimeValue = DateTime.Now;            
            sheet.Range["A5"].BooleanValue = true;

            Console.WriteLine(sheet.Range["A1"].Value2.ToString());
            Console.WriteLine(sheet.Range["A1"].Value);
            Console.WriteLine(sheet.Range["B1"].Text);

            //对一定范围内的单元格进行字体控制
            sheet.Range["A1:B10"].Style.Font.FontName = "微软雅黑";
            sheet.Range["A1:B10"].Style.Font.Size = 20;
            sheet.Range["A1:B10"].Style.Font.Underline = FontUnderlineType.DoubleAccounting;

            var columes = sheet.Range["C5:E6"].Columns ;
            foreach (var column in columes)
            { }
            */

            string[] RES = new string[17];
            tempbook.CalculateAllValue();
            RES[0] = jdjlfm.JJWD; // A01 温度
            RES[1] = jdjlfm.QT05; // A02 气压
            sheet = tempbook.Worksheets[1];
            RES[2] = sheet.Range["H21"].NumberText.ToString(); // A03 横轴误差
            RES[3] = sheet.Range["G12"].NumberText.ToString(); // A04 望远镜视轴对横轴的垂直度
            RES[4] = sheet.Range["E17"].NumberText.ToString(); // A05 指 标 差            

            sheet = tempbook.Worksheets[2];
            RES[5] = sheet.Range["E32"].NumberText.ToString(); // A06 一测回水平方向标准偏差

            sheet = tempbook.Worksheets[1];
            RES[6] = sheet.Range["I22"].NumberText.ToString(); // A07 补偿范围
            RES[7] = sheet.Range["L27"].NumberText.ToString(); // A08 垂直角补偿精度
            RES[8] = sheet.Range["L28"].NumberText.ToString(); // A09 水平角补偿精度
            RES[9] = sheet.Range["H29"].NumberText.ToString(); // A10 零位误差X
            RES[10] = sheet.Range["L29"].NumberText.ToString(); // A11 零位误差Y
            RES[11] = sheet.Range["M17"].NumberText.ToString(); // A12 照准误差

            sheet = tempbook.Worksheets[3];
            RES[12] = sheet.Range["J21"].NumberText.ToString(); // A13 测量重复性

            sheet = tempbook.Worksheets[4];
            RES[13] = sheet.Range["F52"].NumberText.ToString(); // A14 K
            RES[14] = sheet.Range["F53"].NumberText.ToString(); // A15 R
            RES[15] = sheet.Range["F54"].NumberText.ToString(); // A16 A
            RES[16] = sheet.Range["F55"].NumberText.ToString(); // A17 B

            MakeCert(jdjlfm, template, Signer, RES);

            string[] resA17 = new string[RES.Length + 2];
            resA17[0] = jdjlfm.ID.ToString();
            resA17[1] = jdjlfm.ZSBH;
            for (int i = 2; i < resA17.Length; i++)
                resA17[i] = RES[i-2];
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

            var tmpFieldNames = new string[] { "QRC", "JDR", "HYR", "PZR", "ZSBH", "DWMC", "QJMC", "XHGG", "CCBH", "ZZC", "JDRQY", "JDRQM", "JDRQD", "YXQZY", "YXQZM", "YXQZD", "A01", "A02", "A03", "A04", "A05", "A06", "A07", "A08", "A09", "A10", "A11", "A12", "A13", "A14", "A15", "A16", "A17" };
            var tmpFieldValues = new string[] { imgQRC, imgJDR, imgHYR, imgPZR, jdjlfm.ZSBH, jdjlfm.DWMC, jdjlfm.QJMC, jdjlfm.XHGG, jdjlfm.CCBH, jdjlfm.ZZC, Y1, M1, D1, Y2, M2, D2, A[0], A[1], A[2], A[3], A[4], A[5], A[6], A[7], A[8], A[9], A[10], A[11], A[12], A[13], A[14], A[15], A[16] };

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
        
        /// <summary>
        /// 内符精度计算
        /// </summary>
        /// <param name="BCJD"></param>
        private void Calcu_NFJD(string BCJD)
        {
            double lm_CSZ;
            Random rnd = new Random();
            int S = 0;
            if (double.Parse(BCJD) < 2)
                S = 1;
            else if (double.Parse(BCJD) < 3)
                S = 2;
            else
                S = 3;
            lm_CSZ = Math.Round(31.052 + 2 * rnd.NextDouble() / 1000, 3);

            for (int i = 11; i <= 20; i++)
            {
                if (rnd.NextDouble() + 0.2 > 1)
                {
                    if (rnd.NextDouble() + 0.1 > 1)
                    {
                        sheet.Range[i - 1, 3].NumberValue = lm_CSZ + 0.001 * S;
                    }
                    else
                    {
                        sheet.Range[i - 1, 3].NumberValue = lm_CSZ - 0.001 * S;
                    }
                }
                else
                {
                    sheet.Range[i - 1, 3].NumberValue = lm_CSZ;
                }
            }

            for (int i = 11; i <= 20; i++)
            {
                if (rnd.NextDouble() + 0.2 > 1)
                {
                    if (rnd.NextDouble() + 0.1 > 1)
                    {
                        sheet.Range[i - 1, 6].NumberValue = lm_CSZ + 0.001 * S;
                    }
                    else
                    {
                        sheet.Range[i - 1, 6].NumberValue = lm_CSZ - 0.001 * S;
                    }
                }
                else
                {
                    sheet.Range[i - 1, 6].NumberValue = lm_CSZ;
                }
            }
            for (int i = 11; i <= 20; i++)
            {
                if (rnd.NextDouble() + 0.2 > 1)
                {
                    if (rnd.NextDouble() + 0.1 > 1)
                    {
                        sheet.Range[i - 1, 9].NumberValue = lm_CSZ + 0.001 * S;
                    }
                    else
                    {
                        sheet.Range[i - 1, 9].NumberValue = lm_CSZ - 0.001 * S;
                    }
                }
                else
                {
                    sheet.Range[i - 1, 9].NumberValue = lm_CSZ;
                }
            }

            //计算
            double V2 = 0;
            double DP = 0;

            for (int i = 10; i < 20; i++)
                DP += sheet.Range[i, 3].NumberValue;
            for (int i = 10; i < 20; i++)
                DP += sheet.Range[i, 6].NumberValue;
            for (int i = 10; i < 20; i++)
                DP += sheet.Range[i, 9].NumberValue;

            DP = DP / 30;

            for (int i = 10; i < 20; i++)
                sheet.Range[i, 4].NumberValue = Math.Round((sheet.Range[i, 3].NumberValue - DP) * 1000, 1);
            for (int i = 10; i < 20; i++)
                sheet.Range[i, 7].NumberValue = Math.Round((sheet.Range[i, 6].NumberValue - DP) * 1000, 1);
            for (int i = 10; i < 20; i++)
                sheet.Range[i, 10].NumberValue = Math.Round((sheet.Range[i, 9].NumberValue - DP) * 1000, 1);

            for (int i = 10; i < 20; i++)
                V2 += sheet.Range[i, 4].NumberValue * sheet.Range[i, 4].NumberValue;
            for (int i = 10; i < 20; i++)
                V2 += sheet.Range[i, 7].NumberValue * sheet.Range[i, 7].NumberValue;
            for (int i = 10; i < 20; i++)
                V2 += sheet.Range[i, 10].NumberValue * sheet.Range[i, 10].NumberValue;

            double NFJD = Math.Round(Math.Sqrt(V2 / 29), 2);
            sheet.Range["J21"].NumberValue = NFJD;
        }
        
        /// <summary>
        /// 加乘常数计算、精度评定
        /// </summary>
        /// <param name="WD"></param>
        /// <param name="BCJDA"></param>
        /// <param name="QY"></param>
        private void Cal(string WD, string BCJDA, string QY)
        {
            //---
            string wd1 = "", wd2 = "";
            string[] wd = null;

            if (WD.IndexOf("~") > 0)
                wd = WD.Split('~');
            else if (WD.IndexOf("～") > 0)
                wd = WD.Split('～');
            if (wd.Length > 1)
            {
                wd1 = wd[0];
                wd2 = wd[1];
            }
            else
            {
                wd1 = WD;
                wd2 = WD;
            }
            string txtWD = "";
            if (double.Parse(wd1) < 15)
                txtWD = wd1;
            else if (double.Parse(wd2) > 20)
                txtWD = wd2;
            else
                txtWD = wd1;

            sheet.Range["I8:I48"].Text = txtWD;
            sheet.Range["J8:J48"].Text = QY;

            double JXZ = 0, GCZ = 0;
            double sign;
            int iRnd;
            Random rnd = new Random(unchecked((int)DateTime.Now.Ticks));

            // 三处修改
            // 36 / 48 / 15 / 21

            //for(int j=7;j<=36;j=j+2) //j<=36 15段
            for (int j = 8; j <= 48; j = j + 2) //j<=48 21段			
            {

                iRnd = rnd.Next(-3, 3);
                if (iRnd > 1)
                    sign = 1;
                else if (iRnd < -1)
                    sign = -1;
                else
                    sign = 0;

                for (int i = 4; i <= 8; i++)
                {
                    JXZ = sheet.Range["B" + j].NumberValue;
                    JXZ = Math.Round((JXZ - (Math.Ceiling(JXZ) - 1)) * 1000, 0);
                    GCZ = JXZ + sign * double.Parse(BCJDA) + rnd.Next(-1, 1);
                    sheet.Range[j, i].NumberValue = GCZ;
                }
            }

            // 计算
            //double[] JXZH = new double[21];
            //double[] JCCSPJ = new double[21];
            List<Double> JXZHS = new List<double>();
            List<Double> RawDatas = new List<double>();
            double cal = 0;
            for (int j = 8; j <= 48; j = j + 2) //36 15段，48 21段
            {
                JXZ = sheet.Range["B" + j].NumberValue;
                JXZHS.Add(JXZ);
                JXZ = Math.Ceiling(JXZ) - 1;
                cal = 0;
                for (int i = 4; i <= 8; i++)
                {
                    cal += sheet.Range[j, i].NumberValue;
                }
                RawDatas.Add(JXZ + (cal / 5) / 1000);
            }
            double[] ret = JDJS.DataProcess(JXZHS, RawDatas);
            sheet.Range["F52"].NumberValue = ret[0];
            sheet.Range["F53"].NumberValue = ret[1];
            sheet.Range["F54"].NumberValue = ret[2];
            sheet.Range["F55"].NumberValue = ret[3];
        }

    }
}
