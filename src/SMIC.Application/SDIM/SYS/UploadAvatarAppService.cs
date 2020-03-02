using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using Abp.Dapper.Repositories;
using System.Linq;
using System.Data;
using DapperExtensions;
using DapperExtensions.Mapper;
using System.Reflection;
using System.Data.SqlClient;
using Dapper;
using DapperExtensions.Sql;
using SMIC;
using Microsoft.Extensions.Configuration;
using SMIC.Configuration;
using SMIC.Web;

using Abp.Auditing;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Abp.Runtime.Session;
using Abp.UI;
using SMIC.Common.Dto;
using System;
using System.IO;

//using SixLabors.ImageSharp;
//using SixLabors.ImageSharp.Processing;
//using SixLabors.ImageSharp.PixelFormats;
using Microsoft.AspNetCore.Mvc;
//using Abp.Authorization;
//using SMIC.Authorization;

namespace SMIC.Common
{
    //[AbpAuthorize(PermissionNames.Pages_ChangeAvatar)]
    public class UploadAvatarAppService : SMICAppServiceBase
    {
        private readonly IAbpSession _abpSession;
        public UploadAvatarAppService(IAbpSession abpSession)
        {
            _abpSession = abpSession;
        }             

        public void UploadFile1([FromBody]UploadAvatarDto input)
        {
            Logger.Error("Upload: ... ...");
            if (_abpSession.UserId == null)  // 此处错误，怎么取用户 ID？
            {
                throw new UserFriendlyException("请登录后再进行操作！");
            }
            long userId = _abpSession.UserId.Value;

            var stream = input.File.OpenReadStream();
            var filename = "logo_" + userId + ".png";
            var path = $"{filename.Substring(0, 2)}/{filename.Substring(2, 2)}";
            var dir = $"{Environment.CurrentDirectory}\\wwwroot\\upload\\{path}";

            Logger.Error("Dir:"+dir);

            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            using (var fileStream = new FileStream($"{dir}\\{filename}", FileMode.Create))
            {
                //await input.File.CopyToAsync(fileStream);
                input.File.CopyToAsync(fileStream);
            }
            /*
            using (var image = Image.Load<Rgba32>($"{dir}\\{filename}"))
            {
                image.Mutate(x => x
                    .Resize(160, 160));
                image.Save($"{dir}\\thumbnail_{filename}");
            }*/
        }
                
        public string UploadFile(IFormFile file)        
        {
            try
            {
                //var ddf = long.Parse(Request.Headers["token"]);
                //if ((ddf & 178) == 178)
                {
                    //定义图片数组后缀格式
                    //string[] LimitPictureType = { ".JPG", ".JPEG", ".GIF", ".PNG", ".BMP" };
                    //获取图片后缀是否存在数组中
                    //string currentPictureExtension = Path.GetExtension(input.File.FileName).ToUpper();
                    //if (LimitPictureType.Contains(currentPictureExtension))
                    //{
                    //为了查看图片就不在重新生成文件名称了

                    // var new_path = DateTime.Now.ToString("yyyyMMdd")+ file.FileName;
                    // var new_path = Path.Combine("uploads/images/", file.FileName);                                        

                    // AbpSession.GetUserId().ToString()
                    // _abpSession.UserId.Value

                    long userId = _abpSession.UserId.Value;
                    
                    var filename = "logo_" + userId + ".png"; //文件改名                    
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/", filename);
                        //var path = Path.Combine(AppContext.BaseDirectory, "images", new_path);                        

                        //var path = @"E:\JLMISIII\SMIC_VUE\vue\public\img";
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            //图片路径保存到数据库里面去
                            //bool flage = QcnApplyDetm.FinancialCreditQcnApplyDetmAdd(EntId, CrtUser, new_path);
                            //if (flage == true)
                            {
                                //再把文件保存的文件夹中
                                file.CopyTo(stream);

                            }
                        }
                    //}
                    //return Json(new { status = 0, message = "上传成功" });
                }
                //return Json(new { status = -1, message = "没有授权" });
                return "OK";
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace);
                //return Json(new { status = -3, message = "上传失败", data = ex.Message });
                return ex.Message;
            }

        }
        
        public string uploadSign(IFormFile file)
        {
            try
            {
                {
                    long userId = _abpSession.UserId.Value;
                    var filename = "sign_" + userId + ".png"; //文件改名                    
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/SignImages/", filename);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                       file.CopyTo(stream);
                    }
                }
                return "OK";
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace);
                //return Json(new { status = -3, message = "上传失败", data = ex.Message });
                return ex.Message;
            }

        }

    }
}
