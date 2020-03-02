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
            var MBJD = jdjlfm.CJJD; // 2 6

            var xlsTempPath = Path.Combine(baseDirectory, @"wwwroot\Templates\" + template.MBMC + "-" + MBJD + ".xls");  // 模板

            //var xlsTempPath = Path.Combine(baseDirectory, @"wwwroot\Templates\" + template.MBMC + "-" + jdjlfm.CJJD + ".xls"); // 模板

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
                                                                                                                       // sheet.Range["G14"].DateTimeValue // 无法设置格式?

            sheet.Range["C13"].Text = jdjlfm.JJWD;

            //原始记录验证码
            sheet.Range["FF1"].Style.Font.Color = Color.White;
            sheet.Range["FF1"].Text = jdjlfm.ID.ToString();

            // ToDo 数据处理
            sheet = tempbook.Worksheets[1]; // 一
            // 数据 - 视准轴竖轴垂直度
            double res = Math.Round(1 + 0.5 - rnd.NextDouble(), 1); // 1±.5
            double res1 = 0;
            sheet.Range["I10"].NumberValue = res;
            res1 = Math.Round((3 - rnd.NextDouble() * 6) / 10, 1);
            if (res1 == 0) res1 = 0.1;
            res = res + res1;
            sheet.Range["I11"].NumberValue = res;

            // 数据 - 横轴竖轴垂直度
            res = Math.Round(1 + 0.5 - rnd.NextDouble(), 1); // 1±.5
            sheet.Range["I13"].NumberValue = res;
            res1 = Math.Round((3 - rnd.NextDouble() * 6) / 10, 1);
            if (res1 == 0) res1 = 0.1;
            res = res + res1;
            sheet.Range["I14"].NumberValue = res;

            // 数据 - 指标差
            sheet.Range["F16"].NumberValue = 89;
            sheet.Range["H16"].NumberValue = 59;
            sheet.Range["F17"].NumberValue = 269;
            sheet.Range["H17"].NumberValue = 59;

            res = Math.Round(57 + 2 - rnd.NextDouble() * 4, 0);
            sheet.Range["J16"].NumberValue = res;
            res = Math.Round(54 + 2 - rnd.NextDouble() * 4, 0);
            sheet.Range["J17"].NumberValue = res;


            // 数据 - 竖盘指标补偿误差
            sheet.Range["F21"].NumberValue = 90;
            sheet.Range["F22"].NumberValue = 90;
            sheet.Range["F23"].NumberValue = 90;
            sheet.Range["F24"].NumberValue = 90;
            sheet.Range["F25"].NumberValue = 90;
            sheet.Range["H21"].NumberValue = 0;
            sheet.Range["H22"].NumberValue = 0;
            sheet.Range["H23"].NumberValue = 0;
            sheet.Range["H24"].NumberValue = 0;
            sheet.Range["H25"].NumberValue = 0;

            if (jdjlfm.CJJD == "6")  // <12
            {
                res = rnd.NextDouble() >= 0.5 ? 6 : 0;
                sheet.Range["J21"].NumberValue = res;
                res = rnd.NextDouble() >= 0.5 ? 6 : 0;
                sheet.Range["J22"].NumberValue = res;
                res = rnd.NextDouble() >= 0.5 ? 6 : 0;
                sheet.Range["J23"].NumberValue = res;
                res = rnd.NextDouble() >= 0.5 ? 6 : 0;
                sheet.Range["J24"].NumberValue = res;
                res = rnd.NextDouble() >= 0.5 ? 6 : 0;
                sheet.Range["J25"].NumberValue = res;
            }
            else
            {   // <6
                res = Math.Round(4 - rnd.NextDouble() * 4, 0);
                sheet.Range["J21"].NumberValue = res;
                res = Math.Round(4 - rnd.NextDouble() * 4, 0);
                sheet.Range["J22"].NumberValue = res;
                res = Math.Round(4 - rnd.NextDouble() * 4, 0);
                sheet.Range["J23"].NumberValue = res;
                res = Math.Round(4 - rnd.NextDouble() * 4, 0);
                sheet.Range["J24"].NumberValue = res;
                res = Math.Round(4 - rnd.NextDouble() * 4, 0);
                sheet.Range["J25"].NumberValue = res;
            }

            sheet = tempbook.Worksheets[2]; // 二
            if (jdjlfm.CJJD == "2")
            {
                for (int i = 5; i < 10; i++)
                {
                    if (i != 7)
                    {
                        res = Math.Round(1 + rnd.NextDouble() * 10, 0);
                        sheet.Range["I" + i].NumberValue = res;
                        res = Math.Round(1 + rnd.NextDouble() * 10, 0);
                        sheet.Range["O" + i].NumberValue = res;
                        res = Math.Round(3 + rnd.NextDouble() * 10, 0);
                        sheet.Range["U" + i].NumberValue = res;
                        res = Math.Round(3 + rnd.NextDouble() * 10, 0);
                        sheet.Range["AA" + i].NumberValue = res;
                        res = Math.Round(3 + rnd.NextDouble() * 10, 0);
                        sheet.Range["AG" + i].NumberValue = res;
                    }
                }
            }
            else
            {
                for (int i = 5; i < 10; i++)
                {
                    if (i != 7)
                    {
                        res = Math.Round(rnd.NextDouble() * 3 * 6, 0);
                        sheet.Range["I" + i].NumberValue = res;
                        res = Math.Round(rnd.NextDouble() * 3 * 6, 0);
                        sheet.Range["O" + i].NumberValue = res;
                        res = Math.Round(rnd.NextDouble() * 3 * 6, 0);
                        sheet.Range["U" + i].NumberValue = res;
                        res = Math.Round(rnd.NextDouble() * 3 * 6, 0);
                        sheet.Range["AA" + i].NumberValue = res;
                        res = Math.Round(rnd.NextDouble() * 3 * 6, 0);
                        sheet.Range["AG" + i].NumberValue = res;
                    }
                }
            }
            sheet = tempbook.Worksheets[3]; // 三
            if (jdjlfm.CJJD == "2")
            {
                sheet.Range["G7"].NumberValue = Math.Round(rnd.NextDouble() * 4, 0);
                sheet.Range["J7"].NumberValue = Math.Round(rnd.NextDouble() * 6, 0);
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
                for (int i = 8; i <= 29; i++)
                {
                    res = Math.Round(J2BZJZ[i - 8], 0) + Math.Round(1.5 - rnd.NextDouble() * 3, 0);
                    sheet.Range["G" + i].NumberValue = res;
                    res = res + Math.Round(1 - rnd.NextDouble() * 3, 0);
                    sheet.Range["J" + i].NumberValue = res;
                }
            }
            else
            {
                for (int i = 6; i <= 17; i++)
                {
                    res = 1 + Math.Round(rnd.NextDouble() * 10, 0);
                    sheet.Range["C" + i].NumberValue = res;
                    res = res + Math.Round(2 - rnd.NextDouble() * 4, 0);
                    if (res == 0) res = 2;
                    sheet.Range["D" + i].NumberValue = res;
                }
            }

            /*
                // tempbook.PrintDocument.Print();// 免费版只能打印3页
                Stream stream = new MemoryStream();
                tempbook.SaveToStream(stream);
                XlsPrinter.Print(stream);
            */

            tempbook.SaveToFile(Path.Combine(xlsPath, jdjlfm.ID.ToString() + ".xls"));

            string[] RES = new string[8];
            tempbook.CalculateAllValue();
            RES[0] = jdjlfm.JJWD; // A01 温度
            RES[1] = "";          // A02 未用 
            sheet = tempbook.Worksheets[1];
            RES[2] = sheet.Range["F12"].NumberText.ToString(); // A03 视准轴与横轴的垂直度
            RES[3] = sheet.Range["G15"].NumberText.ToString(); // A04 横轴与竖轴的垂直度
            RES[4] = sheet.Range["H18"].NumberText.ToString(); // A05 竖盘指标差            

            sheet = tempbook.Worksheets[3];
            if (jdjlfm.CJJD == "2")
                RES[5] = sheet.Range["L35"].NumberText.ToString(); // A06 一测回水平方向标准偏差
            else
                RES[5] = sheet.Range["F32"].NumberText.ToString(); // A06 一测回水平方向标准偏差            

            sheet = tempbook.Worksheets[2];
            RES[6] = sheet.Range["D13"].NumberText.ToString(); // A07 望远镜调焦运行误差

            sheet = tempbook.Worksheets[1];
            RES[7] = sheet.Range["J26"].NumberText.ToString(); // A08 竖盘指标自动补偿误差

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

            var tmpFieldNames = new string[] { "QRC", "ZSBH", "DWMC", "QJMC", "XHGG", "CCBH", "ZZC", "JDRQY", "JDRQM", "JDRQD", "YXQZY", "YXQZM", "YXQZD", "A01", "A02", "A03", "A04", "A05", "A06", "A07", "A08" };
            var tmpFieldValues = new string[] { imgQRC, jdjlfm.ZSBH, jdjlfm.DWMC, jdjlfm.QJMC, jdjlfm.XHGG, jdjlfm.CCBH, jdjlfm.ZZC, Y1, M1, D1, Y2, M2, D2, A[0], A[1], A[2], A[3], A[4], A[5], A[6], A[7] };

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
            var MBJD = jdjlfm.CJJD; // 2 6

            var xlsTempPath = Path.Combine(baseDirectory, @"wwwroot\Templates\" + template.MBMC + "-" + MBJD + ".xls");  // 模板

            //var xlsTempPath = Path.Combine(baseDirectory, @"wwwroot\Templates\" + template.MBMC + "-" + jdjlfm.CJJD + ".xls"); // 模板

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
                                                                                                                       // sheet.Range["G14"].DateTimeValue // 无法设置格式?

            sheet.Range["C13"].Text = jdjlfm.JJWD;

            //原始记录验证码
            sheet.Range["FF1"].Style.Font.Color = Color.White;
            sheet.Range["FF1"].Text = jdjlfm.ID.ToString();

            // ToDo 数据处理
            sheet = tempbook.Worksheets[1]; // 一
            // 数据 - 视准轴竖轴垂直度
            double res = Math.Round(1 + 0.5 - rnd.NextDouble(), 1); // 1±.5
            double res1 = 0;
            sheet.Range["I10"].NumberValue = res;
            res1 = Math.Round((3 - rnd.NextDouble() * 6) / 10, 1);
            if (res1 == 0) res1 = 0.1;
            res = res + res1;
            sheet.Range["I11"].NumberValue = res;

            // 数据 - 横轴竖轴垂直度
            res = Math.Round(1 + 0.5 - rnd.NextDouble(), 1); // 1±.5
            sheet.Range["I13"].NumberValue = res;
            res1 = Math.Round((3 - rnd.NextDouble() * 6) / 10, 1);
            if (res1 == 0) res1 = 0.1;
            res = res + res1;
            sheet.Range["I14"].NumberValue = res;

            // 数据 - 指标差
            sheet.Range["F16"].NumberValue = 89;
            sheet.Range["H16"].NumberValue = 59;
            sheet.Range["F17"].NumberValue = 269;
            sheet.Range["H17"].NumberValue = 59;

            res = Math.Round(57 + 2 - rnd.NextDouble() * 4, 0);
            sheet.Range["J16"].NumberValue = res;
            res = Math.Round(54 + 2 - rnd.NextDouble() * 4, 0);
            sheet.Range["J17"].NumberValue = res;


            // 数据 - 竖盘指标补偿误差
            sheet.Range["F21"].NumberValue = 90;
            sheet.Range["F22"].NumberValue = 90;
            sheet.Range["F23"].NumberValue = 90;
            sheet.Range["F24"].NumberValue = 90;
            sheet.Range["F25"].NumberValue = 90;
            sheet.Range["H21"].NumberValue = 0;
            sheet.Range["H22"].NumberValue = 0;
            sheet.Range["H23"].NumberValue = 0;
            sheet.Range["H24"].NumberValue = 0;
            sheet.Range["H25"].NumberValue = 0;

            if (jdjlfm.CJJD == "6")  // <12
            {
                res = rnd.NextDouble() >= 0.5 ? 6 : 0;
                sheet.Range["J21"].NumberValue = res;
                res = rnd.NextDouble() >= 0.5 ? 6 : 0;
                sheet.Range["J22"].NumberValue = res;
                res = rnd.NextDouble() >= 0.5 ? 6 : 0;
                sheet.Range["J23"].NumberValue = res;
                res = rnd.NextDouble() >= 0.5 ? 6 : 0;
                sheet.Range["J24"].NumberValue = res;
                res = rnd.NextDouble() >= 0.5 ? 6 : 0;
                sheet.Range["J25"].NumberValue = res;
            }
            else
            {   // <6
                res = Math.Round(4 - rnd.NextDouble() * 4, 0);
                sheet.Range["J21"].NumberValue = res;
                res = Math.Round(4 - rnd.NextDouble() * 4, 0);
                sheet.Range["J22"].NumberValue = res;
                res = Math.Round(4 - rnd.NextDouble() * 4, 0);
                sheet.Range["J23"].NumberValue = res;
                res = Math.Round(4 - rnd.NextDouble() * 4, 0);
                sheet.Range["J24"].NumberValue = res;
                res = Math.Round(4 - rnd.NextDouble() * 4, 0);
                sheet.Range["J25"].NumberValue = res;
            }

            sheet = tempbook.Worksheets[2]; // 二
            if (jdjlfm.CJJD == "2")
            {
                for (int i = 5; i < 10; i++)
                {
                    if (i != 7)
                    {
                        res = Math.Round(1 + rnd.NextDouble() * 10, 0);
                        sheet.Range["I" + i].NumberValue = res;
                        res = Math.Round(1 + rnd.NextDouble() * 10, 0);
                        sheet.Range["O" + i].NumberValue = res;
                        res = Math.Round(3 + rnd.NextDouble() * 10, 0);
                        sheet.Range["U" + i].NumberValue = res;
                        res = Math.Round(3 + rnd.NextDouble() * 10, 0);
                        sheet.Range["AA" + i].NumberValue = res;
                        res = Math.Round(3 + rnd.NextDouble() * 10, 0);
                        sheet.Range["AG" + i].NumberValue = res;
                    }
                }
            }
            else
            {
                for (int i = 5; i < 10; i++)
                {
                    if (i != 7)
                    {
                        res = Math.Round(rnd.NextDouble() * 3 * 6, 0);
                        sheet.Range["I" + i].NumberValue = res;
                        res = Math.Round(rnd.NextDouble() * 3 * 6, 0);
                        sheet.Range["O" + i].NumberValue = res;
                        res = Math.Round(rnd.NextDouble() * 3 * 6, 0);
                        sheet.Range["U" + i].NumberValue = res;
                        res = Math.Round(rnd.NextDouble() * 3 * 6, 0);
                        sheet.Range["AA" + i].NumberValue = res;
                        res = Math.Round(rnd.NextDouble() * 3 * 6, 0);
                        sheet.Range["AG" + i].NumberValue = res;
                    }
                }
            }
            sheet = tempbook.Worksheets[3]; // 三
            if (jdjlfm.CJJD == "2")
            {
                sheet.Range["G7"].NumberValue = Math.Round(rnd.NextDouble() * 4, 0);
                sheet.Range["J7"].NumberValue = Math.Round(rnd.NextDouble() * 6, 0);
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
                for (int i = 8; i <= 29; i++)
                {
                    res = Math.Round(J2BZJZ[i - 8], 0) + Math.Round(1.5 - rnd.NextDouble() * 3, 0);
                    sheet.Range["G" + i].NumberValue = res;
                    res = res + Math.Round(1 - rnd.NextDouble() * 3, 0);
                    sheet.Range["J" + i].NumberValue = res;
                }
            }
            else
            {
                for (int i = 6; i <= 17; i++)
                {
                    res = 1 + Math.Round(rnd.NextDouble() * 10, 0);
                    sheet.Range["C" + i].NumberValue = res;
                    res = res + Math.Round(2 - rnd.NextDouble() * 4, 0);
                    if (res == 0) res = 2;
                    sheet.Range["D" + i].NumberValue = res;
                }
            }

            /*
                // tempbook.PrintDocument.Print();// 免费版只能打印3页
                Stream stream = new MemoryStream();
                tempbook.SaveToStream(stream);
                XlsPrinter.Print(stream);
            */

            tempbook.SaveToFile(Path.Combine(xlsPath, jdjlfm.ID.ToString() + ".xls"));

            string[] RES = new string[8];
            tempbook.CalculateAllValue();
            RES[0] = jdjlfm.JJWD; // A01 温度
            RES[1] = "";          // A02 未用 
            sheet = tempbook.Worksheets[1];
            RES[2] = sheet.Range["F12"].NumberText.ToString(); // A03 视准轴与横轴的垂直度
            RES[3] = sheet.Range["G15"].NumberText.ToString(); // A04 横轴与竖轴的垂直度
            RES[4] = sheet.Range["H18"].NumberText.ToString(); // A05 竖盘指标差            

            sheet = tempbook.Worksheets[3];
            if (jdjlfm.CJJD == "2")
                RES[5] = sheet.Range["L35"].NumberText.ToString(); // A06 一测回水平方向标准偏差
            else
                RES[5] = sheet.Range["F32"].NumberText.ToString(); // A06 一测回水平方向标准偏差            

            sheet = tempbook.Worksheets[2];
            RES[6] = sheet.Range["D13"].NumberText.ToString(); // A07 望远镜调焦运行误差

            sheet = tempbook.Worksheets[1];
            RES[7] = sheet.Range["J26"].NumberText.ToString(); // A08 竖盘指标自动补偿误差

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

            var tmpFieldNames = new string[] { "QRC", "JDR", "HYR", "PZR", "ZSBH", "DWMC", "QJMC", "XHGG", "CCBH", "ZZC", "JDRQY", "JDRQM", "JDRQD", "YXQZY", "YXQZM", "YXQZD", "A01", "A02", "A03", "A04", "A05", "A06", "A07", "A08" };
            var tmpFieldValues = new string[] { imgQRC, imgJDR, imgHYR, imgPZR, jdjlfm.ZSBH, jdjlfm.DWMC, jdjlfm.QJMC, jdjlfm.XHGG, jdjlfm.CCBH, jdjlfm.ZZC, Y1, M1, D1, Y2, M2, D2, A[0], A[1], A[2], A[3], A[4], A[5], A[6], A[7] };

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