

using System;
using System.Threading.Tasks;
using Abp;
using Abp.Domain.Services;

namespace SMIC.SDIM.DomainService
{
    public interface IHomeInfoManager : IDomainService
    {

        /// <summary>
        /// 初始化方法
        ///</summary>
        void InitHomeInfo();



		 
      
         

    }
}
