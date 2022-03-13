using System;
using System.Collections.Generic;

namespace CodemmyApi.Models
{
    public class Imagen
    {
        public int Id { get; set; } 
        public string Nombre { get; set; }
        public string Extension { get; set; }
        public string Ruta { get; set; }
        public string TipoDeContenido { get; set; }
        public DateTime FechaHoraAlta { get; set; }
        public ICollection<ImagenXArticulo> ImagenesXArticulos { get; set; }
    }
}
