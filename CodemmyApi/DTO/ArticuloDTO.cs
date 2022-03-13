using CodemmyApi.Models;
using System;
using System.Collections.Generic;

namespace CodemmyApi.DTO
{
    public class ArticuloDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string PreLectura { get; set; }
        public string Contenido { get; set; }
        public string NombreDeRuta { get; set; }
        public string IdCategoria { get; set; }
        public Categoria Categoria { get; set; }
        public string Nombre { get; set; }
        public DateTime? FechaHoraPublicacion { get; set; }
        public DateTime FechaHoraAlta { get; set; }
        public DateTime? FechaHoraUltimaActualizacion { get; set; }
        public DateTime? FechaHoraBaja { get; set; }
        public int IdImagen { get; set; }
        public List<ImagenXArticulo> ImagenesXArticulos { get; set; }
    }
}
