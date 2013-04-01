using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SasquatchCAIRS.Models {
    public interface IDataContext {
        IQueryable<T> Repository<T>() where T : class;

        void insert<T>(T item) where T : class;

        void delete<T>(T item) where T : class;

        void submitChanges();
    }
}
