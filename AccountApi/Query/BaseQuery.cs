using AccountApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace AccountApi.Query
{
    public class BaseQuery<T> : DbContext where T : class
    {
        public BaseQuery()
            : base(Config.SqlConnName)
        { }

        public DbSet<T> _context { get; set; }

        /// <summary>
        /// Получение всех записей сущности
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetList()
        {
            foreach (var item in _context)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Добавление сущности
        /// </summary>
        /// <param name="entity"></param>
        public void CreateEntity(T entity)
        {
            _context.Add(entity);
            SaveChanges();
        }

        /// <summary>
        /// Обновление сущности
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateEntity(T entity)
        {
            _context.Add(entity);
            Entry(entity).State = EntityState.Modified;
            SaveChanges();
        }
    }
}