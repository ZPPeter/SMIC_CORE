using System;
using System.Collections.Generic;
using System.Text;

namespace SMIC.SJCL
{
    public abstract class BasePluginsService
    {
        //public interface IDisposable { };
        //public abstract string Handle(RawTemplate template, JDJLFM jdjlfm, int[] Signer);
        public abstract string[] Handle(RawTemplate template, JDJLFM jdjlfm, int[] Signer);
        public abstract string[] Handle(RawTemplate template, JDJLFM jdjlfm);
        public abstract void Handle(string QJMCBM, string ID, int[] Signer);
    }
}
