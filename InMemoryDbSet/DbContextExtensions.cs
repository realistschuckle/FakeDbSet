using System;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Reflection;

namespace InMemoryDbSet
{
    public static class DbContextExtensions
    {
        /// <summary>
        /// Sets up a fake IDbSet.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="dbContext">The DbContext containing the IDbSet to setup.</param>
        /// <param name="expression">The IDbSet to setup.</param>
        /// <returns></returns>
        public static IDbSet<TEntity> SetupFakeDbSet<TEntity, TDbContext>(this TDbContext dbContext, Expression<Func<TDbContext, IDbSet<TEntity>>> expression) where TEntity : class
        {
            var dbSet = new InMemoryDbSet<TEntity>();
            
            var member = expression.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException(String.Format("Expression '{0}' refers to a method, not a property.", expression));
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException(String.Format("Expression '{0}' refers to a field, not a property.", expression));
            }

            propInfo.SetValue(dbContext, dbSet);

            return dbSet;
        }
    }
}
