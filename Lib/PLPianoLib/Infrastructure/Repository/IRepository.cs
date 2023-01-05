using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLPianoLib.Infrastructure.Repository
{
    internal interface IRepository<T>
    {
        void Save(T entity);
        T GetById(object id);
        List<T> GetAll();

        List<T> FindAll(string filter);
    }
}
