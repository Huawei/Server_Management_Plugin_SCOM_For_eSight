// ***********************************************************************
// Assembly         : Huawei.SCOM.ESightPlugin.DAO
// Author           : suxiaobo
// Created          : 11-19-2017
//
// Last Modified By : suxiaobo
// Last Modified On : 12-12-2017
// ***********************************************************************
// <copyright file="IBaseRepository.cs" company="广州摩赛网络技术有限公司">
//     Copyright ©  2017
// </copyright>
// <summary>数据库 DAO 基类</summary>
// ***********************************************************************

namespace Huawei.SCOM.ESightPlugin.DAO
{
    using System.Collections.Generic;

    /// <summary>
    /// 数据库 DAO 基类
    /// </summary>
    /// <typeparam name="T">
    /// 实体Model类
    /// </typeparam>
    public interface IBaseRepository<T>
    {
        /// <summary>
        /// 通过ID查询实体类。
        /// </summary>
        /// <param name="id">
        /// 实体类ID
        /// </param>
        /// <returns>
        /// 填充好数据的实体类
        /// </returns>
        T GetEntityById(int id);

        /// <summary>
        /// 根据条件返回实体类列表。
        /// </summary>
        /// <param name="condition">
        /// 条件
        /// </param>
        /// <returns>
        /// 实体类列表。
        /// </returns>
        IList<T> GetList(string condition);

        /// <summary>
        /// 插入实体类数据到数据库。
        /// </summary>
        /// <param name="entity">
        /// 实体类
        /// </param>
        /// <returns>
        /// 插入数量
        /// </returns>
        int InsertEntity(T entity);

        /// <summary>
        /// 更新实体类
        /// </summary>
        /// <param name="entity">
        /// 实体类
        /// </param>
        /// <returns>
        /// 更新数量
        /// </returns>
        int UpdateEntity(T entity);
    }
}