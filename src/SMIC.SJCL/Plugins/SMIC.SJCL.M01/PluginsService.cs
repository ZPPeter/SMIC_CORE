using System;
using System.IO;
using System.Drawing;
using Spire.Xls;
using Spire.Doc;
using Spire.Doc.Reporting;
using SMIC.SJCL.Common;
using Newtonsoft.Json;
namespace SMIC.SJCL
{
    public class PluginsService : BasePluginsService
    {
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

        private string[] MakeXls(JDJLFM jdjlfm, RawTemplate template)
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
                                                                                                                       // sheet.Range["G14"].DateTimeValue // 无法设置格式?

            sheet.Range["C13"].Text = jdjlfm.JJWD;

            // 原始记录验证码
            sheet.Range["FF1"].Style.Font.Color = Color.White;
            sheet.Range["FF1"].Text = jdjlfm.ID.ToString();

            // 数据处理
            sheet = tempbook.Worksheets[1];
            double baseline_value = sheet.Range["C10"].NumberValue;

            //double res1 = Math.Round(1 + rnd.NextDouble(), 1);
            //double res2 = Math.Round((1 - rnd.NextDouble() * 2) / 10, 1);
            double res = 0;
            var cells = sheet.Range["D10:G10"].CellList;
            foreach (CellRange cell in cells)
            {
                res = Math.Round((baseline_value * 10000 + rnd.Next(-30, 30)) / 10000, 4);
                cell.NumberValue = res;
            }
            baseline_value = sheet.Range["D17"].NumberValue;
            res = Math.Round((baseline_value * 10000 + rnd.Next(-7, 7) * 10) / 10000, 4);
            if (res == baseline_value)
                res = res + 0.0015;
            sheet.Range["F17"].NumberValue = res;

            /*
                // tempbook.PrintDocument.Print();// 免费版只能打印3页
                Stream stream = new MemoryStream();
                tempbook.SaveToStream(stream);
                XlsPrinter.Print(stream);
            */

            tempbook.SaveToFile(Path.Combine(xlsPath, jdjlfm.ID.ToString() + ".xls"));

            string[] RES = new string[5];
            tempbook.CalculateAllValue();
            RES[0] = jdjlfm.JJWD;                              // A01 温度
            RES[2] = sheet.Range["C12"].NumberText.ToString(); // A03 相位误差
            RES[3] = sheet.Range["D17"].NumberText.ToString(); // A04 基线
            RES[4] = sheet.Range["C19"].NumberText.ToString(); // A05 测量误差
            MakeCert(jdjlfm, template, RES);

            string[] resA17 = new string[RES.Length + 2];
            resA17[0] = jdjlfm.ID.ToString();
            resA17[1] = jdjlfm.ZSBH;
            for (int i = 2; i < resA17.Length; i++)
                resA17[i] = RES[i - 2];
            //return JsonConvert.SerializeObject(resA17);
            return resA17;
        }
        private string[] MakeXls(JDJLFM jdjlfm, RawTemplate template, int[] Signer)
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
                                                                                                                       // sheet.Range["G14"].DateTimeValue // 无法设置格式?

            sheet.Range["C13"].Text = jdjlfm.JJWD;

            // 原始记录验证码
            sheet.Range["FF1"].Style.Font.Color = Color.White;
            sheet.Range["FF1"].Text = jdjlfm.ID.ToString();

            // 数据处理
            sheet = tempbook.Worksheets[1];
            double baseline_value = sheet.Range["C10"].NumberValue;

            //double res1 = Math.Round(1 + rnd.NextDouble(), 1);
            //double res2 = Math.Round((1 - rnd.NextDouble() * 2) / 10, 1);
            double res = 0;
            var cells = sheet.Range["D10:G10"].CellList;
            foreach (CellRange cell in cells)
            {
                res = Math.Round((baseline_value * 10000 + rnd.Next(-30, 30)) / 10000, 4);
                cell.NumberValue = res;
            }
            baseline_value = sheet.Range["D17"].NumberValue;
            res = Math.Round((baseline_value * 10000 + rnd.Next(-7, 7)*10) / 10000, 4);
            if (res == baseline_value)
                res = res + 0.0015;
            sheet.Range["F17"].NumberValue = res;

            /*
                // tempbook.PrintDocument.Print();// 免费版只能打印3页
                Stream stream = new MemoryStream();
                tempbook.SaveToStream(stream);
                XlsPrinter.Print(stream);
            */

            tempbook.SaveToFile(Path.Combine(xlsPath, jdjlfm.ID.ToString() + ".xls"));

            string[] RES = new string[5];
            tempbook.CalculateAllValue();
            RES[0] = jdjlfm.JJWD;                              // A01 温度
            RES[2] = sheet.Range["C12"].NumberText.ToString(); // A03 相位误差
            RES[3] = sheet.Range["D17"].NumberText.ToString(); // A04 基线
            RES[4] = sheet.Range["C19"].NumberText.ToString(); // A05 测量误差
            MakeCert(jdjlfm, template, Signer, RES);

            string[] resA17 = new string[RES.Length + 2];
            resA17[0] = jdjlfm.ID.ToString();
            resA17[1] = jdjlfm.ZSBH;
            for (int i = 2; i < resA17.Length; i++)
                resA17[i] = RES[i-2];
            //return JsonConvert.SerializeObject(resA17);
            return resA17;
        }

        private void MakeCert(JDJLFM jdjlfm, RawTemplate template, int[] Signer, string[] A)
        {
            var baseDirectory = Directory.GetCurrentDirectory();
            var imgQRC = Path.Combine(baseDirectory, @"wwwroot\Temp\" + jdjlfm.ID + ".png");
            if (!File.Exists(imgQRC)) { 
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

            //var tmpFieldNames = new string[] { "QRC", "JDR", "HYR", "PZR", "ZSBH", "DWMC", "QJMC", "XHGG", "CCBH", "ZZC" , "JDRQY", "JDRQM", "JDRQD", "YXQZY", "YXQZM", "YXQZD", "A01", "A02", "A03", "A04", "A05", "A06", "A07", "A08", "A09", "A10", "A11", "A12", "A13", "A14", "A15", "A16", "A17" };
            //var tmpFieldValues = new string[] { imgQRC, imgJDR, imgHYR, imgPZR, jdjlfm.ZSBH, jdjlfm.DWMC, jdjlfm.QJMC, jdjlfm.XHGG, jdjlfm.CCBH, jdjlfm.ZZC, Y1, M1, D1, Y2, M2, D2, A[0], A[1], A[2], A[3], A[4], A[5], A[6], A[7], A[8], A[9], A[10], A[11], A[12], A[13], A[14], A[15], A[16] };

            var tmpFieldNames = new string[] { "QRC", "JDR", "HYR", "PZR", "ZSBH", "DWMC", "QJMC", "XHGG", "CCBH", "ZZC", "JDRQY", "JDRQM", "JDRQD", "YXQZY", "YXQZM", "YXQZD", "A01", "A02", "A03", "A04", "A05" };
            var tmpFieldValues = new string[] { imgQRC, imgJDR, imgHYR, imgPZR, jdjlfm.ZSBH, jdjlfm.DWMC, jdjlfm.QJMC, jdjlfm.XHGG, jdjlfm.CCBH, jdjlfm.ZZC, Y1, M1, D1, Y2, M2, D2, A[0], A[1], A[2], A[3], A[4] };

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

            //var tmpFieldNames = new string[] { "QRC", "JDR", "HYR", "PZR", "ZSBH", "DWMC", "QJMC", "XHGG", "CCBH", "ZZC" , "JDRQY", "JDRQM", "JDRQD", "YXQZY", "YXQZM", "YXQZD", "A01", "A02", "A03", "A04", "A05", "A06", "A07", "A08", "A09", "A10", "A11", "A12", "A13", "A14", "A15", "A16", "A17" };
            //var tmpFieldValues = new string[] { imgQRC, imgJDR, imgHYR, imgPZR, jdjlfm.ZSBH, jdjlfm.DWMC, jdjlfm.QJMC, jdjlfm.XHGG, jdjlfm.CCBH, jdjlfm.ZZC, Y1, M1, D1, Y2, M2, D2, A[0], A[1], A[2], A[3], A[4], A[5], A[6], A[7], A[8], A[9], A[10], A[11], A[12], A[13], A[14], A[15], A[16] };

            var tmpFieldNames = new string[] { "QRC", "ZSBH", "DWMC", "QJMC", "XHGG", "CCBH", "ZZC", "JDRQY", "JDRQM", "JDRQD", "YXQZY", "YXQZM", "YXQZD", "A01", "A02", "A03", "A04", "A05" };
            var tmpFieldValues = new string[] { imgQRC, jdjlfm.ZSBH, jdjlfm.DWMC, jdjlfm.QJMC, jdjlfm.XHGG, jdjlfm.CCBH, jdjlfm.ZZC, Y1, M1, D1, Y2, M2, D2, A[0], A[1], A[2], A[3], A[4] };

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
