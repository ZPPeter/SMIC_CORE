
using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using Abp.UI;
using Abp.AutoMapper;
using Abp.Authorization;
using Abp.Linq.Extensions;
using Abp.Domain.Repositories;
using Abp.Application.Services;
using Abp.Application.Services.Dto;


using SMIC.SDIM.Dtos;


namespace SMIC.SDIM
{
    /// <summary>
    /// HomeInfo应用层服务的接口方法
    ///</summary>
    public interface IHomeInfoAppService : IApplicationService
    {
        /// <summary>
		/// 获取HomeInfo的分页列表信息
		///</summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<HomeInfoListDto>> GetPagedAll(GetHomeInfosInput input);


		/// <summary>
		/// 通过指定id获取HomeInfoListDto信息
		/// </summary>
		Task<HomeInfoListDto> GetById(EntityDto<int> input);


        /// <summary>
        /// 返回实体的EditDto
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<GetHomeInfoForEditOutput> GetForEdit(NullableIdDto<int> input);


        /// <summary>
        /// 添加或者修改HomeInfo的公共方法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task CreateOrUpdate(CreateOrUpdateHomeInfoInput input);


        /// <summary>
        /// 删除HomeInfo信息的方法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task Delete(EntityDto<int> input);


        /// <summary>
        /// 批量删除HomeInfo
        /// </summary>
        Task BatchDelete(List<int> input);


		/// <summary>
        /// 导出HomeInfo为excel表
        /// </summary>
        /// <returns></returns>
		//Task<FileDto> GetToExcel();

    }
}
