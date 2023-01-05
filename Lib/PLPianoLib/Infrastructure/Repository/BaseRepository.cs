using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLPianoLib.Infrastructure.Repository
{
    public class BaseRepository<T> : IRepository<T>
    {
        IDbConnection _Connection;
        string _EntityName;
        public BaseRepository(IDbConnection connection) 
        {
            _Connection = connection;
            connection.Open();
            _EntityName = typeof(T).Name;

        }

        public virtual void  CreateTable()
        {
            IDbCommand cmd = _Connection.CreateCommand();
            cmd.CommandText = @"CREATE TABLE cars(id INTEGER PRIMARY KEY,
                name TEXT, price INT)";
            cmd.ExecuteNonQuery();
        }
        public List<T> FindAll(string filter)
        {
            throw new NotImplementedException();
        }

        public List<T> GetAll()
        {
            throw new NotImplementedException();
        }

        public T GetById(object id)
        {
            throw new NotImplementedException();
        }

        public void Save(T entity)
        {
            throw new NotImplementedException();
        }
    }


}
