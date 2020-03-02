using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Spire.Xls;
using Spire.Xls.Core.Spreadsheet;
namespace SMIC.SJCL.Common
{
    /// <summary>
    /// Spire.Xls 免费版只能打印前3页，采用此模块打印全部页面
    /// </summary>
    public class XlsPrinter
    {
        public static void Print(Stream stream) {
            Workbook tempbook = new Workbook();
            tempbook.Version = ExcelVersion.Version2010;
            tempbook.LoadFromStream(stream);
            Workbook newbook = new Workbook();
            newbook.Version = ExcelVersion.Version2010;
            newbook.Worksheets.Clear();
            int j = 0;
            for (int i = 0; i < tempbook.Worksheets.Count; i++) {
                j++;
                newbook.Worksheets.AddCopy(tempbook.Worksheets[i]);
                if (j == 3)
                {
                    newbook.PrintDocument.Print();
                    newbook = new Workbook();
                    newbook.Version = ExcelVersion.Version2010;
                    newbook.Worksheets.Clear();
                    j = 0;
                }                
            }
            if (tempbook.Worksheets.Count % 3 > 0) {
                newbook.PrintDocument.Print();
            }
        }
    }
}
