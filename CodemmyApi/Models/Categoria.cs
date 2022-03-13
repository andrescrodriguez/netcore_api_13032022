using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodemmyApi.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string NombreDeRuta { get; set; }
        public ICollection<Articulo> Articulos { get; set; }
    }
}
