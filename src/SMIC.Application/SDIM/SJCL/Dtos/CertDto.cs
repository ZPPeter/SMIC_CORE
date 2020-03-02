using SMIC.SJCL;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMIC.SDIM.SJCL.Dtos
{
    public class CertDto
    {
        public int[] Signer { get; set; }
        public JDJLFM jdjlfm;
        public RawTemplate rawTemplate;
    }

    public class CertDto2
    {
        public int[] Signer { get; set; }
        public string MBMC { get; set; }
        public string QJMCBM { get; set; }
        public string ID { get; set; }
    }

    public class RejectInput
    {
        public int ID { get; set; }
        public string Info { get; set; }
    }
}
