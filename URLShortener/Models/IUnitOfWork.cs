using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener.Models
{

    //Don't forget to make these interfaces public so they can be accessed from the controllers.
    public interface IUnitOfWork :IDisposable
    {
        IRepo<Url> Urls { get; }
        int Complete();
    }
}
