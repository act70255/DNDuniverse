using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Repository.Interface
{
    public interface IRepository<TDocument>
    {
        IEnumerable<TDocument> GetAll();
        TDocument GetByID(int id);
        void Insert(TDocument entity);
        void Update(TDocument entity);
        void Delete(int id);
    }
}
