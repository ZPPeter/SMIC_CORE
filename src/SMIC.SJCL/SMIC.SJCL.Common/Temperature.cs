using System;

namespace SMIC.SJCL.Common
{
    public class Temperature
    {
        //public Temperature(ETemperature BZ)
        //{
        //}
        public string GetTemperature(ETemperature BZ)
        {
            Random rnd = new Random();
            double wd1 = 0, wd2 = 0;
            int sMonth = System.DateTime.Now.Month;
            if (sMonth == 12 || sMonth == 1 || sMonth == 2)
            {
                wd1 = Math.Round(1.2 + rnd.NextDouble() * 2, 0);
            }
            else if (sMonth == 3)
            {
                wd1 = Math.Round(14.7 + rnd.NextDouble() * 5, 0);

            }
            else if (sMonth >= 4 && sMonth <= 5)
            {
                wd1 = Math.Round(15.4 + rnd.NextDouble() * 5, 0);
            }
            else if (sMonth == 6)
            {
                wd1 = Math.Round(20.7 + rnd.NextDouble() * 5, 0);

            }
            else if (sMonth >= 7 && sMonth <= 8)
            {
                wd1 = Math.Round(22.7 + rnd.NextDouble() * 5, 0);
            }
            else if (sMonth == 9)
            {
                wd1 = Math.Round(20.7 + rnd.NextDouble() * 5, 0);

            }
            else if (sMonth == 10)
            {
                wd1 = Math.Round(14.7 + rnd.NextDouble() * 5, 0);

            }
            else if (sMonth == 11)
            {
                wd1 = Math.Round(10.2 + rnd.NextDouble() * 5, 0);
            }

            // 室内外 全站仪
            if (BZ == ETemperature.InOutRoom)
            {
                wd2 = Math.Round(20 + rnd.NextDouble() * 5, 0);
                if (wd2 > wd1)
                    return wd1 + "～" + wd2;
                else
                    return wd2 + "～" + wd1;
            }

            // 室内 水准仪
            if (BZ == ETemperature.InRoom)
            {
                wd1 = Math.Round(20 + rnd.NextDouble() * 5, 0);
                return wd1.ToString();
            }

            // 室外 GPS
            if (BZ == ETemperature.OutRoom)
            {
                wd2 = wd1 + Math.Round(3 - rnd.NextDouble() * 6, 0);
                if (wd2 == wd1)
                    wd2 += 2;
                if (wd2 > wd1)
                    return wd1 + "～" + wd2;
                else
                    return wd2 + "～" + wd1;
            }
            return "";
        }

    }

}
