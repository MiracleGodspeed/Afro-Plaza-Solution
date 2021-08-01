using BusinessLayer.Interface;
using DataLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLayer
{
    public class Repository<T> : IRepository<T> where T : BaseModel
    {
        protected readonly AfrostichContext _context;
        private Microsoft.EntityFrameworkCore.DbSet<T> entities;
        public Repository(AfrostichContext context)
        {
            _context = context;
            entities = _context.Set<T>();
        }
       

        public IEnumerable<T> GetAll()
        {
            return entities.AsEnumerable();
        }

        public T GetById(long Id)
        {
            return entities.SingleOrDefault(f => f.Id == Id);
        }

        
    }
}
