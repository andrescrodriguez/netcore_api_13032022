using CodemmyApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodemmyApi.Repository
{
    public class ArticuloSQL : IArticuloSQL
    {
        private ApplicationDbContext _context;
        public ArticuloSQL(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Articulo> GetAll()
        {
            return _context.Articulo.ToList();
            //return new List<Articulo>()
            //{
            //    new Articulo(){ Id = 1, Titulo = "Titulo 1", Contenido = "Contenido 1", NombreDeRuta = "NombreDeRuta 1" },
            //    new Articulo(){ Id = 2, Titulo = "Titulo 2", Contenido = "Contenido 2", NombreDeRuta = "NombreDeRuta 2" },
            //    new Articulo(){ Id = 3, Titulo = "Titulo 3", Contenido = "Contenido 3", NombreDeRuta = "NombreDeRuta 3" },
            //    new Articulo(){ Id = 4, Titulo = "Titulo 4", Contenido = "Contenido 4", NombreDeRuta = "NombreDeRuta 4" }
            //};
        }
    }
}
