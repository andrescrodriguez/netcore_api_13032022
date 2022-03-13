using CodemmyApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodemmyApi.Repository
{
    public interface IArticuloSQL
    {
        List<Articulo> GetAll();
    }
}
