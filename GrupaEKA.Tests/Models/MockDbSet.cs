using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Collections.ObjectModel;

namespace GrupaEka.Tests.Models
{
    public class MockDbSet<T> : IDbSet<T> where T : class
    {
        private List<T> _data;

        public MockDbSet()
        {
            _data = new List<T>();
        }

        public virtual T Find(params object[] keyValues)
        {
            return (T)(_data.Skip((int)keyValues[0]-1).FirstOrDefault());
            //throw new NotImplementedException();
        }

        public T Frist()
        {
            return _data.First();
        }

        public T Add(T item)
        {
            _data.Add(item);
            return item;
        }

        public T Remove(T item)
        {
            _data.Remove(item);
            return item;
        }

        public T Attach(T item)
        {
            _data.Add(item);
            return item;
        }

        public void Detach(T item)
        {
            _data.Remove(item);
        }

        Type IQueryable.ElementType
        {
            get { return _data.AsQueryable().ElementType; }
        }

        Expression IQueryable.Expression
        {
            get { return _data.AsQueryable().Expression; }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return _data.AsQueryable().Provider; }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        public T Create()
        {
            return Activator.CreateInstance<T>();
        }

        public ObservableCollection<T> Local
        {
            get
            {
                return new ObservableCollection<T>(_data);
            }
        }

        public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
        {
            return Activator.CreateInstance<TDerivedEntity>();
        }
    }
}
