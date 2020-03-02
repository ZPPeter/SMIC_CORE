
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Abp.Dapper.Repositories;
using Abp.Domain.Repositories;

using System.Collections.Generic;
using System.Linq.Expressions;
using Abp.Specifications;
using System.Reflection;
using SMIC.SJCL;
using System.IO;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using SMIC.SJCL.Common;
using System.Text;

using System.Data;
using System.Linq;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Extensions;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Abp.Dapper.Filters;

using Abp.Timing; // Clock.Now;
using Newtonsoft.Json;
using DapperExtensions;
using Abp.Data;
using Abp.Runtime.Caching;
using SMIC.SDIM.SJCL.Dtos;
using Microsoft.AspNetCore.Identity;
using SMIC.Authorization.Users;
namespace SMIC.SDIM.SJCL
{
    public class CertAppServices : SMICAppServiceBase
    {
        private readonly IDapperRepository<SJMX_ZSBH> _sjmxDapperRepository;
        private readonly IDapperRepository<ZSH_DATA> _zshDapperRepository;
        private readonly IMemoryCache _cache;
        private readonly PluginsOptions _options;
        private readonly UserManager _userManager;
        //IRepository<User, long> repository,

        public CertAppServices(
            IDapperRepository<SJMX_ZSBH> sjmxDapperRepository,
            IDapperRepository<ZSH_DATA> zshDapperRepository,
            IMemoryCache cache,
            IOptions<PluginsOptions> optionsAccessor,
            UserManager userManager
            )
        {
            _sjmxDapperRepository = sjmxDapperRepository;
            _zshDapperRepository = zshDapperRepository;
            _cache = cache;
            _options = optionsAccessor.Value;
            _userManager = userManager;
        }

        /// <summary>
        /// 取证书编号
        /// LB
        /// M01 GPS
        /// M02 全站仪
        /// </summary>
        /// <param name="LB"></param>
        /// <returns></returns>
        private string GetZSBH(string LB, double ID)
        //public dynamic GetZSBH(string LB, double ID)
        {
            string strSQL1 = @"
            Update YQSF_SJMX
            set jdzt='在检' 
            where ID=" + ID;
            _sjmxDapperRepository.Execute(strSQL1);

            StringBuilder strSQL = new StringBuilder(100);
            strSQL.Append("declare @SQL VARCHAR(255)");
            strSQL.Append("\r\n");
            strSQL.Append("DECLARE @Year decimal");
            strSQL.Append("\r\n");
            strSQL.Append("DECLARE @Zsbh VARCHAR(12)");
            strSQL.Append("\r\n");
            strSQL.Append("set @Year = (select LEFT(Right(MAX(zsbh), 8), 4) from YQSF_SJMX where zsbh like '" + LB + "-%')");
            strSQL.Append("\r\n");
            strSQL.Append("if (year(getdate()) > @Year) ");
            strSQL.Append("\r\n");
            strSQL.Append("set @Zsbh = '" + LB + "-" + DateTime.Now.Year + "0001'");
            strSQL.Append("\r\n");
            strSQL.Append("else");
            strSQL.Append("\r\n");
            strSQL.Append("set @Zsbh = '" + LB + "-'+str((select Right(MAX(zsbh),8)+1 from YQSF_SJMX where zsbh like '" + LB + "-%'),8,0)");
            strSQL.Append("\r\n");
            strSQL.Append("update yqsf_sjmx set zsbh=@Zsbh where ID=" + ID + "  and len(zsbh)=0");
            strSQL.Append("\r\n");
            strSQL.Append("select zsbh from YQSF_SJMX where ID=" + ID);
            return _sjmxDapperRepository.Query(strSQL.ToString()).FirstOrDefault().ZSBH;
        }

        private void AddtoZshData(int ID, string Data)
        {
            long jdyid = (long)AbpSession.UserId;
            //jdzt = 111
            //string userName = AbpSession.GetUserName();//获取当前登录用户 surName
            var param = new
            {
                ID = ID,
                Data = Data,
                JDRQ = DateTime.Now,
                JDYID = jdyid
            };
            string strSQL = @"delete from ZSH_Data where ID=@ID 
                              insert into ZSH_Data values(@ID,@Data)
                              update SJCL_CHYQ set JDRQ = @JDRQ,JDZT = 111,JDYID = @JDYID where ID=@ID";
            int ret = _sjmxDapperRepository.Execute(strSQL, param);
            // ToDo 主页数据刷新
        }

        /// <summary>
        /// 生成原始记录证书 并 签名
        /// </summary>
        /// <returns></returns>
        public async Task<string[]> MakeCert(CertDto ipt)
        //public async Task<string[]> MakeCert(JDJLFM jdjlfm, RawTemplate rawTemplate, int[] Signer)        
        {
            if (string.IsNullOrWhiteSpace(ipt.rawTemplate.MBMC))
            {
                return null;
            }
            var plugin = await PluginFactory.GetPlugin(_cache, _options, ipt.rawTemplate.MBMC);
            if (plugin != null)
            {
                var zsbh = GetZSBH(ipt.rawTemplate.MBMC, ipt.jdjlfm.ID);
                ipt.jdjlfm.ZSBH = zsbh;
                string[] ret = plugin.Handle(ipt.rawTemplate, ipt.jdjlfm, ipt.Signer);
                string resData = JsonConvert.SerializeObject(ret);
                AddtoZshData(ipt.jdjlfm.ID, resData);
                return ret;
            }
            return null;
        }

        /// <summary>
        /// 生成原始记录证书 不 签名
        /// </summary>
        /// <param name="ipt"></param>
        /// <returns></returns>
        public async Task<string[]> MakeXlsCert(CertDto ipt)
        //public async Task<string[]> MakeCert(JDJLFM jdjlfm, RawTemplate rawTemplate, int[] Signer)        
        {
            if (string.IsNullOrWhiteSpace(ipt.rawTemplate.MBMC))
            {
                return null;
            }

            //PerformanceCounterTest test = new PerformanceCounterTest();
            //test.Go();
            var plugin = await PluginFactory.GetPlugin(_cache, _options, ipt.rawTemplate.MBMC);
            if (plugin != null)
            {
                var zsbh = GetZSBH(ipt.rawTemplate.MBMC, ipt.jdjlfm.ID);
                ipt.jdjlfm.ZSBH = zsbh;
                //string[] ret = plugin.Handle(ipt.rawTemplate, ipt.jdjlfm, ipt.Signer);
                string[] ret = plugin.Handle(ipt.rawTemplate, ipt.jdjlfm);
                string resData = JsonConvert.SerializeObject(ret);
                AddtoZshData(ipt.jdjlfm.ID, resData);
                //test.Go();
                return ret;
            }

            return null;
        }

        private async Task<string> SignerCert(CertDto2 ipt)
        {
            var plugin = await PluginFactory.GetPlugin(_cache, _options, ipt.MBMC);
            if (plugin != null)
            {
                plugin.Handle(ipt.QJMCBM, ipt.ID, ipt.Signer);
                return "OK";
            }
            return null;
        }

        public string ShowResults(int ID)
        {
            string strSQL = @"select Data from ZSH_Data where ID=@ID";
            var param = new
            {
                ID = ID
            };
            return _zshDapperRepository.Query(strSQL, param).FirstOrDefault().Data;
        }

        public int SetJDWB(int ID)
        {
            string strSQL = @"update SJCL_CHYQ set JWRQ = @JWRQ,JDZT = 122 where ID=@ID";
            var param = new
            {
                ID = ID,
                JWRQ = DateTime.Now
            };
            return _zshDapperRepository.Execute(strSQL, param);
        }

        public async Task<int> SetHYWB(int ID)
        {
            long jdyid = (long)AbpSession.UserId;
            var user = await _userManager.GetUserByIdAsync(jdyid);

            string strSQL = @"update SJCL_CHYQ set HYYJ = @HYYJ,HYYID=@HYYID,HYY=@HYY,JDZT = 200 where ID=@ID";
            var param = new
            {
                ID = ID,
                HYYID = jdyid,
                HYY = user.Surname,
                HYYJ = DateTime.Now.ToString()
            };
            return _zshDapperRepository.Execute(strSQL, param);
        }

        // 核验驳回
        public async Task<int> SetReject(RejectInput input)
        {
            long jdyid = (long)AbpSession.UserId;
            var user = await _userManager.GetUserByIdAsync(jdyid);

            string strSQL = @"update SJCL_CHYQ set HYYJ = @HYYJ,HYYID=@HYYID,HYY=@HYY,JDZT = 111 where ID=@ID";
            var param = new
            {
                ID = input.ID,
                HYYID = jdyid,
                HYY = user.Surname,
                HYYJ = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ";" + input.Info
            };
            return _zshDapperRepository.Execute(strSQL, param);
        }

        // 批准驳回
        public async Task<int> SetApproveReject(RejectInput input)
        {
            long jdyid = (long)AbpSession.UserId;
            var user = await _userManager.GetUserByIdAsync(jdyid);

            string strSQL = @"update SJCL_CHYQ set PZYJ = @PZYJ,PZRID=@PZRID,PZR=@PZR,JDZT = 111 where ID=@ID";
            var param = new
            {
                ID = input.ID,
                PZRID = jdyid,
                PZR = user.Surname,
                PZYJ = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ";" + input.Info
            };
            return _zshDapperRepository.Execute(strSQL, param);
        }

        public async Task<int> SetPZWB(double ID)
        {
            long jdyid = (long)AbpSession.UserId;
            var user = await _userManager.GetUserByIdAsync(jdyid);

            string strSQL = @"
            Update YQSF_SJMX
            set jdzt='检完' 
            where ID=" + ID;
            _zshDapperRepository.Execute(strSQL);

            strSQL = @"
            Update YQSF_SJD 
            set jdzt='检完' 
            where yqjs=(select count(*) from YQSF_SJMX where jdzt='检完' and SJDID=YQSF_SJD.id)";
            _zshDapperRepository.Execute(strSQL);

            // 证书签名
            // await SignerCert(ipt);

            strSQL = @"update SJCL_CHYQ set PZRID=@PZRID,PZR=@PZR,PZYJ=@PZYJ,JDZT = 222 where ID=@ID";
            var param = new
            {
                ID = ID,
                PZRID = jdyid,
                PZR = user.Surname,
                PZYJ = DateTime.Now.ToString()
            };
            return _zshDapperRepository.Execute(strSQL, param);
        }

        /// <summary>
        /// 全部批准
        /// </summary>
        /// <returns></returns>
        public async Task<int> SetQBWB()
        {
            long jdyid = (long)AbpSession.UserId;
            var user = await _userManager.GetUserByIdAsync(jdyid);

            string strSQL = @"
            Update YQSF_SJMX
            set jdzt='检完' 
            where ID in (select ID from SJCL_CHYQ where JDZT=200)";
            _zshDapperRepository.Execute(strSQL);

            strSQL = @"
            Update YQSF_SJD 
            set jdzt='检完' 
            where yqjs=(select count(*) from YQSF_SJMX where jdzt='检完' and SJDID=YQSF_SJD.id)";
            _zshDapperRepository.Execute(strSQL);

            strSQL = @"update SJCL_CHYQ set PZRID=@PZRID,PZR=@PZR,PZYJ='ALL:'+@PZYJ,JDZT = 222 where JDZT=200";
            var param = new
            {
                PZRID = jdyid,
                PZR = user.Surname,
                PZYJ = DateTime.Now.ToString()
            };
            return _zshDapperRepository.Execute(strSQL, param);
        }
    }

    public class PerformanceCounterTest
    {
        /*
         直接使用PerformanceCounter就行了，这个DLL已经可以在.NET Core 3.0和3.1上使用
        */
        public void Go()
        {
            Process[] p = Process.GetProcesses();//获取进程信息
            Int64 totalMem = 0;
            string info = "";
            foreach (Process pr in p)
            {
                totalMem += pr.WorkingSet64 / 1024;
                info += pr.ProcessName + "内存：-----------" + (pr.WorkingSet64 / 1024).ToString() + "KB\r\n";//得到进程内存
            }
            Debug.WriteLine(info);
            /*            
                            Debug.WriteLine("总内存totalmem:" + totalMem / 1024 + "M");
                            Debug.WriteLine("判断是否为Windows Linux OSX");
                            Debug.WriteLine($"Linux:{RuntimeInformation.IsOSPlatform(OSPlatform.Linux)}");
                            Debug.WriteLine($"OSX:{RuntimeInformation.IsOSPlatform(OSPlatform.OSX)}");
                            Debug.WriteLine($"Windows:{RuntimeInformation.IsOSPlatform(OSPlatform.Windows)}");
                            Debug.WriteLine($"系统架构：{RuntimeInformation.OSArchitecture}");
                            Debug.WriteLine($"系统名称：{RuntimeInformation.OSDescription}");
                            Debug.WriteLine($"进程架构：{RuntimeInformation.ProcessArchitecture}");
                            Debug.WriteLine($"是否64位操作系统：{Environment.Is64BitOperatingSystem}");
                            Debug.WriteLine("CPU CORE:" + Environment.ProcessorCount);
                            Debug.WriteLine("HostName:" + Environment.MachineName);
                            Debug.WriteLine("Version:" + Environment.OSVersion);

                            Debug.WriteLine("内存相关的:" + Environment.WorkingSet);
                            string[] LogicalDrives = Environment.GetLogicalDrives();
                            for (int i = 0; i < LogicalDrives.Length; i++)
                            {
                                Debug.WriteLine("驱动:" + LogicalDrives[i]);
                            }
                            // Debug.ReadLine();

                        // top 是 Linux 的命令
                            //创建一个ProcessStartInfo对象 使用系统shell 指定命令和参数 设置标准输出
                            var psi = new ProcessStartInfo("top", " -b -n 1") { RedirectStandardOutput = true };
                            //启动
                            var proc = Process.Start(psi);

                            //   psi = new ProcessStartInfo("", "1") { RedirectStandardOutput = true };
                            //启动
                            // proc = Process.Start(psi);

                            if (proc == null)
                            {
                                Debug.WriteLine("Can not exec.");
                            }
                            else
                            {
                                Debug.WriteLine("-------------Start read standard output-------cagy-------");
                                //开始读取
                                using (var sr = proc.StandardOutput)
                                {
                                    while (!sr.EndOfStream)
                                    {
                                        Debug.WriteLine(sr.ReadLine());
                                    }

                                    if (!proc.HasExited)
                                    {
                                        proc.Kill();
                                    }
                                }
                                Debug.WriteLine("---------------Read end-----------cagy-------");
                                Debug.WriteLine($"Total execute time :{(proc.ExitTime - proc.StartTime).TotalMilliseconds} ms");
                                Debug.WriteLine($"Exited Code ： {proc.ExitCode}");
                            }
                            */
        }
    }
}

