﻿using System;
using ZeroCode.Utility.Extensions;

namespace ZeroCode.Repository.Data.Extensions
{
    public static class EntityInterfaceExtensions
    {
        /// <summary>
        /// 检测并执行<see cref="ICreatedTime"/>接口的逻辑
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static TEntity CheckICreatedTime<TEntity,TKey>(this TEntity entity) 
            where TEntity:IEntity<TKey>
            where TKey:IEquatable<TKey>
        {
            if(!(entity is ICreatedTime))
            {
                return entity;
            }
            ICreatedTime entity1 = entity as ICreatedTime;
            entity1.CreateTime = DateTime.Now;
            return (TEntity)entity1;
        }


        /// <summary>
        /// 检测并执行<see cref="ICreatedAudited"/>接口的逻辑
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static TEntity CheckICreatedAudited<TEntity,TKey>(this TEntity entity)
            where TEntity :IEntity<TKey>
            where TKey:IEquatable<TKey>
        {
            if (!(entity is ICreatedAudited))
            {
                return entity;
            }
            ICreatedAudited entity1 = entity as ICreatedAudited;
            //未实现的记录当前用户信息
            entity1.CreateTime = DateTime.Now;
            return (TEntity)entity1;
        }

        /// <summary>
        /// 检测并执行<see cref="IUpdateAudited"/>接口的逻辑，此功能应按需调用
        /// </summary>
        /// <param name="entity">要检测的实体信息</param>
        public static TEntity CheckIUpdateAudited<TEntity, TKey>(this TEntity entity)
            where TEntity : IEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            if (!(entity is IUpdateAudited))
            {
                return entity;
            }
            IUpdateAudited entity1 = entity as IUpdateAudited;
            //未实现的记录当前用户信息
            entity1.LastUpdatedTime = DateTime.Now;
            return (TEntity)entity1;
        }



        /// <summary>
        /// 检测并执行<see cref="IRecyclable"/>接口的逻辑
        /// </summary>
        /// <typeparam name="TEntity">实体基类</typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="entity">要检测的实体信息</param>
        /// <param name="operation">回收站操作类型</param>
        /// <returns></returns>
        public static TEntity CheckIRecycle<TEntity,TKey>(this TEntity entity,RecycleOperation operation)
            where TEntity:IEntity<TKey>
            where TKey:IEquatable<TKey>
        {
            if(!(entity is IRecyclable))
            {
                return entity;
            }
            IRecyclable entity1 = entity as IRecyclable;
            switch(operation)
            {
                case RecycleOperation.LogicDelete:
                    entity1.IsDeleted = true;
                    break;
                case RecycleOperation.Restore:
                    entity1.IsDeleted = false;
                    break;
                case RecycleOperation.PhysicalDelete:
                    if(!entity1.IsDeleted)
                    {
                        throw new InvalidOperationException("数据不处于回收（IsDeleted==true）状态，不能永久删除。");
                    }
                    break;
            }
            return (TEntity)entity1;
        }

        /// <summary>
        /// 判断指定类型是否为<see cref="IEntity{TKey}"/>实体类型
        /// </summary>
        /// <param name="type">要判断的类型</param>
        /// <returns></returns>
        public static bool IsEntityType(this Type type)
        {
            type.CheckNotNull("type");
            return typeof(IEntity<>).IsGenericAssignableFrom(type) && !type.IsAbstract;
        }

        /// <summary>
        /// 判断指定实体是否不在有效期
        /// </summary>
        public static bool IsExpired(this IExpirable entity)
        {
            entity.CheckNotNull("entity");
            DateTime now = DateTime.Now;
            return entity.BeginTime != null && entity.BeginTime.Value > now ||
                entity.EndTime != null && entity.EndTime.Value < now;
        }

    }
}
