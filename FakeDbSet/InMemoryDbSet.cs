using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Collections;

namespace FakeDbSet
{
    /// <summary>
    /// The in-memory database set, taken from Microsoft's online example (http://msdn.microsoft.com/en-us/ff714955.aspx) 
    /// and modified to be based on DbSet instead of ObjectSet.
    /// </summary>
    /// <typeparam name="T">The type of DbSet.</typeparam>
    public class InMemoryDbSet<T> : DbSet<T>, IDbSet<T>, IDbAsyncEnumerable<T>, IQueryable where T : class
    {
        readonly HashSet<T> _data;
        readonly IQueryable _query;

        public InMemoryDbSet() : this(false)
        {
            _data = new HashSet<T>();
            _query = _data.AsQueryable();
        }

        public InMemoryDbSet(bool clearDownExistingData)
        {
            if (clearDownExistingData)
            {
                Clear();
            }
        }

        public void Clear()
        {
            _data.Clear();
        }

        public override T Add(T entity)
        {
            _data.Add(entity);
            return entity;
        }

        public override IEnumerable<T> AddRange(IEnumerable<T> entities)
        {
            var items = entities.ToArray();
            foreach (var item in items)
            {
                Add(item);
            }
            return items;
        }

        public override T Attach(T entity)
        {
            _data.Add(entity);
            return entity;
        }

        public new TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
        {
            throw new NotImplementedException();
        }

        public override T Create()
        {
            return Activator.CreateInstance<T>();
        }

        public new virtual T Find(params object[] keyValues)
        {
            if (keyValues.Length != 1)
                throw new Exception("not implemented");

            var type = typeof(T);
            var prop = type.GetProperty(type.Name + "Id");
            var getter = prop.GetGetMethod();

            var keyType = keyValues[0].GetType();
            if (getter.ReturnType != keyType)
                throw new Exception("mismatch keytype");
            if (getter.ReturnType == typeof(Int32))
                return this.FirstOrDefault(t => (int)getter.Invoke(t, null) == (int)keyValues[0]);
            else if (getter.ReturnType == typeof(Guid))
                return this.FirstOrDefault(t => (Guid)getter.Invoke(t, null) == (Guid)keyValues[0]);
            else
                throw new Exception("unknown keytype");
        }

        public override System.Collections.ObjectModel.ObservableCollection<T> Local
        {
            get { return new System.Collections.ObjectModel.ObservableCollection<T>(_data); }
        }

        public override T Remove(T entity)
        {
            _data.Remove(entity);
            return entity;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        public Type ElementType
        {
            get { return _query.ElementType; }
        }

        public Expression Expression
        {
            get { return _query.Expression; }
        }

        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new DbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return GetAsyncEnumerator();
        }

        IQueryProvider IQueryable.Provider
        {
            get { return new DbAsyncQueryProvider<T>(_data.AsQueryable().Provider); }
        }
    }
}
