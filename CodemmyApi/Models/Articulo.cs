using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodemmyApi.Models
{
    public class Articulo
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string PreLectura { get; set; }
        public string Contenido { get; set; }
        public string NombreDeRuta { get; set; }

        [ForeignKey("Categoria")]
        public int IdCategoria { get; set; }
        public Categoria Categoria { get; set; }
        public DateTime? FechaHoraPublicacion { get; set; }
        public DateTime FechaHoraAlta { get; set; }
        public DateTime? FechaHoraUltimaActualizacion { get; set; }
        public DateTime? FechaHoraBaja { get; set; }
        public ICollection<ImagenXArticulo> ImagenesXArticulos { get; set; }

        // propiedad de navegaciòn

    }
}
