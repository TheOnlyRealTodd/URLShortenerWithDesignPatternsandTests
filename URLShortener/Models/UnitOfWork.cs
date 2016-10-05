using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace URLShortener.Models
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IRepo<Url> Urls { get; set; }
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Urls = new Repo<Url>(_context);
            
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}