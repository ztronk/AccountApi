using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AccountApi.Query
{
    public class BaseQuery<T> : DbContext where T : class
    {
        public BaseQuery()
            : base("AccountApiConn")
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
    }
}